using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

public class HomeManager : MonoBehaviour
{
    private MenuManager menuManager;
    private GameObject loading;
    public GameObject WalletPanel;
    public BrowserOpener browserOpener;
    private SocketIOComponent socket;
    public Transform DepositTrans, WthTrans;
    public GameObject DepositItem, WthItem;
    public Transform shopTrans, specailTrans;
    public GameObject shopItem, shopPanel, specailItem;
    public MultiplayerModeSettings mpModeSetting;
    private bool socketHandlersAttached = false;

    private bool SocketReady()
    {
        return socket != null && !socket.disconnected && !string.IsNullOrEmpty(socket.sid);
    }

    private bool EnsureSocket()
    {
        if (socket != null && socketHandlersAttached)
            return socket != null;

        SocketManager sm = SocketManager.Instance;
        if (sm == null)
            sm = FindObjectOfType<SocketManager>();

        if (sm == null)
        {
            Debug.LogError("SocketManager not found in HomeManager.");
            return false;
        }

        socket = sm.GetSocketIOComponent();
        if (socket == null)
            return false;

        if (!socketHandlersAttached)
        {
            socket.On("REQ_WALLET_HIS_RESULT", OnGetTransactionHistory);
            socket.On("REQ_SHOPITEMS_RESULT", OnGetShopItems);
            socket.On("REQ_SPECIAL_RESULT", OnGetSpecialItems);
            socket.On("REQ_CHECK_KYC_RESULT", OnGetCheckKYCResult);
            socket.On("REQ_BIDS_RESULT", OnGetBidsResult);
            socket.On("REQ_COIN_RESULT", OnGetCoinsResult);
            socketHandlersAttached = true;
        }

        return true;
    }

    void Start()
    {
        menuManager = transform.parent.GetComponent<MenuManager>();
        Transform loadingTf = transform.Find("Loading");
        if (loadingTf != null)
            loading = loadingTf.gameObject;

        EnsureSocket();
    }

    private void Awake()
    {
        Invoke("Delay_GetTransaction", 1.5f);
        Invoke("CheckKYC", 1.0f);
        InvokeRepeating("GET_COINS", 0.5f, 5f);
    }

    private void OnDestroy()
    {
        if (socket == null || !socketHandlersAttached)
            return;

        socket.Off("REQ_WALLET_HIS_RESULT", OnGetTransactionHistory);
        socket.Off("REQ_SHOPITEMS_RESULT", OnGetShopItems);
        socket.Off("REQ_SPECIAL_RESULT", OnGetSpecialItems);
        socket.Off("REQ_CHECK_KYC_RESULT", OnGetCheckKYCResult);
        socket.Off("REQ_BIDS_RESULT", OnGetBidsResult);
        socket.Off("REQ_COIN_RESULT", OnGetCoinsResult);
        socketHandlersAttached = false;
    }

    private void GET_COINS()
    {
        if (!EnsureSocket() || !SocketReady() || string.IsNullOrEmpty(GameManager.Instance.UserPhone))
            return;

        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("userphone", GameManager.Instance.UserPhone);
        JSONObject jdata = new JSONObject(data);
        socket.Emit("REQ_GET_COINS", jdata);
    }

    private void OnGetCoinsResult(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");        
        if (result == "success")
        {
            GameManager.Instance.Points = int.Parse(evt.data.GetField("points").ToString());
            GameManager.Instance.Ant = int.Parse(evt.data.GetField("winning_amount").ToString());
        }
    }
        

    List<int> bids = new List<int>();
    private void OnGetBidsResult(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        if (result != "success")
        {
            menuManager.On_MessagePanel("Could not load online stakes.\r\nCheck server is running.");
            return;
        }

        int count = int.Parse(evt.data.GetField("itemlength").ToString());
        bids.Clear();
        for (int i = 0; i < count; i++)
        {
            int bidvalue = int.Parse(Global.JsonToString(evt.data.GetField("bids")[i]["bid_value"].ToString(), "\""));
            bids.Add(bidvalue);
        }

        mpModeSetting.SetBids(bids);

        if (GameManager.Instance._Wifi == WIFI.online)
            menuManager.On_MultiplayerModes();
        else if (GameManager.Instance._Wifi == WIFI.privateRoom)
            menuManager.On_PlayWithFriends();
    }

