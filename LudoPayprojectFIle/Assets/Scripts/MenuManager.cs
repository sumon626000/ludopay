using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject RegisterInputPanel, LoginInputPanel, LoginPanel, ProfilePanel, HomePanel, SettingPanel,  MultiplayerModePanel, GameRulesPanel, StartPanel,
        PlayWithFriendsPanel,KYCPanel, MYProfilePanel, SpecialOfferPanel, CreateRoomPanel, JoinRoomLobbyPanel, JoinRoomPanel, ReferralCodePanel, PhoneVerifyPanel, CreateNewPassPanel, messagePanel;
    public RoomManager Roommanager;
    private bool isOpenProfile = false;

    void Start()
    {
        Roommanager = transform.Find("RoomManager").GetComponent<RoomManager>();        
        if(GameManager.Instance.isLogin == false)
        {
            StartPanel.SetActive(true);
            LoginPanel.SetActive(true);
            HomePanel.SetActive(false);
            ProfilePanel.SetActive(false);
        }
        else
        {
            HomePanel.SetActive(true);
            ProfilePanel.SetActive(true);
            StartPanel.SetActive(false);
            LoginPanel.SetActive(false);            
        }
        
        RegisterInputPanel.SetActive(false);
        LoginInputPanel.SetActive(false);
        SettingPanel.SetActive(false);
        MultiplayerModePanel.SetActive(false);

        GameManager.Instance.ReadSettingData();
        RefreshRewardAdConfig();

        if (GetComponent<PlayStoreUiGuard>() == null)
            gameObject.AddComponent<PlayStoreUiGuard>();
    }
    public void On_ReferralCode()
    {
        LoginPanel.SetActive(false);
        ProfilePanel.SetActive(true);
        ReferralCodePanel.SetActive(true);
    }

    public void On_RegisterInput()
    {
        StartPanel.SetActive(false);
        LoginInputPanel.SetActive(false);
        RegisterInputPanel.SetActive(true);
    }

    public void On_LoginInput()
    {
        StartPanel.SetActive(false);
        RegisterInputPanel.SetActive(false);
        LoginInputPanel.SetActive(true);
    }

    public void On_Home()
    {
        LoginPanel.SetActive(false);
        ProfilePanel.SetActive(true);
        HomePanel.SetActive(true);
        RefreshRewardAdConfig();
    }

    void RefreshRewardAdConfig()
    {
        if (RewardAdManager.Instance != null)
            RewardAdManager.Instance.RefreshConfigFromServer();
    }
    public void On_Settings()
    {
        HomePanel.SetActive(false);
        SettingPanel.SetActive(true);
    }
    public void On_SpecialOffer()
    {
        //HomePanel.SetActive(false);
        //StorePanel.GetComponent<StoreManager>().openType = i;
        HomePanel.GetComponent<HomeManager>().On_SpecialOffer();
    }
    public void On_MultiplayerModes()
    {
        HomePanel.SetActive(false);
        MultiplayerModePanel.SetActive(true);
    }
    public void On_GameRules()
    {
        SettingPanel.SetActive(false);
        GameRulesPanel.SetActive(true);
    }
    public void Off_GameRules()
    {
        GameRulesPanel.GetComponent<UIPopup>().Close();
        Invoke("Return_settings", 0.4f);
    }
    void Return_settings()
    {
        On_Settings();
    }
    void Return()
    {
        On_Home();
    }
    public void On_PlayWithFriends()
    {
        HomePanel.SetActive(false);
        PlayWithFriendsPanel.SetActive(true);
    }
    public void Off_PlayWithFriends()
    {
        PlayWithFriendsPanel.GetComponent<UIPopup>().Close();
        Invoke("Return", 0.4f);
    }
    public void On_MyProfile()
    {
        isOpenProfile = false;
        HomePanel.SetActive(false);        
        SettingPanel.SetActive(false); MultiplayerModePanel.SetActive(false); GameRulesPanel.SetActive(false); 
        PlayWithFriendsPanel.SetActive(false); SpecialOfferPanel.SetActive(false);
        CreateRoomPanel.SetActive(false); JoinRoomLobbyPanel.SetActive(false); JoinRoomPanel.SetActive(false);
        MYProfilePanel.SetActive(true);
    }
    public void On_MyProfile_Setting()
    {
        isOpenProfile = true;
        SettingPanel.SetActive(false);
        MYProfilePanel.SetActive(true);
    }

    public void On_KYCPanel()
    {
        if (!PlayStoreCompliance.RealMoneyFeaturesEnabled)
            return;

        GameManager.Instance.KYC = true;
        KYCPanel.SetActive(true);
    }
    public void Off_KYCPanel()
    {
        GameManager.Instance.KYC = false;
        HomePanel.SetActive(true);
        KYCPanel.SetActive(false);
        if (MYProfilePanel.activeSelf)
            MYProfilePanel.SetActive(false);
    }

    public void On_PhoneVerifyPanel()
    {
        if(LoginInputPanel.activeSelf)
            LoginInputPanel.SetActive(false);
        if (RegisterInputPanel.activeSelf)
            RegisterInputPanel.SetActive(false);
        
        PhoneVerifyPanel.SetActive(true);
        PhoneVerifyPanel.transform.GetChild(0).gameObject.SetActive(true);
        PhoneVerifyPanel.transform.GetChild(1).gameObject.SetActive(false);
        PhoneVerifyPanel.transform.GetChild(0).Find("Input_phone").GetComponent<UIInput>().value = "";
        PhoneVerifyPanel.transform.GetChild(1).Find("Input_Code").GetComponent<UIInput>().value = "";
    }

    public void On_ForgotPassword()
    {
        GameManager.Instance.ForgotPassword = true;
    }

    

    public void Off_PhoneVerifyPanel()
    {
        if (LoginInputPanel.activeSelf == false)
            LoginInputPanel.SetActive(true);
        if (RegisterInputPanel.activeSelf == true)
            RegisterInputPanel.SetActive(false);

        PhoneVerifyPanel.SetActive(false);
        CreateNewPassPanel.SetActive(false);
        PhoneVerifyPanel.transform.GetChild(0).gameObject.SetActive(true);
        PhoneVerifyPanel.transform.GetChild(1).gameObject.SetActive(false);
        PhoneVerifyPanel.transform.GetChild(0).Find("Input_phone").GetComponent<UIInput>().value = "";
        PhoneVerifyPanel.transform.GetChild(1).Find("Input_Code").GetComponent<UIInput>().value = "";
    }

    public void On_PhoneNumberSend()
    {
        PhoneVerifyPanel.transform.GetChild(1).gameObject.SetActive(true);
        PhoneVerifyPanel.transform.GetChild(0).gameObject.SetActive(false);
        PhoneVerifyPanel.transform.GetChild(0).Find("Input_phone").GetComponent<UIInput>().value = "";
        PhoneVerifyPanel.transform.GetChild(1).Find("Input_Code").GetComponent<UIInput>().value = "";
    }

    public void Off_PhoneNumberSend()
    {
        PhoneVerifyPanel.transform.GetChild(1).gameObject.SetActive(false);
        PhoneVerifyPanel.transform.GetChild(0).gameObject.SetActive(true);
        PhoneVerifyPanel.transform.GetChild(0).Find("Input_phone").GetComponent<UIInput>().value = "";
        PhoneVerifyPanel.transform.GetChild(1).Find("Input_Code").GetComponent<UIInput>().value = "";
    }

    public void On_CreateNewPassword()
    {
        PhoneVerifyPanel.SetActive(false);
        PhoneVerifyPanel.transform.GetChild(0).gameObject.SetActive(true);
        PhoneVerifyPanel.transform.GetChild(1).gameObject.SetActive(false);
        PhoneVerifyPanel.transform.GetChild(0).Find("Input_phone").GetComponent<UIInput>().value = "";
        PhoneVerifyPanel.transform.GetChild(1).Find("Input_Code").GetComponent<UIInput>().value = "";
        CreateNewPassPanel.SetActive(true);
    }

    public void Off_CreateNewPassword()
    {
        LoginInputPanel.SetActive(true);        
        CreateNewPassPanel.SetActive(false);
    }

    public void Off_SpecialOffer()
    {
        HomePanel.SetActive(true);
        SpecialOfferPanel.SetActive(false);
    }
    public void On_MessagePanel(string message)
    {
        messagePanel.SetActive(true);
        messagePanel.transform.Find("message").GetComponent<UILabel>().text = message;
        StartCoroutine(MessagePanel(message));
    }

    IEnumerator MessagePanel(string message)
    {        
        yield return new WaitForSeconds(2.0f);
        messagePanel.transform.Find("message").GetComponent<UILabel>().text = "";
        messagePanel.SetActive(false);
    }

    public void Off_Panel(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void Off_MyProfile()
    {
        if(!isOpenProfile)
            On_Home();
        else
            On_Settings();
    }
    
    private void Update()
    {
        if (GameManager.Instance.settingData.music == true)
        {
            GetComponent<AudioSource>().enabled = true;
        }
        else
            GetComponent<AudioSource>().enabled = false;

        if (LoginInputPanel.activeSelf || RegisterInputPanel.activeSelf)
            GameManager.Instance.ForgotPassword = false;
    }
}
