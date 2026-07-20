using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdsScript : MonoBehaviour
{
    public int rewardCoins = 300;
    //public UILabel coinsText;
    public GameObject intersttialButton;
    public GameObject rewardedButton;
    public ProfileManager profileManage;
    public RoomManager roommanager;
    public GameObject alert;


    void Awake()
    {
       // Advertisements.Instance.Initialize();
    }
        
 
    public void ShowInterstitial()
    {
      // Advertisements.Instance.ShowInterstitial(InterstitialClosed);
    }

 
    public void ShowRewardedVideo()
    {
       //// Advertisements.Instance.ShowRewardedVideo(CompleteMethod);
      //  if (!Advertisements.Instance.IsRewardVideoAvailable())
      //  {
            alert.GetComponent<TweenAlpha>().from = 0;
            alert.GetComponent<TweenAlpha>().to = 1;
            alert.GetComponent<TweenAlpha>().ResetToBeginning();
            alert.GetComponent<TweenAlpha>().PlayForward();
            StartCoroutine("Crou");
      // }
    }
    IEnumerator Crou()
    {
        yield return new WaitForSeconds(4.0f);
        alert.GetComponent<TweenAlpha>().from = 1;
        alert.GetComponent<TweenAlpha>().to = 0;
        alert.GetComponent<TweenAlpha>().ResetToBeginning();
        alert.GetComponent<TweenAlpha>().PlayForward();
    }


    void Update()
    {
        if (intersttialButton != null || rewardedButton != null)
        {
           //if (Advertisements.Instance.IsInterstitialAvailable())
            //{
                intersttialButton.SetActive(true);
           // }
         //   else
         //   {
                intersttialButton.SetActive(false);
          //  }

           // if (Advertisements.Instance.IsRewardVideoAvailable())
          //  {
                rewardedButton.GetComponent<UISprite>().alpha = 1.0f;
           // /}
            //else
           // {
                rewardedButton.GetComponent<UISprite>().alpha = 0.65f;
          //  }
        }
    }

    private void CompleteMethod(bool completed)
    {
        if (completed)
        {
            GameManager.Instance.Points += rewardCoins;
        }
        
        roommanager.UpdateUserInfo();
        profileManage.Counting(GameManager.Instance.Points, GameManager.Instance.Points - rewardCoins);
    }
    
    private void InterstitialClosed(string advertiser)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }
}
