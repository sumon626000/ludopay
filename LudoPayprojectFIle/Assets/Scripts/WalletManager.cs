using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIO;
using System;

public class WalletManager : MonoBehaviour
{
    private SocketIOComponent socket;
    public MenuManager menuManager;
    public  UILabel lbTtbalance, lbwthBalance, lbwthAmount, lbPaymentName, lbDescript;
    public  UIInput inputPaytm, inputGooglePay, inputPhonePe, inputBankID, inputBankName, inputBankCode, InputWthAmount;
    public  GameObject WthUI, PaymentUI, WalletUI, HistoryUI, HomePanel, BankDetils;

    void Start()
    {
        if (!PlayStoreCompliance.RealMoneyFeaturesEnabled)
        {
            enabled = false;
            return;
        }

        socket = SocketManager.Instance.GetSocketIOComponent();
        socket.On("REQ_WITHDRAW_RESULT", OnGetWithdrawResult);        
        lbDescript.text = "* Minimum amount" + GameManager.Instance.MinWithdraw + "required* \r\n You can withdraw within only withdrawable balance";
    }

    private void Update()
    {
        lbTtbalance.text = GameManager.Instance.Points.ToString();
        lbwthBalance.text = GameManager.Instance.Ant.ToString();
    }

    private void OnDestroy()
    {
        socket.Off("REQ_WITHDRAW_RESULT", OnGetWithdrawResult);
    }

    private void OnGetWithdrawResult(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        if (result == "success")
        {
            menuManager.On_MessagePanel("You requested Withdraw Successfully");
            Off_DepositUI();
            WthUI.SetActive(false);
        }
        else
        {
            menuManager.On_MessagePanel("Not enough coins");
        }
    }

    public void On_PaymentUI()
    {
        inputPaytm.gameObject.SetActive(false);
        inputGooglePay.gameObject.SetActive(false);
        inputPhonePe.gameObject.SetActive(false);
        BankDetils.SetActive(false);

        if (PaymentUI.activeSelf)
            PaymentUI.SetActive(false);
        else
            PaymentUI.SetActive(true);
        
    }
    public void On_DepositUI()
    {
        WalletUI.SetActive(true);
        HistoryUI.SetActive(false);
        WthUI.SetActive(false);        
    }
    public void On_WithdrawUI()
    {
        WthUI.SetActive(true);
        HistoryUI.SetActive(false);                
    }
    public void On_HistoryUI()
    {
        HistoryUI.SetActive(true);
        WthUI.SetActive(false);        
    }
    public void On_AddCash()
    {

    }
    public void On_DepositHistory()
    {
        HistoryUI.transform.Find("DepositScroll").gameObject.SetActive(true);
        HistoryUI.transform.Find("WithdrawScroll").gameObject.SetActive(false);
    }
    public void On_WithdrawHistory()
    {
        HistoryUI.transform.Find("DepositScroll").gameObject.SetActive(false);
        HistoryUI.transform.Find("WithdrawScroll").gameObject.SetActive(true);
    }
    public void On_Paytm()
    {
        PaymentUI.SetActive(false);
        lbPaymentName.text = "Paytm";
        inputPaytm.gameObject.SetActive(true);
        inputGooglePay.gameObject.SetActive(false);
        inputPhonePe.gameObject.SetActive(false);
        BankDetils.SetActive(false);
    }
    public void On_GooglePay()
    {
        PaymentUI.SetActive(false);
        lbPaymentName.text = "Google Pay";
        inputPaytm.gameObject.SetActive(false);
        inputGooglePay.gameObject.SetActive(true);
        inputPhonePe.gameObject.SetActive(false);
        BankDetils.SetActive(false);
    }
    public void On_PhonePe()
    {
        PaymentUI.SetActive(false);
        lbPaymentName.text = "Phone Pe";
        inputPaytm.gameObject.SetActive(false);
        inputGooglePay.gameObject.SetActive(false);
        inputPhonePe.gameObject.SetActive(true);
        BankDetils.SetActive(false);
    }
    public void On_Bank()
    {
        PaymentUI.SetActive(false);
        lbPaymentName.text = "Bank";
        inputPaytm.gameObject.SetActive(false);
        inputGooglePay.gameObject.SetActive(false);
        inputPhonePe.gameObject.SetActive(false);
        BankDetils.SetActive(true);
    }
    public void Request_Withdraw()
    {
        if (!PlayStoreCompliance.RealMoneyFeaturesEnabled)
        {
            menuManager.On_MessagePanel(PlayStoreCompliance.BlockedFeatureMessage);
            return;
        }

        string wallet_number = "";
        string bankName = "";
        string bankNum = "";
        string bankCode = "";
        if (lbPaymentName.text == "Select Payment Method")
        {
            menuManager.On_MessagePanel("Please Select Payment Method");
            return;
        }

        if (InputWthAmount.value == "0" || InputWthAmount.value == "")
        {
            menuManager.On_MessagePanel("Not enough coins");
            return;
        }

        if (int.Parse(InputWthAmount.value) > GameManager.Instance.Ant)
        {
            menuManager.On_MessagePanel("Not enough coins");
            return;
        }

        if (int.Parse(InputWthAmount.value) < GameManager.Instance.MinWithdraw)
        {
            menuManager.On_MessagePanel("*Minimum Amount is " + GameManager.Instance.MinWithdraw);
            return;
        }

        switch (lbPaymentName.text)
        {            
            case "Paytm":
                if(inputPaytm.value.Length == 0)
                {
                    menuManager.On_MessagePanel("Please Enter Paytm Number");
                    return;
                }
                wallet_number = inputPaytm.value;
                inputBankID.value = "";
                inputBankName.value = "";
                inputBankCode.value = "";
                break;
            case "Google Pay":
                if (inputGooglePay.value.Length == 0)
                {
                    menuManager.On_MessagePanel("Please Enter Google Pay Number");
                    return;
                }
                wallet_number = inputGooglePay.value;
                inputBankID.value = "";
                inputBankName.value = "";
                inputBankCode.value = "";
                break;
            case "Phone Pe":
                if (inputPhonePe.value.Length == 0)
                {
                    menuManager.On_MessagePanel("Please Enter Phone Pe Number");
                    return;
                }
                wallet_number = inputPhonePe.value;
                inputBankID.value = "";
                inputBankName.value = "";
                inputBankCode.value = "";
                break;
            case "Bank":
                if (inputBankID.value.Length == 0 || inputBankName.value.Length == 0 || inputBankCode.value.Length == 0)
                {
                    menuManager.On_MessagePanel("Please Enter Correct Bank Information");
                    return;
                }
                wallet_number = "";
                bankName = inputBankName.value;
                bankNum = inputBankID.value;
                bankCode = inputBankCode.value;
                break;
        }

        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("userid", GameManager.Instance.UserID);
        data.Add("amount", InputWthAmount.value);
        data.Add("payment_method", lbPaymentName.text);
        data.Add("wallet_number", wallet_number);
        data.Add("bank_name", bankName);
        data.Add("account_number", bankNum);
        data.Add("ifsc_code", bankCode);
        JSONObject jdata = new JSONObject(data);
        socket.Emit("REQ_WITHDRAW", jdata);
    }



    public void Off_Panel(GameObject currentPanel)
    {
        currentPanel.SetActive(false);
    }
    public void Off_DepositUI()
    {
        WalletUI.SetActive(false);
        HomePanel.SetActive(true);
    }
    
}
