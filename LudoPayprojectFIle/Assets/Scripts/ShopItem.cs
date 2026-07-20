using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public GameObject BuyBtn;
    [SerializeField]
    private BrowserOpener browserOpener;
    void Start()
    {
        browserOpener = GameObject.Find("BrowserOpener").GetComponent<BrowserOpener>();
        UIEventListener.Get(BuyBtn).onClick += OnButtonClickActive;
    }

    private void OnButtonClickActive(GameObject go)
    {
        if (!PlayStoreCompliance.ExternalPaymentsEnabled)
        {
            Debug.LogWarning("Play Store build: external shop payments disabled.");
            return;
        }

        string amount = go.transform.parent.Find("Title").Find("Value").GetComponent<UILabel>().text;
        string paymentURL = GameManager.Instance.PurchaseLink + 
                            "amount=" + amount + 
                            "&name=" + GameManager.Instance.UserName + 
                            "&email=" + GameManager.Instance.UserEmail + 
                            "&phone=" + GameManager.Instance.UserPhone +
                            "&userid=" + GameManager.Instance.UserID;

        //print("payment URL :  \r\n" + paymentURL);
        browserOpener.OnButtonClicked(paymentURL);
        //Application.OpenURL(paymentURL);
    }
}