    private void OnGetCheckKYCResult(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        string status = Global.JsonToString(evt.data.GetField("status").ToString(), "\"");
        if (result == "success")
        {
            switch (status)
            {
                case "0":
                    PlayerPrefs.SetString("KYC_VERFIY", "PENDING");
                    break;
                case "1":
                    PlayerPrefs.SetString("KYC_VERFIY", "SUCCESS");
                    break;
                case "2":
                    PlayerPrefs.SetString("KYC_VERFIY", "REJECTED");
                    break;
            }
        }
    }

    private void CheckKYC()
    {
        if (PlayerPrefs.HasKey("KYC_VERFIY"))
            return;

        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("userid", GameManager.Instance.UserID);
        JSONObject jdata = new JSONObject(data);
        socket.Emit("REQ_CHECK_KYC", jdata);
    }

    private void OnGetSpecialItems(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        int count = int.Parse(evt.data.GetField("itemlength").ToString());
        if (result == "success")
        {   
            if(specailTrans.childCount > 0)
                for (int i = 0; i < specailTrans.childCount; i++)
                    Destroy(specailTrans.GetChild(i).gameObject);

            for (int i = 0; i < count; i++)
            {
                GameObject Temp = Instantiate(specailItem, specailTrans);
                Temp.transform.Find("OfferImg").GetComponent<ImageDownload>().url = Global.JsonToString(evt.data.GetField("offeritems")[i]["offerimage"].ToString(), "\"");
                Temp.transform.Find("Add").Find("Value").GetComponent<UILabel>().text = Global.JsonToString(evt.data.GetField("offeritems")[i]["add_offer_coin"].ToString(), "\"");
                Temp.transform.Find("Receive").Find("Value").GetComponent<UILabel>().text = Global.JsonToString(evt.data.GetField("offeritems")[i]["user_received_coin"].ToString(), "\"");
                Temp.transform.Find("OfferImg").GetComponent<ImageDownload>().enabled = true;
            }

            specailTrans.GetComponent<UIGrid>().enabled = true;
            gameObject.SetActive(false);
            specailTrans.parent.parent.gameObject.SetActive(true);
        }
    }

    private void OnGetShopItems(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        int count = int.Parse(evt.data.GetField("itemlength").ToString());
        if (result == "success")
        {
            if (shopTrans.childCount > 0)
                for (int i = 0; i < shopTrans.childCount; i++)
                    Destroy(shopTrans.GetChild(i).gameObject);

            for (int i = 0; i < count; i++)
            {
                GameObject Temp = Instantiate(shopItem, shopTrans);
                Temp.transform.Find("Title").Find("Value").GetComponent<UILabel>().text = Global.JsonToString(evt.data.GetField("shopitems")[i]["shop_coin"].ToString(), "\"");
            }

            shopTrans.GetComponent<UIGrid>().enabled = true;
            shopPanel.SetActive(true);
        }
        else
        {

        }
    }

