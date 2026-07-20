using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIO;
using System;
public class KYCManager : MonoBehaviour
{
    private SocketIOComponent socket;
    public MenuManager menuManager;
    public UIInput inputDL, inputFirstName, inputLastName, inputBirth;
    public UILabel lbUploadDesc, lbNumberTitle, lbDocsType;
    public GameObject LicenseUI;
    public SaveDataManager savedatamanager;
    void Start()
    {
        socket = SocketManager.Instance.GetSocketIOComponent();

        socket.On("UPLOAD_LIC_PHOTO_RESULT", OnGetImageUploadResult);
        socket.On("REQ_KYC_RESULT", OnGetInsertKYCResult);
    }
    private void OnDestroy()
    {
        socket.Off("UPLOAD_LIC_PHOTO_RESULT", OnGetImageUploadResult);
        socket.Off("REQ_KYC_RESULT", OnGetInsertKYCResult);
    }
    private void OnEnable()
    {
        lbDocsType.text = "Select Document Type";
        lbNumberTitle.text = "DL Number";
        lbUploadDesc.text = "*Please upload front driving license card image.";
    }

    private void OnGetInsertKYCResult(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        if(result == "success")
        {
            menuManager.On_MessagePanel("You proceed your KYC successfully");
            PlayerPrefs.SetString("KYC_VERFIY", "PENDING");
            menuManager.Off_KYCPanel();
        }
        else
        {
            menuManager.On_MessagePanel("You already proceed your KYC.");
            return;
        }
    }

    private void OnGetImageUploadResult(SocketIOEvent evt)
    {
        GameManager.Instance.KYCImageURL = Global.JsonToString(evt.data.GetField("photo_url").ToString(), "\"");
        lbUploadDesc.text = "Your License Image Uploaded Successfully.";
    }

    public void Click_Proceed()
    {
        if (GameManager.Instance.KYCImageURL.Length == 0)
        {
            menuManager.On_MessagePanel("Please Upload the License Image");
            return;
        }
        if(lbDocsType.text == "Select Document Type" || inputDL.value.Length == 0 || inputFirstName.value.Length == 0 || inputLastName.value.Length == 0 || inputBirth.value.Length == 0)
        {
            menuManager.On_MessagePanel("Please Upload the License Image");
            return;
        }

        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("userid", GameManager.Instance.UserID);
        data.Add("document_number", inputDL.value);
        data.Add("first_name", inputFirstName.value);
        data.Add("last_name", inputLastName.value);
        data.Add("dob", inputBirth.value);
        data.Add("document_image", GameManager.Instance.KYCImageURL);
        data.Add("document_type", lbDocsType.text);
        socket.Emit("REQ_KYC_INFO", new JSONObject(data));
    }

    public void On_DocumentType()
    {
        if (LicenseUI.activeSelf)
            LicenseUI.SetActive(false);
        else
            LicenseUI.SetActive(true);
    }
    public void On_Type(GameObject docs)
    {
        LicenseUI.SetActive(false);
        switch (docs.name)
        {
            case "Driving License":
                lbNumberTitle.text = "DL Number";
                lbUploadDesc.text = "*Please upload front driving license card image.";
                lbDocsType.text = "Driving License";
                break;
            case "PanCard":
                lbNumberTitle.text = "PanCard Number";
                lbUploadDesc.text = "*Please upload front PanCard image.";
                lbDocsType.text = "PanCard";
                break;
            case "VoterCard":
                lbNumberTitle.text = "VoterCard Number";
                lbUploadDesc.text = "*Please upload front VoterCard image.";
                lbDocsType.text = "VoterCard";
                break;
            case "AadharCard":
                lbNumberTitle.text = "AadhardCard Number";
                lbUploadDesc.text = "*Please upload front AadharCard image.";
                lbDocsType.text = "AadharCard";
                break;
        }
    }

}
