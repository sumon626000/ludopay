using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    private SocketIOComponent socket;
    public MenuManager Menumanager;
    public SearchUserManager SManager;
    public MultiplayerModeSettings MMS;
    public GameObject SharePanel;
    public ReferralCodeManager referralManager;
    public PrivateJoinManager PrivateJoin;
    public ContestManager Contestmanager;    
    public UILabel MyPoints;

    private Action<SocketIOEvent> onCreateRoomResult;
    private Action<SocketIOEvent> onCheckRoomsResult;
    private Action<SocketIOEvent> onEnterRoomResult;
    private Action<SocketIOEvent> onUserListResult;
    private Action<SocketIOEvent> onLeaveRoomResult;
    private Action<SocketIOEvent> onUserInfoResult;
    private Action<SocketIOEvent> onUpdateUserInfoResult;
    private Action<SocketIOEvent> onRoomInfoResult;
    private Action<SocketIOEvent> onRefferalResult;
    private Action<SocketIOEvent> onRefferalBounceResult;
    private Action<SocketIOEvent> onJoinRoomResult;

    // Helper method to wrap event handlers with logging
    private Action<SocketIOEvent> WithLogging(string eventKey, Action<SocketIOEvent> handler)
    {
        return (evt) => {
            Debug.Log($"Socket Event: {eventKey}\nData: {evt.data}");
            handler(evt);
        };
    }
    void Start()
    {
        // socket = SocketManager.Instance.GetSocketIOComponent();
        // socket.On("REQ_CREATE_ROOM_RESULT", OnGetCreateRoomResult);
        // socket.On("REQ_CHECK_ROOMS_RESULT", OnGetCheckRoomsResult);
        // socket.On("REQ_ENTER_ROOM_RESULT", OnGetEnterRoomResult);
        // socket.On("REQ_USERLIST_ROOM_RESULT", OnGetUserListResult);
        // socket.On("REQ_LEAVE_ROOM_RESULT", OnGetLeaveRoomResult);
        // socket.On("GET_USERINFO_RESULT", GetUserInfoResult);
        // socket.On("REQ_UPDATE_USERINFO_RESULT", GetUpdateUserInfoResult);
        // socket.On("REQ_ROOM_INFO_RESULT", OnGetRoomInfoResult);
        // //socket.On("REQ_RANK_LIST_RESULT", OnGetRankListResult);        
        // socket.On("REQ_CHECK_REFFERAL_RESULT", OnGetRefferalResult);
        // socket.On("REQ_CHECK_REFFERAL_BOUNCE_RESULT", OnGetRefferalBounceResult);
        // socket.On("REQ_JOIN_ROOM_RESULT", OnGetJoinRoomResult);
        

        socket = SocketManager.Instance.GetSocketIOComponent();

        onCreateRoomResult = WithLogging("REQ_CREATE_ROOM_RESULT", OnGetCreateRoomResult);
        onCheckRoomsResult = WithLogging("REQ_CHECK_ROOMS_RESULT", OnGetCheckRoomsResult);
        onEnterRoomResult = WithLogging("REQ_ENTER_ROOM_RESULT", OnGetEnterRoomResult);
        onUserListResult = WithLogging("REQ_USERLIST_ROOM_RESULT", OnGetUserListResult);
        onLeaveRoomResult = WithLogging("REQ_LEAVE_ROOM_RESULT", OnGetLeaveRoomResult);
        onUserInfoResult = WithLogging("GET_USERINFO_RESULT", GetUserInfoResult);
        onUpdateUserInfoResult = WithLogging("REQ_UPDATE_USERINFO_RESULT", GetUpdateUserInfoResult);
        onRoomInfoResult = WithLogging("REQ_ROOM_INFO_RESULT", OnGetRoomInfoResult);
        onRefferalResult = WithLogging("REQ_CHECK_REFFERAL_RESULT", OnGetRefferalResult);
        onRefferalBounceResult = WithLogging("REQ_CHECK_REFFERAL_BOUNCE_RESULT", OnGetRefferalBounceResult);
        onJoinRoomResult = WithLogging("REQ_JOIN_ROOM_RESULT", OnGetJoinRoomResult);

        socket.On("REQ_CREATE_ROOM_RESULT", onCreateRoomResult);
        socket.On("REQ_CHECK_ROOMS_RESULT", onCheckRoomsResult);
        socket.On("REQ_ENTER_ROOM_RESULT", onEnterRoomResult);
        socket.On("REQ_USERLIST_ROOM_RESULT", onUserListResult);
        socket.On("REQ_LEAVE_ROOM_RESULT", onLeaveRoomResult);
        socket.On("GET_USERINFO_RESULT", onUserInfoResult);
        socket.On("REQ_UPDATE_USERINFO_RESULT", onUpdateUserInfoResult);
        socket.On("REQ_ROOM_INFO_RESULT", onRoomInfoResult);
        socket.On("REQ_CHECK_REFFERAL_RESULT", onRefferalResult);
        socket.On("REQ_CHECK_REFFERAL_BOUNCE_RESULT", onRefferalBounceResult);
        socket.On("REQ_JOIN_ROOM_RESULT", onJoinRoomResult);

        GameManager.Instance.RoomID = 0;
        GameManager.Instance.PrivateRoomId = string.Empty;
        GameManager.Instance.RoomStakeMoney = 0;
        GameManager.Instance.RoomWinMoney = 0;
        GameManager.Instance.isCreateRoom = false;
        GameManager.Instance.Users.Clear();
    }

    private void OnDestroy()
    {
        if (socket == null)
            return;

        if (onCreateRoomResult != null) socket.Off("REQ_CREATE_ROOM_RESULT", onCreateRoomResult);
        if (onCheckRoomsResult != null) socket.Off("REQ_CHECK_ROOMS_RESULT", onCheckRoomsResult);
        if (onEnterRoomResult != null) socket.Off("REQ_ENTER_ROOM_RESULT", onEnterRoomResult);
        if (onUserListResult != null) socket.Off("REQ_USERLIST_ROOM_RESULT", onUserListResult);
        if (onLeaveRoomResult != null) socket.Off("REQ_LEAVE_ROOM_RESULT", onLeaveRoomResult);
        if (onUserInfoResult != null) socket.Off("GET_USERINFO_RESULT", onUserInfoResult);
        if (onUpdateUserInfoResult != null) socket.Off("REQ_UPDATE_USERINFO_RESULT", onUpdateUserInfoResult);
        if (onRoomInfoResult != null) socket.Off("REQ_ROOM_INFO_RESULT", onRoomInfoResult);
        if (onRefferalResult != null) socket.Off("REQ_CHECK_REFFERAL_RESULT", onRefferalResult);
        if (onRefferalBounceResult != null) socket.Off("REQ_CHECK_REFFERAL_BOUNCE_RESULT", onRefferalBounceResult);
        if (onJoinRoomResult != null) socket.Off("REQ_JOIN_ROOM_RESULT", onJoinRoomResult);
    }

    private void OnGetJoinRoomResult(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        if(result == "failed")
        {
            Menumanager.On_MessagePanel("Not enough money!");
        }
    }

    public void CreateRoom()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("userphone", GameManager.Instance.UserPhone);
        data.Add("username", GameManager.Instance.UserName);
        string room_title = "";
        if (GameManager.Instance._Wifi == WIFI.online)
            room_title = GameManager.Instance.UserName + " has set a challenge";
        else if (GameManager.Instance._Wifi == WIFI.privateRoom)
        {
            int myRandomNo = UnityEngine.Random.Range(1000000, 9999999);
            room_title = myRandomNo.ToString();
            GameManager.Instance.PrivateRoomId = room_title;
        }
        data.Add("room_title", room_title);
        data.Add("seat_limit", ((int)GameManager.Instance._GamePlayType).ToString());
        data.Add("status", "ready");
        data.Add("game_mode", GameManager.Instance._GameMode.ToString());
        data.Add("wifi_mode", GameManager.Instance._Wifi.ToString());
        data.Add("stake_money", GameManager.Instance.RoomStakeMoney.ToString());
        data.Add("win_money", GameManager.Instance.RoomWinMoney.ToString());
        JSONObject jdata = new JSONObject(data);
        socket.Emit("REQ_CREATE_ROOM", jdata);
    }

    private void OnGetCreateRoomResult(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        
        if (result == "success")
        {
            int roomID = int.Parse(evt.data.GetField("roomID").ToString());
            GameManager.Instance.RoomID = roomID;
            if (GameManager.Instance._Wifi == WIFI.online)
            {                
                SManager.gameObject.SetActive(true);
                Join_Room();
            }
            else if (GameManager.Instance._Wifi == WIFI.privateRoom)
            {
                GameManager.Instance.isCreateRoom = true;
                GameManager.Instance.PrivateRoomId += roomID.ToString();
                SManager.gameObject.SetActive(true);
                Join_Room();
                //SharePanel.SetActive(true);
            }
        }
        else
        {
            Menumanager.On_MessagePanel("Not enough coins");
            MMS.gameObject.SetActive(true);
        }
    }
    public void Join_Room()
    {
        Debug.Log("Join Room");
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("username", GameManager.Instance.UserName);
        data.Add("userphone", GameManager.Instance.UserPhone);
        data.Add("stake_money", GameManager.Instance.RoomStakeMoney.ToString());
        data.Add("roomID", GameManager.Instance.RoomID.ToString());
        data.Add("photo", GameManager.Instance.AvatarURL.ToString());
        JSONObject jdata = new JSONObject(data);
        socket.Emit("REQ_JOIN_ROOM", jdata);
    }

    private void OnGetEnterRoomResult(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        if (result == "success")
        {
            GameManager.Instance.Points -= GameManager.Instance.RoomStakeMoney;
            SManager.searching = true;
            SManager.gameObject.SetActive(true);
            print("Room Join Arrived ----------------");
            UpdateUserInfo();
        }
        else
        {
            print("You can't enter room because you are in this room");
            Menumanager.On_MessagePanel("You can't enter room because you are in this room");
        }
    }

    List<string> players = new List<string>();
    List<string> playerPhotos = new List<string>();
    private void OnGetUserListResult(SocketIOEvent evt)
    {
        int roomid = int.Parse(Global.JsonToString(evt.data.GetField("roomid").ToString(), "\""));        
        if (roomid == GameManager.Instance.RoomID)
        {
            JSONObject UserArray = evt.data.GetField("userlist");
            if (UserArray == null)
                return;

            players.Clear(); playerPhotos.Clear();
            GameManager.Instance.Users.Clear();
            for (int i = 0; i < UserArray.Count; i++)
            {                
                JSONObject jsonItem = UserArray[i];
                string username = Global.JsonToString(jsonItem.GetField("username").ToString(), "\"");
                string photourl = Global.JsonToString(jsonItem.GetField("photo").ToString(), "\"");
                string userphone = Global.JsonToString(jsonItem.GetField("userphone").ToString(), "\"");
                int points = int.Parse(Global.JsonToString(jsonItem.GetField("points").ToString(), "\""));
                int level = int.Parse(Global.JsonToString(jsonItem.GetField("level").ToString(), "\""));
                if (!GameManager.Instance.UserName.Equals(username))
                {
                    players.Add(username); playerPhotos.Add(photourl);
                }
                UserInfo info = new UserInfo();
                info.name = username;
                info.phone = userphone;
                info.points = points;
                info.level = level;
                GameManager.Instance.Users.Add(info);
            }
            if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    StartCoroutine(match_player(i));
                }
            }
           
        }
    }

    IEnumerator match_player(int i)
    {
        yield return new WaitForSeconds(1.0f);
        SManager.OnFindPlayer(i, players[i], playerPhotos[i]);
    }

    public void LeaveRoom()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["userphone"] = "" + GameManager.Instance.UserPhone;
        data["roomid"] = "" + GameManager.Instance.RoomID;
        data["username"] = GameManager.Instance.UserName;
        socket.Emit("REQ_LEAVE_ROOM", new JSONObject(data));
    }
    private void OnGetLeaveRoomResult(SocketIOEvent evt)
    {
        string username = Global.JsonToString(evt.data.GetField("username").ToString(), "\"");
        string message = Global.JsonToString(evt.data.GetField("message").ToString(), "\"");
    
        if (players.Contains(username))
        {
            int index = players.IndexOf(username);
            players.RemoveAt(index); playerPhotos.RemoveAt(index);
            if(SManager.gameObject.activeSelf)
                SManager.OnLeavePlayer(index);
        }
    }
    public void RequestRoomInfo()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("roomID", GameManager.Instance.RoomID.ToString());
        JSONObject jdata = new JSONObject(data);
     
        socket.Emit("REQ_ROOM_INFO", jdata);
    }
    private void OnGetRoomInfoResult(SocketIOEvent evt)
    {
        int playercount = int.Parse(evt.data.GetField("seatlimit").ToString());
        if (playercount == 0)
            return;
        string gamemode = Global.JsonToString(evt.data.GetField("gamemode").ToString(), "\"");
        if (gamemode == "classic")
            GameManager.Instance._GameMode = GameMode.classic;
        else if (gamemode == "arrow") {
            GameManager.Instance._GameMode = GameMode.arrow;
        }
        else if(gamemode == "quick")
            GameManager.Instance._GameMode = GameMode.quick;
        GameManager.Instance.RoomStakeMoney = int.Parse(Global.JsonToString(evt.data.GetField("stakemoney").ToString(), "\""));
        GameManager.Instance.RoomWinMoney = int.Parse(Global.JsonToString(evt.data.GetField("winmoney").ToString(), "\""));
        GamePlayType count = (GamePlayType)playercount;
        GameManager.Instance._GamePlayType = count;
        PrivateJoin.joinStart();
    }
    public void Request_RankList(string type)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("rank_type", type);
        JSONObject jdata = new JSONObject(data);
        socket.Emit("REQ_RANK_LIST", jdata);
    }

    //private void OnGetRankListResult(SocketIOEvent evt)
    //{
    //    string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
    //    if (result.Equals("success"))
    //    {
    //        JSONObject UserArray = evt.data.GetField("users");
    //        if (UserArray == null)
    //            return;
    //        Contestmanager.InitUI(UserArray);
    //    }
    //}
    
    public void AddGameHistory(int point)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("username", GameManager.Instance.UserName);
        data.Add("points", point.ToString());
        JSONObject jdata = new JSONObject(data);
        socket.Emit("REQ_GAME_HIST", jdata);
    }
    
    

    #region GET USERINFO
    public void GetUserInfo(string username)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("username", username);
        socket.Emit("REQ_USER_INFO", new JSONObject(data));
    }

    private void GetUserInfoResult(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        if (result.Equals("success"))
        {
            string name = Global.JsonToString(evt.data.GetField("username").ToString(), "\"");
            string photoURL = Global.JsonToString(evt.data.GetField("photo").ToString(), "\"");
            int points = int.Parse(Global.JsonToString(evt.data.GetField("points").ToString(), "\""));
            int level = int.Parse(Global.JsonToString(evt.data.GetField("level").ToString(), "\""));	
            int referralCount = int.Parse(Global.JsonToString(evt.data.GetField("referral_count").ToString(), "\""));
        }
    }
    #endregion

    #region UPDATE USER PROFILE
    public void UpdateUserInfo()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("username", GameManager.Instance.UserName);
        data.Add("userphone", GameManager.Instance.UserPhone);
        data.Add("points", GameManager.Instance.Points.ToString());
        data.Add("winning_amount", GameManager.Instance.Ant.ToString());
        data.Add("level", GameManager.Instance.Level.ToString());
        data.Add("online_played", GameManager.Instance.Online_Multiplayer.played.ToString());
        data.Add("online_won", GameManager.Instance.Online_Multiplayer.won.ToString());
        data.Add("friend_played", GameManager.Instance.Friend_Multiplayer.played.ToString());
        data.Add("friend_won", GameManager.Instance.Friend_Multiplayer.won.ToString());
        data.Add("tokenscaptured_mine", GameManager.Instance.TokensCaptued.mine.ToString());
        data.Add("tokenscaptured_opponents", GameManager.Instance.TokensCaptued.opponents.ToString());
        data.Add("wonstreaks_current", GameManager.Instance.WonStreaks.current.ToString());
        data.Add("wonstreaks_best", GameManager.Instance.WonStreaks.best.ToString());
        JSONObject jdata = new JSONObject(data);
        socket.Emit("REQ_UPDATE_USERINFO", jdata);
    }
    private void GetUpdateUserInfoResult(SocketIOEvent evt)
    {
        
    }
    #endregion

    #region Match Way

    public void Check_Match_Way()
    {
        int val = (int)(GameManager.Instance._GamePlayType);
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("seat_limit", val.ToString());
        data.Add("game_mode", GameManager.Instance._GameMode.ToString());
        data.Add("wifi_mode", GameManager.Instance._Wifi.ToString());
        data.Add("stake_money", GameManager.Instance.RoomStakeMoney.ToString());
        data.Add("win_money", GameManager.Instance.RoomWinMoney.ToString());
        JSONObject jdata = new JSONObject(data);
        socket.Emit("REQ_CHECK_ROOMS", jdata);
    }
    private void OnGetCheckRoomsResult(SocketIOEvent evt)
    {
        try
        {
            Debug.Log("OnGetCheckRoomsResult: " + evt.data);
            string result = evt.data.GetField("result").str;
            if (result == "success")
            {
                var roomIDField = evt.data.GetField("roomID");
                if (roomIDField == null || !int.TryParse(roomIDField.n.ToString(), out int roomID))
                {
                    Debug.LogError("Failed to parse roomID: " + roomIDField?.ToString());
                    return;
                }


                GameManager.Instance.RoomID = roomID;
                if (GameManager.Instance._GamePlayType == GamePlayType.four)
                    SManager.gameObject.SetActive(true);
                Join_Room();
            }
            else
            {
                CreateRoom();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception in OnGetCheckRoomsResult: " + ex);
        }
    }

    #endregion

    #region Check referral code
    public void Check_Referral_Code(string code)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("userphone", GameManager.Instance.UserPhone);
        data.Add("referral", code);
        JSONObject jdata = new JSONObject(data);
        socket.Emit("REQ_CHECK_REFFERAL", jdata);
    }
    private void OnGetRefferalResult(SocketIOEvent evt)
    {
        print(evt.name + " " + evt.data);
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        referralManager.SetResult(result);        
    }
    public void Check_Referral_Bounce()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("username", GameManager.Instance.UserName);
        JSONObject jdata = new JSONObject(data);
        socket.Emit("REQ_CHECK_REFFERAL_BOUNCE", jdata);
    }
    private void OnGetRefferalBounceResult(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");

        if (result == "success")
        {
			int add_points = int.Parse(evt.data.GetField("bounce").ToString());
			GameManager.Instance.Referral_count = int.Parse(evt.data.GetField("referCount").ToString());
			
            if (add_points > 0)
            {
                GameManager.Instance.Points += add_points;
                Menumanager.ProfilePanel.GetComponent<ProfileManager>().Counting(GameManager.Instance.Points, GameManager.Instance.Points - add_points);
            }
        }
    }
    #endregion
}