    private void OnGetTransactionHistory(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        int depCount = int.Parse(evt.data.GetField("deposit_length").ToString());
        int wthCount = int.Parse(evt.data.GetField("withdraw_length").ToString());
        if (result == "success")
        {
            if(DepositTrans.childCount > 0)
                for (int i = 0; i < DepositTrans.childCount; i++)
                    Destroy(DepositTrans.GetChild(i).gameObject);
            if (WthTrans.childCount > 0)
                for (int i = 0; i < WthTrans.childCount; i++)
                    Destroy(WthTrans.GetChild(i).gameObject);

            for (int i = 0; i < depCount; i++)
            {
                GameObject Temp = Instantiate(DepositItem, DepositTrans);
                Temp.transform.Find("amount").GetComponent<UILabel>().text = evt.data.GetField("deposits")[i]["amount"].ToString();
                Temp.transform.Find("date").GetComponent<UILabel>().text = Global.JsonToString(evt.data.GetField("deposits")[i]["trans_date"].ToString(), "\"");
                Temp.transform.Find("result").GetComponent<UILabel>().text = Global.JsonToString(evt.data.GetField("deposits")[i]["status"].ToString(), "\""); 
            }
            for (int i = 0; i < depCount; i++)
            {
                GameObject Temp = Instantiate(WthItem, WthTrans);
                Temp.transform.Find("amount").GetComponent<UILabel>().text = Global.JsonToString(evt.data.GetField("withdraws")[i]["amount"].ToString(), "\""); 
                Temp.transform.Find("date").GetComponent<UILabel>().text = Global.JsonToString(evt.data.GetField("withdraws")[i]["created_at"].ToString(), "\"");
                string wthResult = Global.JsonToString(evt.data.GetField("withdraws")[i]["withdraw_status"].ToString(), "\"");
                if(wthResult == "0")
                {
                    Temp.transform.Find("result").GetComponent<UILabel>().text = "Pending";
                }
                if (wthResult == "1")
                {
                    Temp.transform.Find("result").GetComponent<UILabel>().text = "Success";
                }
                if (wthResult == "2")
                {
                    Temp.transform.Find("result").GetComponent<UILabel>().text = "Rejected";
                }
            }

            DepositTrans.GetComponent<UIGrid>().enabled = true;
            WthTrans.GetComponent<UIGrid>().enabled = true;
        }
        else
        {

        }

    }

    public void On_SpecialOffer()
    {
        socket.Emit("REQ_SPECIAL_INFO");
    }
    private void Delay_GetTransaction()
    {
        Get_TransactionHistory();
    }

    public void On_ShopMenu()
    {
        socket.Emit("REQ_SHOPITEM_INFO");
        
    }

    private void Get_TransactionHistory()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("userid", GameManager.Instance.UserID);
        JSONObject jdata = new JSONObject(data);
        socket.Emit("REQ_WALLET_HISTORIES", jdata);
    }
    private void OnEnable()
    {
        GameManager.Instance.RoomID = 0;
        GameManager.Instance.PrivateRoomId = string.Empty;
        GameManager.Instance.isCreateRoom = false;
        GameManager.Instance.Online_Bot_Mode = false;
        GameManager.Instance.Users.Clear();
    }
    public void OnClick_OfflineMode()
    {
        GameManager.Instance._Wifi = WIFI.offline;
        menuManager.On_MultiplayerModes();
    }
    public void OnClick_OnlineMode()
    {
        if (!EnsureSocket())
        {
            menuManager.On_MessagePanel("Server connection missing.\r\nRestart the app.");
            return;
        }

        if (!SocketReady())
        {
            menuManager.On_MessagePanel("Server disconnected.\r\nRestart server and try again.");
            return;
        }

        GameManager.Instance._Wifi = WIFI.online;
        socket.Emit("REQ_GAME_BIDS");
    }
    public void OnClick_VsComputerMode()
    {
        GameManager.Instance._Wifi = WIFI.vsComputer;
        menuManager.On_MultiplayerModes();
    }
    public void OnClick_PrivateRoomMode()
    {
        if (!EnsureSocket() || !SocketReady())
        {
            menuManager.On_MessagePanel("Server disconnected.\r\nRestart server and try again.");
            return;
        }

        GameManager.Instance._Wifi = WIFI.privateRoom;
        socket.Emit("REQ_GAME_BIDS");
    }
    public void OnClick_Settings()
    {
        menuManager.On_Settings();
    }
    public void OnClick_Whatsapp()
    {
        //browserOpener.OnButtonClicked(GameManager.Instance.whatsappLink);
        Application.OpenURL(GameManager.Instance.whatsappLink);
    }
    public void OnClick_Youtube()
    {
        //browserOpener.OnButtonClicked(GameManager.Instance.youtubeLink);
        Application.OpenURL(GameManager.Instance.youtubeLink);
    }
    public void OnClick_Wallet()
    {
        if (!PlayStoreCompliance.RealMoneyFeaturesEnabled)
            return;

        gameObject.SetActive(false);
        WalletPanel.SetActive(true);
    }

}
