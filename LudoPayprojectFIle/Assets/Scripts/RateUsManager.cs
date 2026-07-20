using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RateUsManager : MonoBehaviour
{
    public UILabel Processlabel;
    public Transform Stars;
    public GameObject GoogleRateUs;
    public GameObject thanksforSupport;
    private float processValue;
    private int rateValue;

    private void OnEnable()
    {
        GoogleRateUs.SetActive(false);
        thanksforSupport.SetActive(false);
    }
    private void Update()
    {
        processValue = float.Parse(Processlabel.text);
        if (processValue < 0.16f)
            SetStars(0);
        else if (processValue <= 0.35f)
            SetStars(1);
        else if (processValue <= 0.6f)
            SetStars(2);
        else if (processValue <= 0.85f)
            SetStars(3);
        else if (processValue <= 1.0f)
            SetStars(4);
    }

    void SetStars(int index)
    {
        for (int i = 0; i < index+1; i++)
        {
            Stars.GetChild(i).GetComponent<UISprite>().enabled = true;
        }
        if (index < Stars.childCount-1)
        {
            for (int i = index + 1; i < Stars.childCount; i++)
            {
                Stars.GetChild(i).GetComponent<UISprite>().enabled = false;
            }
        }
        rateValue = index+1;
    }
    public void RateUs()
    {
        if (rateValue >= 4)
        {
            GoogleRateUs.SetActive(true);
        }
        else
            thanksforSupport.SetActive(true);
    }
    public void RateUsGoogle()
    {
        Application.OpenURL("market://details?id=" + "com.ByteGlance.Ludo6");
        
    }
    public void Cancel()
    {
        GoogleRateUs.SetActive(false);
    }
    public void Confirm_Cancel()
    {
        thanksforSupport.SetActive(false);
    }
    public void SendEmail()
    {
        string email = "support@byteglance.com";
        string subject = MyEscapeURL("Ludo6 Feedback");
        string body = MyEscapeURL("Please write below your feedback/issue:\r\n");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }
    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }
}
