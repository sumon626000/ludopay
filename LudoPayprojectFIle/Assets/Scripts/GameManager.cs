using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Space(5.0f)]
    [Header("My Profile")]
    public Texture AvatarImage;
    public Texture KYCImage;
    public string AvatarURL;
    public string KYCImageURL = "";
    public string DeviceToken;
    public string UserName;
    public string UserEmail;
    public string UserID;
    public string FacebookPhone;
    public string GooglePhone;
    public string GuestPhone;
    public string UserPhone;
    public string UserPassword;
    public int Points;
    public int Ant;    
    public string Referral_code;
    public int Referral_count;
    public int Level;
    public int Commission;
    public int MinWithdraw;
    public int ReferBonus;
    public string whatsappLink = "";
    public string youtubeLink = "";
    public string EnableBot = "";
    public string GameVersion;
    public string PurchaseLink = "";
    public string PP, TC;
    public string OTP, OTPResponse;
    public bool ForgotPassword = false ;
    public bool KYC = false, Pawnmoved = false;
    [Space(5.0f)]
    [Header("My History")]
    public HistoryPlay Online_Multiplayer;
    public HistoryPlay Friend_Multiplayer;
    public HistoryToken TokensCaptued;
    public HistoryWonStreaks WonStreaks;
    [Space(5.0f)]
    [Header("Game Config Information")]
    public string KYCStatus;
    public bool isLogin = false;
    public bool GoogleLogined = false;
    public bool IwannaFacebookLogin = false;
    public bool Online_Bot_Mode = false;
    public LOGIN_MODE _Login_Mode = LOGIN_MODE.guest;
    public WIFI _Wifi = WIFI.offline;
    public GameMode _GameMode = GameMode.classic;
    public GamePlayType _GamePlayType = GamePlayType.two;
    public PlayerType _PlayerType = PlayerType.blue;
    [Space(5.0f)]
    [Header("Choose Information")]
    public int[] playerColors = new int[6] { 1, 2, 3, 4, 5, 6 };
    public string[] playerNames;
    [Space(5.0f)]
    [Header("Room Information")]
    public int RoomID;
    public string PrivateRoomId;
    public int RoomStakeMoney;
    public int RoomWinMoney;
    public bool isCreateRoom = false;    
    [Space(5.0f)]
    [Header("Users in online-mode")]
    public List<UserInfo> Users = new List<UserInfo>();
    [Space(5.0f)]
    [Header("Spin")]
    public bool wheelSpin = false;
    [Header("--- Setting Data")]
    public Setting settingData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Start()
    {
#if !GUEST_BUILD
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
#endif
    }

#if !GUEST_BUILD
    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        DeviceToken = token.Token;
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
    }
#endif

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject ExitPanel = GameObject.Find("UI Root/Camera/ExitPanel").gameObject;
            if (ExitPanel != null)
            {
                ExitPanel.SetActive(true);
            }
        }
    }
    public void Vibrating()
    {
#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR

        if(GameManager.Instance.settingData.vibration)
            Handheld.Vibrate();
#endif
    }
    [ContextMenu("Write Setting Data to JSON File")]
    public void WriteSettingData()
    {
        string jsonData = JsonUtility.ToJson(settingData, true);
        string path = "";
#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, "setting.json");
#elif UNITY_EDITOR

        path = Application.streamingAssetsPath + "/" + "setting.json";
#endif
        File.WriteAllText(path, jsonData);
    }

    [ContextMenu("Read Setting Data from JSON File")]
    public void ReadSettingData()
    {
        string path = "";
#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, "setting.json");
#elif UNITY_EDITOR
        path = Application.streamingAssetsPath + "/" + "setting.json";        
#endif
        string jsonData = File.ReadAllText(path);
        settingData = JsonUtility.FromJson<Setting>(jsonData);        
        if (settingData == null)
            settingData = new Setting();
    }
    
}

[Serializable]
public class UserInfo
{
    public string phone;
    public string name;
    public int points;
    public int level;
}

[Serializable]
public class HistoryPlay
{
    public int played;
    public int won;
}
[Serializable]
public class HistoryToken
{
    public int mine;
    public int opponents;
}
[Serializable]
public class HistoryWonStreaks
{
    public int current;
    public int best;
}

[Serializable]
public class Setting
{
    public bool music;
    public bool sound;
    public bool vibration;
    public bool notification;
}

