using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingController : MonoBehaviour
{
    private MenuManager menuManager;
    public GameObject AboutUsPanel;
    public GameObject PrivacyPolicyPanel;
    public GameObject RateusPanel;
    public GameObject LanguagePanel;
    public UIToggle Googlelogin;
    public UIToggle FacebookLogin;
    public ToggleController musicToggle;
    public ToggleController soundToggle;
    public ToggleController vibratToggle;
    public ToggleController notifiToggle;

    void Start()
    {
        menuManager = transform.parent.GetComponent<MenuManager>();
        musicToggle.Set(GameManager.Instance.settingData.music);
        soundToggle.Set(GameManager.Instance.settingData.sound);
        vibratToggle.Set(GameManager.Instance.settingData.vibration);
        notifiToggle.Set(GameManager.Instance.settingData.notification);
    }
    private void OnEnable()
    {
        
    }
    public void OnExit()
    {
        GameManager.Instance.settingData.music = musicToggle.value;
        GameManager.Instance.settingData.sound = soundToggle.value;
        GameManager.Instance.settingData.vibration = vibratToggle.value;
        GameManager.Instance.settingData.notification = notifiToggle.value;
        GameManager.Instance.WriteSettingData();
        GetComponent<UIPopup>().Close();
        Invoke("Return", 0.4f);
    }
    void Return()
    {
        menuManager.On_Home();
    }

    public void OnChangeValue_Slider(UISlider slider)
    {
        GameObject on = slider.transform.Find("On").gameObject;
        if (slider.value == 0)
        {
            on.GetComponent<UISprite>().enabled = true;
        }
        else
        {
            on.GetComponent<UISprite>().enabled = false;
        }
    }
    public void On_Language()
    {
        LanguagePanel.SetActive(true);
        gameObject.SetActive(false);
    }
    public void On_AboutUs()
    {
        AboutUsPanel.SetActive(true);
        gameObject.SetActive(false);
    }
    public void Off_AboutUs()
    {
        AboutUsPanel.GetComponent<UIPopup>().Close();
        Invoke("Return2", 0.4f);
    }
    void Return2()
    {
        gameObject.SetActive(true);
    }
    public void On_PrivacyPolicy()
    {
#if PLAY_STORE_BUILD
        Application.OpenURL(PlayStoreCompliance.PrivacyPolicyUrl);
        return;
#endif
        PrivacyPolicyPanel.SetActive(true);
        gameObject.SetActive(false);
    }
    public void Off_PrivacyPolicy()
    {
        PrivacyPolicyPanel.GetComponent<UIPopup>().Close();
        Invoke("Return2", 0.4f);
    }
    public void On_Rateus()
    {
        RateusPanel.SetActive(true);
        gameObject.SetActive(false);
    }
    public void Off_Rateus()
    {
        RateusPanel.GetComponent<UIPopup>().Close();
        Invoke("Return2", 0.4f);
    }

    public void LogOut()
    {
        PlayerPrefs.DeleteKey("LoginType");
        PlayerPrefs.DeleteKey("KYC_VERFIY");
        GameManager.Instance.GoogleLogined = false;
        GameManager.Instance.isLogin = false;
        gameObject.SetActive(false);
        GameManager.Instance.AvatarURL = "";
        SceneManager.LoadScene("MenuScene");        
    }


    public void Login_out_google()
    {
        if (Googlelogin.value == false)
        {
            
            if (GameManager.Instance._Login_Mode == LOGIN_MODE.facebook)
                menuManager.GetComponent<FirebaseFacebookManager>().Logout();
            SceneManager.LoadScene("MenuScene");
        }
        else 
        {            
            SceneManager.LoadScene("SnsLogin");
        }
    }
    public void Login_out_facebook()
    {
        if (FacebookLogin.value == false) 
        {
            GameManager.Instance.IwannaFacebookLogin = true;
            if (GameManager.Instance._Login_Mode == LOGIN_MODE.google)
                menuManager.GetComponent<FirebaseFacebookManager>().GoogleLogout();
            SceneManager.LoadScene("MenuScene");
        }
        else 
        {
            print("logout");
            menuManager.GetComponent<FirebaseFacebookManager>().Logout();

            GameManager.Instance.UserName = "";
            GameManager.Instance.AvatarImage = Resources.Load("Avatar1") as Texture;
            GameManager.Instance.AvatarURL = "";
            PlayerPrefs.DeleteKey("USERNAME");
            PlayerPrefs.DeleteKey("LOGIN");

            SceneManager.LoadScene("MenuScene");

        }
    }

    public void Share_Refferal()
    {
        string str = "Hi 😃 ! Let's play Ludo Pay game. For every friend sign up with your refer code, you both will get "+ GameManager.Instance.ReferBonus+" coins, Your referral code is "
                    + GameManager.Instance.Referral_code;
        
#if UNITY_ANDROID
        //Reference of AndroidJavaClass class for intent
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        //Reference of AndroidJavaObject class for intent
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        //call setAction method of the Intent object created
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        //set the type of sharing that is happening
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");
        //add data to be passed to the other activity i.e., the data to be sent
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "Referral Code");
        //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), "Text Sharing ");
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), str);
        //get the current activity
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        //start the activity by sending the intent data
        AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share Via");
        currentActivity.Call("startActivity", jChooser);
#endif
    }
}
