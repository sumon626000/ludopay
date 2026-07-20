#if GUEST_BUILD
using System.Collections;
using UnityEngine;

public class FirebaseFacebookManager : MonoBehaviour
{
    public LoginManager loginmanager;
    public string webClientId = "<your client id here>";
    public UILabel MyText;
    public GameObject phoneloginPanel;
    public GameObject phonenumberPanel;
    public GameObject verificodePanel;
    public UILabel Seconds;

    private MenuManager menuManager;
    private string userphone;

    void EnsureMenuManager()
    {
        if (menuManager == null)
            menuManager = GetComponent<MenuManager>();
    }

    public void CheckFBStatus() { }
    public void FacebooklogIn() { Debug.LogWarning("Facebook login disabled in guest build."); }
    public void Logout()
    {
        GameManager.Instance.IwannaFacebookLogin = false;
        GameManager.Instance.UserName = "";
        PlayerPrefs.DeleteKey("USERNAME");
        PlayerPrefs.DeleteKey("LOGIN");
    }
    public void GoogleLogout()
    {
        GameManager.Instance.UserName = "";
        GameManager.Instance._Login_Mode = LOGIN_MODE.guest;
        GameManager.Instance.AvatarImage = Resources.Load("Avatar1") as Texture;
        GameManager.Instance.AvatarURL = "";
        PlayerPrefs.DeleteKey("USERNAME");
        PlayerPrefs.DeleteKey("LOGIN");
    }

    public void OpenPhoneLogin()
    {
        EnsureMenuManager();
        phoneloginPanel.SetActive(true);
        phonenumberPanel.SetActive(true);
        phonenumberPanel.transform.Find("Input_phone").GetComponent<UIInput>().value = "";
        verificodePanel.SetActive(false);
        verificodePanel.transform.Find("Input_Code").GetComponent<UIInput>().value = "";
    }

    public void SubmitPhone(string phoneNumber)
    {
        EnsureMenuManager();
        if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length < 10)
        {
            menuManager.On_MessagePanel("Mobile Number is not correct");
            return;
        }

        userphone = phoneNumber;
        GameManager.Instance.UserPhone = phoneNumber;
        GameManager.Instance.UserName = "";
        GameManager.Instance.UserEmail = "";
        GameManager.Instance.UserPassword = "";

        if (menuManager.PhoneVerifyPanel.activeSelf)
            menuManager.Off_PhoneVerifyPanel();

        phoneloginPanel.SetActive(false);
        verificodePanel.SetActive(false);
        loginmanager.Valid_Account();
    }

    public void GetOTP(string phoneNumber) { SubmitPhone(phoneNumber); }

    public void VerficationEnter(string verificationCode)
    {
        EnsureMenuManager();
        if (!string.IsNullOrEmpty(userphone))
            SubmitPhone(userphone);
        else
            menuManager.On_MessagePanel("Please enter your mobile number first.");
    }

    public void CloseVerifyWindow()
    {
        verificodePanel.SetActive(false);
        phonenumberPanel.SetActive(true);
    }

    public void ClosePhonenumberWindow()
    {
        phoneloginPanel.SetActive(false);
        phonenumberPanel.SetActive(true);
        verificodePanel.SetActive(false);
    }
}
#else
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Firebase.Auth;
using UnityEngine.UI;
using System;
using System.IO;
using Google;
using SimpleJSON;
using UnityEngine.Networking;

public class FirebaseFacebookManager : MonoBehaviour
{
    public LoginManager loginmanager;
    private MenuManager menuManager;
    private FirebaseAuth auth;
    PhoneAuthProvider provider;
    private bool isLoginSuccess = false;
    private string username, userphone;

    public string webClientId = "<your client id here>";
   // private GoogleSignInConfiguration configuration;

    void EnsureMenuManager()
    {
        if (menuManager == null)
            menuManager = GetComponent<MenuManager>();
    }

    void EnsureFacebookInitialized()
    {
        if (!FB.IsInitialized)
            FB.Init();
        else
            FB.ActivateApp();
    }

    FirebaseAuth GetAuth()
    {
        if (auth == null)
        {
            try { auth = FirebaseAuth.DefaultInstance; }
            catch (Exception ex) { Debug.LogWarning("Firebase Auth unavailable: " + ex.Message); }
        }
        return auth;
    }

    void EnsurePhoneAuthProvider()
    {
        var firebaseAuth = GetAuth();
        if (firebaseAuth == null || provider != null)
            return;

        try
        {
            provider = PhoneAuthProvider.GetInstance(firebaseAuth);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("PhoneAuthProvider init failed: " + ex.Message);
        }
    }

    public void CheckFBStatus()
    {
        if (FB.IsInitialized)
            FB.LogOut();
    }

    public void FacebooklogIn()
    {
        if (!enabled)
            enabled = true;

        EnsureMenuManager();
        EnsureFacebookInitialized();
        CheckFBStatus();
        loginmanager.loading.SetActive(true);
        FB.LogInWithReadPermissions(callback: OnLogIn);
    }

