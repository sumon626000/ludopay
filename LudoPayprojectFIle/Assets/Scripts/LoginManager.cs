using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIO;
using System;
using System.Text.RegularExpressions;

public class LoginManager : MonoBehaviour
{
    private MenuManager menuManager;
    [System.NonSerialized]
    public GameObject loading;
    public GameObject privacyPanel;
    private UILabel lbPP, lbTC;
    private SocketIOComponent socket;
    public bool delete_PlayerPrefs = false;
    public RoomManager Roommanager;    
    public FirebaseFacebookManager FFBManager;
    public UIInput regName, regPass, regConfrimPass, regEmail, regPhone, logPhone, logPass, newPass, newConfimPass;    
    private Transform privacyTrans1, privacyTrans2;
    public MultiplayerModeSettings mpModeSetting;
    private bool startupSyncDone = false;
    private bool socketHandlersAttached = false;

    private bool SocketReady()
    {
        return socket != null && !socket.disconnected && !string.IsNullOrEmpty(socket.sid);
    }

    private void RestoreSessionIfLoggedIn()
    {
        if (!PlayerPrefs.HasKey("LoginType"))
            return;

        switch (PlayerPrefs.GetString("LoginType"))
        {
            case "Google":
                GameManager.Instance.UserPhone = PlayerPrefs.GetString("GooglePhone");
                GameManager.Instance.UserPassword = PlayerPrefs.GetString("Password", "");
                break;
            case "Facebook":
                GameManager.Instance.UserPhone = PlayerPrefs.GetString("FacebookPhone");
                GameManager.Instance.UserPassword = PlayerPrefs.GetString("Password", "");
                break;
            case "Guest":
                GameManager.Instance.UserPhone = PlayerPrefs.GetString("GuestPhone");
                GameManager.Instance.UserPassword = PlayerPrefs.GetString("Password", "");
                break;
            case "Phone":
                GameManager.Instance.UserPhone = PlayerPrefs.GetString("UserPhone");
                GameManager.Instance.UserPassword = PlayerPrefs.GetString("Password");
                break;
        }
    }

    private bool EnsureSocket()
    {
        if (socket != null && socketHandlersAttached)
            return true;

        SocketManager sm = SocketManager.Instance;
        if (sm == null)
            sm = FindObjectOfType<SocketManager>();

        if (sm == null)
        {
            Debug.LogError("SocketManager not found. Add SocketIO object to MenuScene.");
            return false;
        }

        socket = sm.GetSocketIOComponent();
        if (socket == null)
        {
            Debug.LogError("SocketIOComponent missing on SocketIO object.");
            return false;
        }

        if (!socketHandlersAttached)
        {
            socket.On("GET_LOGIN_RESULT", WithLogging("GET_LOGIN_RESULT", OnGetLoginResult));
            socket.On("GET_REGISTER_RESULT", WithLogging("GET_REGISTER_RESULT", OnGetRegisterResult));
            socket.On("REQ_VALID_PHONE_RESULT", WithLogging("REQ_VALID_PHONE_RESULT", OnGetValidAccountResult));
            socket.On("REQ_GAMESETTINGS_RESULT", WithLogging("REQ_GAMESETTINGS_RESULT", OnGetSettingResult));
            socket.On("GET_CHANGEPASS_RESULT", WithLogging("GET_CHANGEPASS_RESULT", OnGetChangePassResult));
            socket.On("connect", OnSocketConnected);
            socketHandlersAttached = true;
        }

        return true;
    }