    private void OnLogIn(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            AccessToken tocken = result.AccessToken;
            GetAllFacebookInfos();
        }
        else
        {           
            loginmanager.loading.SetActive(false);
        }
    }

    private void GetAllFacebookInfos()
    {
        FB.API("/me?fields=first_name", HttpMethod.GET, GetUserName);
        FB.API("/me?fields=email", HttpMethod.GET, GetUserEmail);
        FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, GetUserPic);
    }
    private void GetUserName(IResult result)
    {
        if (result.Error == null)
        {
            string name = "" + result.ResultDictionary["first_name"];
            GameManager.Instance.UserName = name;
            Debug.Log("--- Facebook UserName --- :  " + name);
        }
        else
        {
            Debug.Log(result.Error);
        }
    }
    private void GetUserEmail(IResult result)
    {
        if (result.Error == null)
        {
            if (result.ResultDictionary.ContainsKey("email") == false)
            {
                GameManager.Instance.UserEmail = "";
            }
            else
            {
                GameManager.Instance.UserEmail = result.ResultDictionary["email"].ToString();
            }
        }
        else
        {
            Debug.Log(result.Error);
        }
        Debug.Log("--- Facebook UserEmail --- " + GameManager.Instance.UserEmail);
    }
    private void GetUserPic(IGraphResult result)
    {
        if (result.Texture != null)
        {
            GameManager.Instance.AvatarURL = "http" + "://graph.facebook.com/" + AccessToken.CurrentAccessToken.UserId + "/picture?type=square&height=128&width=128";
            Debug.Log("--- Facebook AvatarURL --- \r\n " + GameManager.Instance.AvatarURL);

            loginmanager.loading.SetActive(false);
            loginmanager.Register();
        }
        else
        {
            Debug.Log(result.Error);
        }
    }

    public void Logout()
    {
        print("FB Logout");
        GameManager.Instance.IwannaFacebookLogin = false;
        GameManager.Instance.UserName = "";

        PlayerPrefs.DeleteKey("USERNAME");
        PlayerPrefs.DeleteKey("LOGIN");
        if (FB.IsInitialized)
            FB.LogOut();
     
    }
    public void GoogleLogout()
    {
        Debug.Log("signout");

    //    GoogleSignIn.DefaultInstance.SignOut();

        GameManager.Instance.UserName = "";
        GameManager.Instance._Login_Mode = LOGIN_MODE.guest;
        GameManager.Instance.AvatarImage = Resources.Load("Avatar1") as Texture;
        GameManager.Instance.AvatarURL = "";
        PlayerPrefs.DeleteKey("USERNAME");
        PlayerPrefs.DeleteKey("LOGIN");
    }

    public UILabel MyText;
    string verificationId = "";

    public void OpenPhoneLogin()
    {
        EnsureMenuManager();
        phoneloginPanel.SetActive(true);
        phonenumberPanel.SetActive(true);
        phonenumberPanel.transform.Find("Input_phone").GetComponent<UIInput>().value = "";
        verificodePanel.SetActive(false);
        verificodePanel.transform.Find("Input_Code").GetComponent<UIInput>().value = "";
        verificationId = "";
    }

    // OTP removed — phone only, then login/register on server
    public void SubmitPhone(string phoneNumber)
    {
        EnsureMenuManager();
        if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length < 10)
        {
            menuManager.On_MessagePanel("Mobile Number is not correct");
            return;
        }

        userphone = phoneNumber;
        GameManager.Instance.UserPhone = phoneNumber;
        GameManager.Instance.UserName = "";
        GameManager.Instance.UserEmail = "";
        GameManager.Instance.UserPassword = "";

        if (menuManager.PhoneVerifyPanel.activeSelf)
            menuManager.Off_PhoneVerifyPanel();

        phoneloginPanel.SetActive(false);
        verificodePanel.SetActive(false);

        loginmanager.Valid_Account();
    }

    // Unity buttons still call these names
    public void GetOTP(string phoneNumber)
    {
        SubmitPhone(phoneNumber);
    }

    public void VerficationEnter(string verificationCode)
    {
        EnsureMenuManager();
        if (!string.IsNullOrEmpty(userphone))
            SubmitPhone(userphone);
        else
            menuManager.On_MessagePanel("Please enter your mobile number first.");
    }

    public GameObject phoneloginPanel;
    public GameObject phonenumberPanel;
    public GameObject verificodePanel;
    public UILabel Seconds;

    IEnumerator TimeoutCount()
    {
        Seconds.text = "60s";
        int i = 60;
        while (i>0)
        {
            i--;
            Seconds.text = i.ToString() + "s";
            yield return new WaitForSeconds(1.0f);
        }
    }
    public void CloseVerifyWindow()
    {
        verificodePanel.SetActive(false);
        phonenumberPanel.SetActive(true);
    }
    public void ClosePhonenumberWindow()
    {
        phoneloginPanel.SetActive(false);
        phonenumberPanel.SetActive(true);
        verificodePanel.SetActive(false);
    }

}
#endif