    void Awake()
    {
        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.GameVersion)
            && GameManager.Instance.GameVersion != Application.version)
        {
            Debug.LogWarning("Server game version differs from app build: server=" +
                GameManager.Instance.GameVersion + " app=" + Application.version);
        }

        menuManager = transform.parent != null ? transform.parent.GetComponent<MenuManager>() : null;
        Transform loadingTf = transform.Find("Loading");
        if (loadingTf != null)
            loading = loadingTf.gameObject;

        if (privacyPanel != null)
        {
            Transform pp = privacyPanel.transform.Find("PP");
            Transform tc = privacyPanel.transform.Find("TC");
            if (pp != null && pp.childCount > 0) lbPP = pp.GetChild(0).GetComponent<UILabel>();
            if (tc != null && tc.childCount > 0) lbTC = tc.GetChild(0).GetComponent<UILabel>();
        }

        Transform loginPanel = transform.Find("LoginInputPanel");
        Transform registerPanel = transform.Find("RegisterInputPanel");
        if (loginPanel != null) privacyTrans1 = loginPanel.Find("Privacy");
        if (registerPanel != null) privacyTrans2 = registerPanel.Find("Privacy");

        if (GameManager.Instance != null)
        {
            if (!GameManager.Instance.isLogin)
            {
                GameManager.Instance.UserPhone = "";
                GameManager.Instance.UserPassword = "";
            }
            else
            {
                RestoreSessionIfLoggedIn();
            }
        }

        if (loading != null)
            loading.SetActive(PlayerPrefs.HasKey("LoginType") && (GameManager.Instance == null || !GameManager.Instance.isLogin));
    }
    private Action<SocketIOEvent> WithLogging(string eventKey, Action<SocketIOEvent> handler)
    {
        return (evt) => {
            Debug.Log($"Socket Event: {eventKey}\nData: {evt.data}");
            handler(evt);
        };
    }
    private void Start()
    {
        StartCoroutine(InitAfterSceneReady());
    }

    private IEnumerator InitAfterSceneReady()
    {
        float wait = 5f;
        while (wait > 0f && !EnsureSocket())
        {
            wait -= Time.deltaTime;
            yield return null;
        }

        if (!EnsureSocket())
        {
            if (menuManager != null)
                menuManager.On_MessagePanel("SocketIO missing in scene.\r\nAdd SocketIO prefab to MenuScene.");
            yield break;
        }

        CheckPrivacy();
        Invoke("TryStartupSync", 3.0f);

        if (GameManager.Instance != null && GameManager.Instance.GoogleLogined)
        {
            if (loading != null) loading.SetActive(true);
            Invoke("Register", 2.0f);
        }
    }

    private void OnSocketConnected(SocketIOEvent evt)
    {
        TryStartupSync();
    }

    private void TryStartupSync()
    {
        if (startupSyncDone || !SocketReady())
            return;

        startupSyncDone = true;
        LoadGameSettings();
        EnsureRewardAdManager().RefreshConfigFromServer();

        if (GameManager.Instance.isLogin == false)
            AutoLogin();
    }

    private void AutoLogin()
    {
        if (PlayerPrefs.HasKey("LoginType") == false)
            return;
        
        string logType = PlayerPrefs.GetString("LoginType");
        switch (logType)
        {
            case "Google":
                GameManager.Instance.UserPhone = PlayerPrefs.GetString("GooglePhone");
                GameManager.Instance._Login_Mode = LOGIN_MODE.google;
                Login(GameManager.Instance.UserPhone);
                break;
            case "Facebook":
                GameManager.Instance.UserPhone = PlayerPrefs.GetString("FacebookPhone");
                GameManager.Instance._Login_Mode = LOGIN_MODE.facebook;
                Login(GameManager.Instance.UserPhone);
                break;
            case "Guest":
                GameManager.Instance.UserPhone = PlayerPrefs.GetString("GuestPhone");
                GameManager.Instance._Login_Mode = LOGIN_MODE.guest;
                Login(GameManager.Instance.UserPhone);
                break;
            case "Phone":
                GameManager.Instance.UserPhone = PlayerPrefs.GetString("UserPhone");
                GameManager.Instance.UserPassword = PlayerPrefs.GetString("Password");
                GameManager.Instance._Login_Mode = LOGIN_MODE.phone;
                Login(GameManager.Instance.UserPhone, GameManager.Instance.UserPassword);
                break;
        }
    }

    private void OnGetSettingResult(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        if(result == "success")
        {
            GameManager.Instance.Commission = int.Parse(Global.JsonToString(evt.data.GetField("commission").ToString(), "\""));
            GameManager.Instance.MinWithdraw = int.Parse(Global.JsonToString(evt.data.GetField("min_withdraw").ToString(), "\""));
            GameManager.Instance.ReferBonus = int.Parse(Global.JsonToString(evt.data.GetField("refer_bonus").ToString(), "\""));
            GameManager.Instance.whatsappLink = Global.JsonToString(evt.data.GetField("whatsapp_link").ToString(), "\"");
            GameManager.Instance.youtubeLink = Global.JsonToString(evt.data.GetField("youtube_link").ToString(), "\"");
            GameManager.Instance.EnableBot = Global.JsonToString(evt.data.GetField("bot_mode").ToString(), "\"");
            GameManager.Instance.PurchaseLink = Global.JsonToString(evt.data.GetField("purchase_link").ToString(), "\"");
            GameManager.Instance.PP = Global.JsonToString(evt.data.GetField("privacy_desc").ToString(), "\"");
            GameManager.Instance.TC = Global.JsonToString(evt.data.GetField("terms_desc").ToString(), "\"");
        }
        else
        {

        }
    }

    


    private void OnDestroy()
    {
        if (socket != null)
        {
            socket.Off("GET_LOGIN_RESULT", OnGetLoginResult);
            socket.Off("GET_REGISTER_RESULT", OnGetRegisterResult);
            socket.Off("REQ_VALID_PHONE_RESULT", OnGetValidAccountResult);
            socket.Off("REQ_GAMESETTINGS_RESULT", OnGetSettingResult);
            socket.Off("GET_CHANGEPASS_RESULT", OnGetChangePassResult);
            socket.Off("connect", OnSocketConnected);
        }
    }

    private static RewardAdManager EnsureRewardAdManager()
    {
        if (RewardAdManager.Instance != null)
            return RewardAdManager.Instance;
        var existing = FindObjectOfType<RewardAdManager>();
        if (existing != null)
            return existing;
        var go = new GameObject("RewardAdManager");
        return go.AddComponent<RewardAdManager>();
    }

    private void LoadGameSettings()
    {        
        socket.Emit("REQ_GAME_SETTINGS");        
    }

    private void DelayRegister()
    {        
        Register();
    }
    
    public void Login_Guest()
    {
        if (PlayerPrefs.HasKey("Privacy") == false)
        {
            if (menuManager != null)
                menuManager.On_MessagePanel("Please Check Privacy Policy and \r\n Terms & Conditions");
            return;
        }

        if (GameManager.Instance == null)
            return;

        GameManager.Instance._Login_Mode = LOGIN_MODE.guest;
        GameManager.Instance.UserPassword = "";

        if (PlayerPrefs.HasKey("GuestPhone"))
        {
            GameManager.Instance.UserPhone = PlayerPrefs.GetString("GuestPhone");
            Login(GameManager.Instance.UserPhone);
            return;
        }

        // New guest: auto phone, no OTP / no phone panel
        GameManager.Instance.UserPhone = "9" + UnityEngine.Random.Range(100000000, 999999999).ToString();
        GameManager.Instance.UserName = "";
        GameManager.Instance.UserEmail = "";
        if (loading != null) loading.SetActive(true);
        Valid_Account();
    }

    public void Login_Facebook()
    {
        if (PlayerPrefs.HasKey("Privacy") == false)
        {
            menuManager.On_MessagePanel("Please Check Privacy Policy and \r\n Terms & Conditions");
            return;
        }
        if (PlayerPrefs.HasKey("LoginType") == false && PlayerPrefs.HasKey("FacebookPhone") == true)
        {
            GameManager.Instance.UserPhone = PlayerPrefs.GetString("FacebookPhone");
            GameManager.Instance._Login_Mode = LOGIN_MODE.facebook;
            Login(GameManager.Instance.UserPhone);
            return;
        }

        GameManager.Instance._Login_Mode = LOGIN_MODE.facebook;        
        menuManager.On_PhoneVerifyPanel();
    }

    public void Login_Google()
    {
        if (PlayerPrefs.HasKey("Privacy") == false)
        {
            menuManager.On_MessagePanel("Please Check Privacy Policy and \r\n Terms & Conditions");
            return;
        }
        if (PlayerPrefs.HasKey("LoginType") == false && PlayerPrefs.HasKey("GooglePhone") == true)
        {
            print("already logined in google ....");
            GameManager.Instance.UserPhone = PlayerPrefs.GetString("GooglePhone");
            GameManager.Instance._Login_Mode = LOGIN_MODE.google;
            Login(GameManager.Instance.UserPhone);
            return;
        }

        GameManager.Instance._Login_Mode = LOGIN_MODE.google;
        menuManager.On_PhoneVerifyPanel();
    }

    public void Login_Phone()
    {
        if (PlayerPrefs.HasKey("Privacy") == false)
        {
            menuManager.On_MessagePanel("Please Check Privacy Policy and \r\n Terms & Conditions");
            return;
        }
        GameManager.Instance._Login_Mode = LOGIN_MODE.phone;

        if (logPhone.value.Length < 10 || logPass.value.Length < 6)
        {
            logPhone.value = "";
            logPass.value = "";
            menuManager.On_MessagePanel("Please Input Corret Login Information.");
            return;
        }

        GameManager.Instance._Login_Mode = LOGIN_MODE.phone;
        GameManager.Instance.UserPhone = logPhone.value;
        GameManager.Instance.UserPassword = logPass.value;
        Login(logPhone.value, logPass.value);
    }

    public void Click_Register()
    {
        if(regName.value.Length == 0)
        {
            menuManager.On_MessagePanel("Please input correct Name");
            regName.value = "";
            return;
        }
        if (regPass.value.Length < 6)
        {
            menuManager.On_MessagePanel("Password should be more 6 charactors");
            regPass.value = "";
            return;
        }        
        if (regPass.value != regConfrimPass.value)
        {
            menuManager.On_MessagePanel("Please confirm password again");
            regConfrimPass.value = "";
            return;
        }
        if (!Regex.IsMatch(regEmail.value, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$"))
        {
            menuManager.On_MessagePanel("Please input correct email");
            regEmail.value = "";
            return; 
        }
        if(regPhone.value.Length < 10)
        {
            menuManager.On_MessagePanel("Please input correct Mobile Number");
            regPhone.value = "";
            return;
        }
        if (PlayerPrefs.HasKey("Privacy") == false)
        {
            menuManager.On_MessagePanel("Please Check Privacy Policy and \r\n Terms & Conditions");
            return;
        }

        GameManager.Instance.UserName = regName.value;
        GameManager.Instance.UserEmail = regEmail.value;
        GameManager.Instance.UserPassword = regPass.value;
        GameManager.Instance.UserPhone = regPhone.value;
        GameManager.Instance._Login_Mode = LOGIN_MODE.phone;

        loading.SetActive(true);
        Valid_Account();
    }


    #region socket emits
    private void EmitWhenConnected(Action action)
    {
        if (!EnsureSocket())
        {
            if (menuManager != null)
                menuManager.On_MessagePanel("SocketIO not ready.");
            return;
        }
        if (SocketReady())
        {
            action();
            return;
        }
        if (loading != null && !loading.activeSelf)
            loading.SetActive(true);
        StartCoroutine(WaitAndEmit(action));
    }

    private IEnumerator WaitAndEmit(Action action)
    {
        float timeout = 12f;
        while (timeout > 0f && !SocketReady())
        {
            timeout -= Time.deltaTime;
            yield return null;
        }
        if (!SocketReady())
        {
            loading.SetActive(false);
            menuManager.On_MessagePanel("Cannot connect to server.\r\n1) Run start-server-local.bat\r\n2) Socket URL: ws://127.0.0.1:3000/socket.io/?EIO=3&transport=websocket");
            yield break;
        }
        action();
    }

    public void Valid_Account()
    {
        EmitWhenConnected(() =>
        {
            loading.SetActive(true);
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("phone", GameManager.Instance.UserPhone);
            JSONObject jdata = new JSONObject(data);
            socket.Emit("REQ_VALID_PHONE", jdata);
        });
    }

    private void Login(string phone, string password = "")
    {
        EmitWhenConnected(() =>
        {
            loading.SetActive(true);
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("userphone", phone);
            data.Add("userpassword", password ?? "");
            JSONObject jdata = new JSONObject(data);
            socket.Emit("REQ_LOGIN", jdata);
        });
    }

    public void Register()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager missing.");
            return;
        }
        if (!EnsureSocket())
        {
            if (menuManager != null)
                menuManager.On_MessagePanel("Server connection not ready.");
            if (loading != null) loading.SetActive(false);
            return;
        }

        string signtype = "";
        switch (GameManager.Instance._Login_Mode)
        {
            case LOGIN_MODE.facebook:
                PlayerPrefs.SetString("LoginType", "Facebook");
                PlayerPrefs.SetString("FacebookPhone", GameManager.Instance.UserPhone);
                PlayerPrefs.SetString("Password", "");
                signtype = "facebook";
                break;
            case LOGIN_MODE.google:
                PlayerPrefs.SetString("LoginType", "Google");
                PlayerPrefs.SetString("GooglePhone", GameManager.Instance.UserPhone);
                PlayerPrefs.SetString("Password", "");
                signtype = "google";
                break;
            case LOGIN_MODE.guest:
                PlayerPrefs.SetString("LoginType", "Guest");
                PlayerPrefs.SetString("GuestPhone", GameManager.Instance.UserPhone);
                PlayerPrefs.SetString("Password", "");
                signtype = "guest";
                break;
            case LOGIN_MODE.phone:
                signtype = "phone";
                PlayerPrefs.SetString("LoginType", "Phone");
                PlayerPrefs.SetString("UserPhone", GameManager.Instance.UserPhone);
                PlayerPrefs.SetString("Password", GameManager.Instance.UserPassword);
                break;            
        }

        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("signtype", signtype ?? "guest");
        data.Add("username", GameManager.Instance.UserName ?? "");
        data.Add("userphone", GameManager.Instance.UserPhone ?? "");
        data.Add("useremail", GameManager.Instance.UserEmail ?? "");
        data.Add("userpassword", GameManager.Instance.UserPassword ?? "");
        data.Add("userphoto", GameManager.Instance.AvatarURL ?? "");
        data.Add("device_token", GameManager.Instance.DeviceToken ?? "");
        JSONObject jdata = new JSONObject(data);
        EmitWhenConnected(() =>
        {
            if (loading != null) loading.SetActive(true);
            if (socket != null) socket.Emit("REQ_REGISTER", jdata);
        });
    }
    #endregion

    #region socket events
    private void OnGetValidAccountResult(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");

        if (result.Equals("success"))
        {
            if (GameManager.Instance.ForgotPassword)
            {
                menuManager.On_MessagePanel("No registered account");
            }
            else
            {
                switch (GameManager.Instance._Login_Mode)
                {
                    case LOGIN_MODE.facebook:
                        FFBManager.FacebooklogIn();
                        break;
                    case LOGIN_MODE.google:
                        OnGoogleLogin();
                        break;
                    case LOGIN_MODE.guest:
                        Register();
                        break;
                    case LOGIN_MODE.phone:
                        Register();
                        break;
                }
            }            
        }
        else
        {
            if (GameManager.Instance.ForgotPassword)
                menuManager.On_CreateNewPassword();
            else
                Login(GameManager.Instance.UserPhone, GameManager.Instance.UserPassword); 
        }
            
    }
    private void OnGetLoginResult(SocketIOEvent evt)
    {
        loading.SetActive(false);
        try
        {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        if (result.Equals("success") || result.Equals("already logined"))
        {
            GameManager.Instance.UserName = Global.JsonToString(evt.data.GetField("username").ToString(), "\"");
            GameManager.Instance.UserEmail = Global.JsonToString(evt.data.GetField("useremail").ToString(), "\"");
            GameManager.Instance.UserID = Global.JsonToString(evt.data.GetField("userid").ToString(), "\"");
            GameManager.Instance.UserPhone = Global.JsonToString(evt.data.GetField("userphone").ToString(), "\"");
            GameManager.Instance.UserPassword = Global.JsonToString(evt.data.GetField("pass").ToString(), "\"");
            GameManager.Instance.AvatarURL = Global.JsonToString(evt.data.GetField("photo").ToString(), "\"");
            GameManager.Instance.Points = int.Parse(evt.data.GetField("points").ToString());
            GameManager.Instance.Level = int.Parse(evt.data.GetField("level").ToString());
			GameManager.Instance.Referral_count = int.Parse(evt.data.GetField("referral_count").ToString());
            GameManager.Instance.Referral_code = Global.JsonToString(evt.data.GetField("referral_code").ToString(), "\"");
            GameManager.Instance.Ant = int.Parse(evt.data.GetField("ant").ToString());
            GameManager.Instance.KYCStatus = evt.data.GetField("kyc_status").ToString();
            print("received -----------");
            JSONObject online_multiplayer = evt.data.GetField("online_multiplayer");
            GameManager.Instance.Online_Multiplayer.played = int.Parse(online_multiplayer.GetField("played").ToString());
            GameManager.Instance.Online_Multiplayer.won = int.Parse(online_multiplayer.GetField("won").ToString());
            JSONObject friend_multiplayer = evt.data.GetField("friend_multiplayer");
            GameManager.Instance.Friend_Multiplayer.played = int.Parse(friend_multiplayer.GetField("played").ToString());
            GameManager.Instance.Friend_Multiplayer.won = int.Parse(friend_multiplayer.GetField("won").ToString());
            JSONObject tokens_captured = evt.data.GetField("tokens_captured");
            GameManager.Instance.TokensCaptued.mine = int.Parse(tokens_captured.GetField("mine").ToString());
            GameManager.Instance.TokensCaptued.opponents = int.Parse(tokens_captured.GetField("opponents").ToString());
            JSONObject won_streaks = evt.data.GetField("won_streaks");
            GameManager.Instance.WonStreaks.current = int.Parse(won_streaks.GetField("current").ToString());
            GameManager.Instance.WonStreaks.best = int.Parse(won_streaks.GetField("best").ToString());

            switch (GameManager.Instance._Login_Mode)
            {
                case LOGIN_MODE.facebook:
                    PlayerPrefs.SetString("LoginType", "Facebook");
                    PlayerPrefs.SetString("FacebookPhone",GameManager.Instance.UserPhone);
                    PlayerPrefs.SetString("Password", GameManager.Instance.UserPassword);
                    break;
                case LOGIN_MODE.google:
                    PlayerPrefs.SetString("LoginType", "Google");
                    PlayerPrefs.SetString("GooglePhone", GameManager.Instance.UserPhone);
                    PlayerPrefs.SetString("Password", GameManager.Instance.UserPassword);
                    break;
                case LOGIN_MODE.guest:
                    PlayerPrefs.SetString("LoginType", "Guest");
                    PlayerPrefs.SetString("GuestPhone", GameManager.Instance.UserPhone);
                    PlayerPrefs.SetString("Password", GameManager.Instance.UserPassword);
                    break;
                case LOGIN_MODE.phone:
                    PlayerPrefs.SetString("LoginType", "Phone");
                    PlayerPrefs.SetString("UserPhone", GameManager.Instance.UserPhone);
                    PlayerPrefs.SetString("Password", GameManager.Instance.UserPassword);
                    break;
            }

            LoginSuccess();
        }
        else
        {
            Debug.Log("login error: " + result);

            switch (GameManager.Instance._Login_Mode)
            {
                case LOGIN_MODE.facebook:
                    menuManager.On_MessagePanel("This Mobile already registered. \r\n Please try as another mobile !");
                    break;
                case LOGIN_MODE.google:
                    menuManager.On_MessagePanel("This Mobile already registered. \r\n Please try as another mobile !");
                    break;
                case LOGIN_MODE.guest:
                    menuManager.On_MessagePanel("Guest login failed.\r\nUse a new 10-digit number or restart the server.");
                    if (result.Equals("failed"))
                        menuManager.On_PhoneVerifyPanel();
                    break;
                case LOGIN_MODE.phone:
                    menuManager.On_MessagePanel("Login Info is not correct. \r\n Login Failed !");
                    logPass.value = "";
                    logPhone.value = "";
                    break;
                default:
                    menuManager.On_MessagePanel("Login Info is not correct. \r\n Login Failed !");
                    break;
            }            
        }
        }
        catch (Exception ex)
        {
            Debug.LogError("Login parse error: " + ex.Message);
            menuManager.On_MessagePanel("Login response error. Restart server and try again.");
        }
    }

    private void OnGetRegisterResult(SocketIOEvent evt)
    {
        loading.SetActive(false);
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        if (result != "success")
        {
            Debug.Log("register error");
            menuManager.On_MessagePanel("Registration failed. Check server is running.");
            return;
        }

        GameManager.Instance.UserName = Global.JsonToString(evt.data.GetField("username").ToString(), "\"");
        GameManager.Instance.UserID = Global.JsonToString(evt.data.GetField("userid").ToString(), "\"");
        GameManager.Instance.Points = int.Parse(evt.data.GetField("points").ToString());
        GameManager.Instance.Referral_code = Global.JsonToString(evt.data.GetField("referral_code").ToString(), "\"");
        if (evt.data.HasField("userphone"))
            GameManager.Instance.UserPhone = Global.JsonToString(evt.data.GetField("userphone").ToString(), "\"");

        PlayerPrefs.SetString("USERNAME", GameManager.Instance.UserName);
        if (GameManager.Instance.UserName.Contains("Guest"))
            PlayerPrefs.SetString("GUESTNAME", GameManager.Instance.UserName);

        SignupSuccess();
    }
    #endregion

    void LoginSuccess()
    {
        GameManager.Instance.isLogin = true;
        menuManager.On_Home();                
    }
    void SignupSuccess()
    {
        GameManager.Instance.isLogin = true;
        PlayerPrefs.SetString("LoginType", "Guest");
        PlayerPrefs.SetString("GuestPhone", GameManager.Instance.UserPhone);
        PlayerPrefs.Save();

        if (GameManager.Instance._Login_Mode == LOGIN_MODE.guest)
            LoginSuccess();
        else
            menuManager.On_ReferralCode();
    }

    IEnumerator WaitingLogin()
    {
        yield return new WaitForSeconds(10.0f);
        loading.SetActive(false);
        GameManager.Instance.UserName = "Player1";
        LoginSuccess();
    }
    public void OnGoogleLogin()
    {
        SceneManager.LoadScene("SnsLogin");
    }

    private void CheckPrivacy()
    {
        lbPP.text = GameManager.Instance.PP;
        lbTC.text = GameManager.Instance.TC;
        if (PlayerPrefs.HasKey("Privacy") == false)
        {
            privacyTrans1.GetComponent<UIButton>().enabled = true;
            privacyTrans1.Find("On").gameObject.SetActive(false);
            privacyTrans2.Find("On").gameObject.SetActive(false);            
        }
        else
        {
            privacyTrans1.GetComponent<UIButton>().enabled = false;
            privacyTrans1.Find("On").gameObject.SetActive(true);
            privacyTrans2.Find("On").gameObject.SetActive(true);
        }
    }
    
    public void ClickPrivacyPolicy()
    {
        PlayerPrefs.SetString("Privacy", "True");
        privacyTrans1.GetComponent<UIButton>().enabled = false;
        privacyTrans1.Find("On").gameObject.SetActive(true);
        privacyTrans2.Find("On").gameObject.SetActive(true);
        lbPP.text = GameManager.Instance.PP;
        lbTC.text = GameManager.Instance.TC;
        privacyPanel.SetActive(true);
    }

    public void ClickCreateNewPass()
    {
        if(newPass.value.Length < 6)
        {
            menuManager.On_MessagePanel("Password should be more 6 charactors");
            newPass.value = "";
            return;
        }
        if(newPass.value != newConfimPass.value)
        {
            menuManager.On_MessagePanel("Please Confirm the password");
            newConfimPass.value = "";
            return;
        }

        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("userphone", GameManager.Instance.UserPhone);
        data.Add("newpassword", newPass.value);
        JSONObject jdata = new JSONObject(data);
        socket.Emit("REQ_CHANGE_PASSWORD", jdata);
    }

    private void OnGetChangePassResult(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        GameManager.Instance.UserPassword = Global.JsonToString(evt.data.GetField("newpassword").ToString(), "\"");
        if (result == "success")
        {
            GameManager.Instance._Login_Mode = LOGIN_MODE.phone;
            Login(GameManager.Instance.UserPhone, GameManager.Instance.UserPassword);
            GameManager.Instance.ForgotPassword = false;
        }
    }
}

public enum LOGIN_MODE
{
    facebook,
    google,
    guest,
    phone
}


