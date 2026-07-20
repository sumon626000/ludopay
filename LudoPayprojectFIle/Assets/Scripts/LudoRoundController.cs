using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

public class LudoRoundController : MonoBehaviour
{
    public GameObject Loading;
    public UILabel MyPoints;
    public UILabel WinMoneyLabel;
    public AdsScript adsmanager;
    public int TurnId = -1;
    public int MoveSteps = 0;
    public string _roller = "";
    public GameMode gameMode = GameMode.arrow;
    public GameObject WinmoneyUI, BidsUI;
    [Space(10.0f)]
    public List<LudoPlayerController> PlayerControllers = new List<LudoPlayerController>();
    public List<UserDetail> UserDetails = new List<UserDetail>();
    [Header("Effect")]
    public GameObject CoinEffect;
    public GameObject Firework;
    public AudioClip token_start;
    public AudioClip token_walk;
    public AudioClip token_cut;
    public AudioClip token_finish;
    public AudioClip star_path;
    public AudioClip token_quick_move;
    public AudioClip token_arrow;

    [Space(10.0f)]
    public GameObject Line4p;
    public GameObject Line6p;
    public GameObject Board4p;
    public GameObject Board6p;
    public GameObject Pawns4p;
    public GameObject Yards4P;
    public GameObject Pawns6p;
    public GameObject Yards6P;
    public GameObject Paths4p;
    public GameObject Paths6p;
    public GameObject DiceController;
    public GameObject[] players4p;
    public GameObject[] players6p;
    [Space(10.0f)]
    public GameObject GameEndPopup;
    public List<int> Ranking = new List<int>();
    public Transform RankGrid;
    public GameObject RankItem;

    private int playerCount;
    private List<Vector3> winnerMarkPos = new List<Vector3>();
    private SocketIOComponent socket;
    private string socketID;
    UserInfo firstUser = new UserInfo();
    int firstTurnId = 0;
    public string turnuser = "";
    public int diceNumber = 1;
    public GameObject DisconnectedPanel;
    IEnumerator CoroutineDisconnectTime;
    IEnumerator CoroutineBgrTime;

    void Awake()
    {
        if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
        {            
            Loading.SetActive(true);
        }

        if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom || GameManager.Instance.Online_Bot_Mode)
        {
            WinmoneyUI.SetActive(true);
            BidsUI.SetActive(true);
            WinmoneyUI.transform.Find("Winmoney").GetComponent<UILabel>().text = GameManager.Instance.RoomWinMoney.ToString();
            BidsUI.transform.Find("Bids").GetComponent<UILabel>().text = GameManager.Instance.RoomStakeMoney.ToString();
        }
        else
        {
            WinmoneyUI.SetActive(false);
            BidsUI.SetActive(false);
        }
            
        MyPoints.text = GameManager.Instance.Points.ToString("N0");
        
        playerCount = (int)GameManager.Instance._GamePlayType;
    
        if (playerCount <= 4)
        {
            Line4p.SetActive(true);
            Board4p.SetActive(true);
            Board4p.GetComponent<UISprite>().spriteName = string.Format("board4p_{0}", GameManager.Instance._PlayerType);
            Pawns4p.SetActive(true);
            Paths4p.SetActive(true);
            for (int i = 0; i < 4; i++)
            {
                PlayerType pt = (PlayerType)(GameManager.Instance.playerColors[i]);
                string str = pt.ToString();
                for (int j = 0; j < 4; j++)
                {
                    Pawns4p.transform.GetChild(i).GetChild(j).GetComponent<UISprite>().spriteName = string.Format("pawn_{0}", str);
                    Yards4P.transform.GetChild(i).GetChild(j).GetComponent<UISprite>().spriteName = string.Format("pawnBg_{0}", str);
                }
            }
            List<int> positions = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                positions.Add(i);
            }
            if (playerCount == 2)
            {
                positions.Clear();
                positions.Add(0); positions.Add(2);
                players4p[1].gameObject.SetActive(false);
                players4p[3].gameObject.SetActive(false);
            }
            else if (playerCount == 3)
            {
                players4p[3].gameObject.SetActive(false);
            }
            for (int i = 0; i < positions.Count; i++)
            {
                PlayerControllers.Add(players4p[positions[i]].GetComponent<LudoPlayerController>());
                UserDetails.Add(new UserDetail());
            }
            for (int i = 0; i < PlayerControllers.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    PlayerControllers[i].Pawns.GetChild(j).GetComponent<LudoPawnController>().turnId = i;
                }
            }
            if (playerCount == 2)
            {
                PlayerType pt = (PlayerType)(GameManager.Instance.playerColors[0]);
                string str = pt.ToString();
                players4p[0].SetActive(true);
                players4p[0].transform.Find("Timer/Timer").GetComponent<UISprite>().spriteName = string.Format("timer_circle_{0}", str);
                players4p[0].transform.Find("FramePanel/Image").GetComponent<UISprite>().spriteName = string.Format("TurnMark_{0}", str);
                PlayerType pt1 = (PlayerType)(GameManager.Instance.playerColors[2]);
                string str1 = pt1.ToString();
                players4p[2].SetActive(true);
                players4p[2].transform.Find("Timer/Timer").GetComponent<UISprite>().spriteName = string.Format("timer_circle_{0}", str1);
                players4p[2].transform.Find("FramePanel/Image").GetComponent<UISprite>().spriteName = string.Format("TurnMark_{0}", str1);
            }
            else
            {
                for (int i = 0; i < positions.Count; i++)
                {
                    if (playerCount > i)
                    {
                        PlayerType pt = (PlayerType)(GameManager.Instance.playerColors[i]);
                        string str = pt.ToString();
                        players4p[positions[i]].SetActive(true);
                        players4p[positions[i]].transform.Find("Timer/Timer").GetComponent<UISprite>().spriteName = string.Format("timer_circle_{0}", str);
                        players4p[positions[i]].transform.Find("FramePanel/Image").GetComponent<UISprite>().spriteName = string.Format("TurnMark_{0}", str);
                    }


                }
            }
            players4p[0].transform.Find("Nickname").GetComponent<UILabel>().text = GameManager.Instance.UserName;
           
            if (GameManager.Instance._Wifi == WIFI.offline)
            {
                for (int i = 0; i < players4p.Length; i++)
                {
                    players4p[i].GetComponent<LudoPlayerController>().DiceController.Active_DiceInit_Click();
                }
            }
            else
            {
                players4p[0].GetComponent<LudoPlayerController>().DiceController.Active_DiceInit_Click();
                for (int i = 1; i < players4p.Length; i++)
                {
                    players4p[i].GetComponent<LudoPlayerController>().DiceController.NoneActive_DiceInit_Click();
                    if (GameManager.Instance._Wifi == WIFI.vsComputer)
                        players4p[i].GetComponent<LudoPlayerController>().isAUTO = true;
                }
            }
        }
        else
        {
            Line6p.SetActive(true);
            Board6p.SetActive(true);
            Board6p.GetComponent<UISprite>().spriteName = string.Format("board6p_{0}", GameManager.Instance._PlayerType);
            Pawns6p.SetActive(true);
            Paths6p.SetActive(true);
            DiceController.SetActive(true);

            if (playerCount == 5)
            {
                players6p[5].gameObject.SetActive(false);
            }
            for (int i = 0; i < 6; i++)
            {
                PlayerType pt = (PlayerType)(GameManager.Instance.playerColors[i]);
                string str = pt.ToString();
                for (int j = 0; j < 4; j++)
                {
                    Pawns6p.transform.GetChild(i).GetChild(j).GetComponent<UISprite>().spriteName = string.Format("pawn_{0}", str);
                    Yards6P.transform.GetChild(i).GetChild(j).GetComponent<UISprite>().spriteName = string.Format("pawnBg_{0}", str);
                }
            }
            for (int i = 0; i < players6p.Length; i++)
            {
                if (playerCount > i)
                {
                    PlayerType pt = (PlayerType)(GameManager.Instance.playerColors[i]);
                    string str = pt.ToString(); print(str);
                    players6p[i].SetActive(true);
                    players6p[i].transform.Find("Timer/Timer").GetComponent<UISprite>().spriteName = string.Format("timer_circle_{0}", str);
                    players6p[i].transform.Find("FramePanel/Image").GetComponent<UISprite>().spriteName = string.Format("TurnMark_{0}", str);
                }
                PlayerControllers.Add(players6p[i].GetComponent<LudoPlayerController>());
                UserDetails.Add(new UserDetail());
            }
        }
        switch (GameManager.Instance._GameMode)
        {
            case GameMode.classic:
                break;
            case GameMode.quick:
                for (int i = 0; i < PlayerControllers.Count; i++)
                {
                    PlayerControllers[i].transform.Find("QuickMode").gameObject.SetActive(true);
                }
                break;
            case GameMode.arrow:
                if (playerCount <= 4)
                    Board4p.transform.GetChild(0).gameObject.SetActive(true);
                else
                    Board6p.transform.GetChild(0).gameObject.SetActive(true);
                break;
            default:
                break;
        }
        if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
        {
            firstUser = GameManager.Instance.Users[0];
            ReArray(); 
           
            firstTurnId = GameManager.Instance.Users.IndexOf(firstUser);
            _roller = GameManager.Instance.Users[0].phone;
        }
        else
        {
            if(!GameManager.Instance.Online_Bot_Mode)
                GameManager.Instance.RoomWinMoney = 0;
        }
        WinMoneyLabel.text = GameManager.Instance.RoomWinMoney.ToString("N0");
        if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
        {
            for (int i = 0; i < playerCount; i++)
            {
                PlayerControllers[i].NickNameLabel.text =
                    UserDetails[i].username =
                     GameManager.Instance.Users[i].name;

                UserDetails[i].userphone = GameManager.Instance.Users[i].phone;
            }
        }
        else
        {
            for (int i = 0; i < PlayerControllers.Count; i++)
            {
                PlayerControllers[i].NickNameLabel.text =
                UserDetails[i].username =
                GameManager.Instance.playerNames[i];                
            }
        }
        if (GameManager.Instance._Wifi == WIFI.offline)
        {
            for (int i = 0; i < players6p.Length; i++)
            {
                players6p[i].GetComponent<LudoPlayerController>().DiceController.Active_DiceInit_Click();
            }
        }
        else
        {
            players6p[0].GetComponent<LudoPlayerController>().DiceController.Active_DiceInit_Click();
            for (int i = 1; i < players6p.Length; i++)
            {
                players6p[i].GetComponent<LudoPlayerController>().DiceController.NoneActive_DiceInit_Click();
                if (GameManager.Instance._Wifi == WIFI.vsComputer)
                    players6p[i].GetComponent<LudoPlayerController>().isAUTO = true;
            }
        }
    }
    
    List<UserInfo> buffUsers = new List<UserInfo>();
    public void ReArray()
    {
        int myIndex = 0;
        for (int i = 0; i < GameManager.Instance.Users.Count; i++)
        {
            buffUsers.Add(GameManager.Instance.Users[i]);
            if (GameManager.Instance.Users[i].phone == GameManager.Instance.UserPhone)
            {
                myIndex = i;
            }
        }
        GameManager.Instance.Users.Clear();
        for (int i = 0; i < buffUsers.Count; i++)
        {
            GameManager.Instance.Users.Add(buffUsers[myIndex]);
            if (myIndex < buffUsers.Count - 1)
                myIndex++;
            else
                myIndex = 0;
        }
    }

    private void Start()
    {
        socket = SocketManager.Instance.GetSocketIOComponent();
        socketID = socket.sid;       
        socket.On("RECONNECT_RESULT", GetReconnect); //this is reconnect callback
        socket.On("REQ_TURNUSER_RESULT", OnGetTurnUser);
        socket.On("REQ_ROLL_DICE_RESULT", OnGetRollDiceResult);
        socket.On("REQ_MOVE_TOKEN_RESULT", OnGetMoveTokenResult);
        socket.On("REQ_LEAVE_ROOM_RESULT", OnGetLeaveRoomResult);
        socket.On("REQ_AUTO_RESULT", OnGetAUTOResult);
        socket.On("GAME_END", OnGetEndGameResult);
        socket.On("GET_USERINFO_RESULT", GetUserInfoResult);
        socket.On("REQ_UPDATE_USERINFO_RESULT", GetUpdateUserInfoResult);
        socket.On("REQ_PAUSE_RESULT", GetPausedResult);
        socket.On("REQ_RESUME_RESULT", GetResumedResult);
        socket.On("EXIT_GAME", OnGetExitGame);

        if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom || GameManager.Instance.Online_Bot_Mode)
        {
            GameManager.Instance.Online_Multiplayer.played++;

            for (int i = 0; i < UserDetails.Count; i++)
            {
                GetUserInfo(UserDetails[i].userphone);
            }
        }
        else
        {
            if (GameManager.Instance.AvatarURL == "")
            {
                if (playerCount <= 4)
                    players4p[0].GetComponent<LudoPlayerController>().Photo.mainTexture = GameManager.Instance.AvatarImage;
                else
                    players6p[0].GetComponent<LudoPlayerController>().Photo.mainTexture = GameManager.Instance.AvatarImage;
            }
            else
            {
                if (playerCount <= 4)
                {
                    players4p[0].GetComponent<LudoPlayerController>().Photo.GetComponent<ImageDownload>().url = GameManager.Instance.AvatarURL;
                    players4p[0].GetComponent<LudoPlayerController>().Photo.GetComponent<ImageDownload>().enabled = true;
                }
                else
                {
                    players6p[0].GetComponent<LudoPlayerController>().Photo.GetComponent<ImageDownload>().url = GameManager.Instance.AvatarURL;
                    players6p[0].GetComponent<LudoPlayerController>().Photo.GetComponent<ImageDownload>().enabled = true;
                }
            }
        }
        
        OnNext_Turn();
        if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
        {
            //in online, RequestConnect method will be corutined.
            StartCoroutine(RequestConnect());
        }
    }
    private void OnDestroy()
    {
        DisableEvents();
    }
    void DisableEvents()
    {
        socket.Off("RECONNECT_RESULT", GetReconnect);
        socket.Off("REQ_TURNUSER_RESULT", OnGetTurnUser);
        socket.Off("REQ_ROLL_DICE_RESULT", OnGetRollDiceResult);
        socket.Off("REQ_MOVE_TOKEN_RESULT", OnGetMoveTokenResult);
        socket.Off("REQ_LEAVE_ROOM_RESULT", OnGetLeaveRoomResult);
        socket.Off("REQ_AUTO_RESULT", OnGetAUTOResult);
        socket.Off("GAME_END", OnGetEndGameResult);
        socket.Off("GET_USERINFO_RESULT", GetUserInfoResult);
        socket.Off("REQ_UPDATE_USERINFO_RESULT", GetUpdateUserInfoResult);
        socket.Off("REQ_PAUSE_RESULT", GetPausedResult);
        socket.Off("REQ_RESUME_RESULT", GetResumedResult);
        socket.Off("EXIT_GAME", OnGetExitGame);
    }
    public void Initialize_Dices()
    {
        if (PlayerControllers[TurnId].isCheckInit())
            return;
        PlayerControllers[TurnId].Dice_Init();
        TurnOff();
        OnNext_Turn();
    }
   
    public void OnNext_Turn() 
    {
        if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
        {
            if(TurnId == -1)
                TurnId = firstTurnId;
            
            {
                SetTurnUser();
            }
        }
        else
        {
            if (TurnId == -1)
            {
                TurnId = UnityEngine.Random.Range(0, playerCount);
                PlayerControllers[TurnId].TurnOn();
                return;
            }

            if (MoveSteps < 6)
            {
                if (TurnId < playerCount - 1) 
                    TurnId++;
                else
                    TurnId = 0;
            }
            PlayerControllers[TurnId].TurnOn();
            MoveSteps = 0;
        }

    }
    public void TurnOff()
    {
        PlayerControllers[TurnId].TurnOff();
    }
    public UILabel logTxt;
    public void SetTurnUser()
    {
        Dictionary<string, string> turndata = new Dictionary<string, string>();
        turndata.Add("roomid", GameManager.Instance.RoomID.ToString());
        turndata.Add("dice", MoveSteps.ToString());
        turndata.Add("username", GameManager.Instance.UserName);
        turndata.Add("userphone", GameManager.Instance.UserPhone);
        socket.Emit("REQ_TURNUSER", new JSONObject(turndata));
    }

    private void OnGetTurnUser(SocketIOEvent evt)
    {
        Loading.SetActive(false);
        turnuser = Global.JsonToString(evt.data.GetField("turnuser").ToString(), "\"");
        diceNumber = int.Parse(evt.data.GetField("dice").ToString());
        for (int i = 0; i < GameManager.Instance.Users.Count; i++)
        {
            if (GameManager.Instance.Users[i].phone == turnuser)
            {
                TurnId = i;
                MoveSteps = diceNumber;
                PlayerControllers[i].DiceController.value = diceNumber;
                PlayerControllers[i].TurnOn();
                MoveSteps = 0;
                //break;
            }
            else
            {
                PlayerControllers[i].Dice_Init();
            }
        }
    }
    public void Roll_Event()
    {
        if (turnuser != GameManager.Instance.UserPhone)
            return;
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("roomid", GameManager.Instance.RoomID.ToString());
        data.Add("roller", turnuser);
        data.Add("dice", diceNumber.ToString());
        socket.Emit("REQ_ROLL_DICE", new JSONObject(data));
        _roller = GameManager.Instance.UserPhone;
        Debug.Log( turnuser + " Roll Dice ");
    }
    private void OnGetRollDiceResult(SocketIOEvent evt)
    {
        string roller = Global.JsonToString(evt.data.GetField("roller").ToString(), "\"");
        int dice_number = int.Parse(Global.JsonToString(evt.data.GetField("dice").ToString(), "\""));
       
        _roller = roller;
        for (int i = 0; i < GameManager.Instance.Users.Count; i++)
        {
            if (GameManager.Instance.Users[i].phone == roller)
            {
                TurnId = i;
                PlayerControllers[i].DiceController.value = dice_number;
                PlayerControllers[i].DiceController.Dice_Roll();
                break;
            }
        }
    }
    public void Move_StartToken_Event(int tokenIndex)
    {
        if (turnuser != GameManager.Instance.UserPhone)
        {
            Debug.LogError("Check turnuser logic again");
            return;
        }
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("roomid", GameManager.Instance.RoomID.ToString());
        data.Add("mover", turnuser);
        data.Add("path", tokenIndex.ToString());
        data.Add("status", "startmove");
        socket.Emit("REQ_MOVE_TOKEN", new JSONObject(data));
    
    }
    public void Move_Token_Event(int tokenIndex)
    {
        if (turnuser != GameManager.Instance.UserPhone && TurnId != 0)
        {
            Debug.LogError("Check turnuser logic again");
            return;
        }
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("roomid", GameManager.Instance.RoomID.ToString());
        data.Add("mover", turnuser);
        data.Add("path", tokenIndex.ToString());
        data.Add("status", "pathmove");
        socket.Emit("REQ_MOVE_TOKEN", new JSONObject(data));
  
    }
    private void OnGetMoveTokenResult(SocketIOEvent evt)
    {
        string status = Global.JsonToString(evt.data.GetField("status").ToString(), "\"");
        string mover = Global.JsonToString(evt.data.GetField("mover").ToString(), "\"");
        int path = int.Parse(Global.JsonToString(evt.data.GetField("path").ToString(), "\""));
       

        for (int i = 0; i < GameManager.Instance.Users.Count; i++)
        {
            if (GameManager.Instance.Users[i].phone == mover)
            {
                if (status == "startmove")
                {
                    PlayerControllers[i].PawnsStatic[path].GetComponent<LudoPawnController>().OnlineStartMove();
                }
                else
                {
                    int currentpath = PlayerControllers[i].PawnsMoved[path].GetComponent<LudoPawnController>().currentPath;
                    PlayerControllers[i].PawnsMoved[path].GetComponent<LudoPawnController>().real_paths[currentpath].GetComponent<LudoPathController>().OnlineClick();
                }
                break;
            }
        }
    }
    private void OnGetLeaveRoomResult(SocketIOEvent evt)
    {
        print("check -----------------  " + evt);
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        if (result == "success")
        {
            string username = Global.JsonToString(evt.data.GetField("username").ToString(), "\"");
            string userphone = Global.JsonToString(evt.data.GetField("userphone").ToString(), "\"");
            

            for (int i = 0; i < GameManager.Instance.Users.Count; i++)
            {
                if (GameManager.Instance.Users[i].phone == userphone)
                {                    
                    GameManager.Instance.Users.RemoveAt(i);
                    print("check -----------------123123");
                    PlayerControllers[i].transform.Find("Leave").gameObject.SetActive(true);
                    PlayerControllers[i].Initialize();
                    PlayerControllers[i].Paused = true;
                    PlayerControllers[i].DiceController.OnDisable_DiceInit();
                    break;
                }
            }
        }
    }
    public void ChangeAutoStatus(bool isAuto)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("roomid", GameManager.Instance.RoomID.ToString());
        data.Add("user", GameManager.Instance.UserName);
        data.Add("auto", isAuto.ToString());
        socket.Emit("REQ_AUTO", new JSONObject(data));
    }
    private void OnGetAUTOResult(SocketIOEvent evt)
    {
        string username = Global.JsonToString(evt.data.GetField("user").ToString(), "\"");
        string auto = Global.JsonToString(evt.data.GetField("auto").ToString(), "\"");
        if (GameManager.Instance.UserName != username)
            return;
        for (int i = 0; i < GameManager.Instance.Users.Count; i++)
        {
            if (GameManager.Instance.Users[i].name == username)
            {
                if (auto == "True")
                {
                    PlayerControllers[i].isAUTO = true;
                }
                else
                {
                    PlayerControllers[i].isAUTO = false;
                }
                break;
            }
        }
    }
    public void GetUserInfo(string userphone)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("userphone", userphone);
        socket.Emit("REQ_USER_INFO", new JSONObject(data));
    }

    private void GetUserInfoResult(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        if (result.Equals("success"))
        {
            string name = Global.JsonToString(evt.data.GetField("username").ToString(), "\"");
            string userphone = Global.JsonToString(evt.data.GetField("userphone").ToString(), "\"");
            string userid = Global.JsonToString(evt.data.GetField("userid").ToString(), "\"");
            string photoURL = Global.JsonToString(evt.data.GetField("photo").ToString(), "\"");
            int points = int.Parse(evt.data.GetField("points").ToString());
            int level = int.Parse(evt.data.GetField("level").ToString());
            int referral_count = int.Parse(evt.data.GetField("referral_count").ToString());

            JSONObject online_multiplayer = evt.data.GetField("online_multiplayer");
            HistoryPlay Online_Multiplayer = new HistoryPlay();
            Online_Multiplayer.played = int.Parse(online_multiplayer.GetField("played").ToString());
            Online_Multiplayer.won = int.Parse(online_multiplayer.GetField("won").ToString());

            JSONObject friend_multiplayer = evt.data.GetField("friend_multiplayer");
            HistoryPlay Friend_Multiplayer = new HistoryPlay();
            Friend_Multiplayer.played = int.Parse(friend_multiplayer.GetField("played").ToString());
            Friend_Multiplayer.won = int.Parse(friend_multiplayer.GetField("won").ToString());

            JSONObject tokens_captured = evt.data.GetField("tokens_captured");
            HistoryToken TokensCaptued = new HistoryToken();
            TokensCaptued.mine = int.Parse(tokens_captured.GetField("mine").ToString());
            TokensCaptued.opponents = int.Parse(tokens_captured.GetField("opponents").ToString());

            JSONObject won_streaks = evt.data.GetField("won_streaks");
            HistoryWonStreaks WonStreaks = new HistoryWonStreaks();
            WonStreaks.current = int.Parse(won_streaks.GetField("current").ToString());
            WonStreaks.best = int.Parse(won_streaks.GetField("best").ToString());

            for (int i = 0; i < UserDetails.Count; i++)
            {
                if (UserDetails[i].userphone == userphone)
                {
                    UserDetails[i].userid = userid;
                    UserDetails[i].userphone = userphone;
                    UserDetails[i].photoURL = photoURL;
                    UserDetails[i].points = points;
                    UserDetails[i].level = level;
                    UserDetails[i].referral_count = referral_count;
                    UserDetails[i].online_multiplayer = Online_Multiplayer;
                    UserDetails[i].friend_multiplayer = Friend_Multiplayer;
                    UserDetails[i].tokens_captured = TokensCaptued;
                    UserDetails[i].won_streaks = WonStreaks;

                    if (photoURL != "")
                    {
                        PlayerControllers[i].Photo.GetComponent<ImageDownload>().url = photoURL;
                        PlayerControllers[i].Photo.GetComponent<ImageDownload>().enabled = true;
                    }
                    break;
                }
            }
        }
    }
    public void OnRank(int winner)
    {
        Ranking.Add(winner);
        for (int i = 0; i < playerCount; i++)
        {
            PlayerControllers[i].Paused = true;
            if (!Ranking.Contains(i))
            {
                Ranking.Add(i);
            }
        }
        if (Ranking.Count == playerCount)
        {
            ShowRankResult();
        }
    }
    private void OnGetEndGameResult(SocketIOEvent evt)
    {
        print("----Arrived GameEnd response ----");
        DisconnectedPanel.SetActive(false);
        for (int i = 0; i < playerCount; i++)
        {
            Ranking.Add(i);
        }
        string outerphone = Global.JsonToString(evt.data.GetField("outerphone").ToString(), "\"");
        ShowRankResult(outerphone);
    }
    private void OnGetExitGame(SocketIOEvent evt) 
    {
        DisconnectedPanel.SetActive(false);
        ExitGameScene();
    }
    public void ShowRankResult(string outerPhone = "")
    {
        int rank1, rank2;
        List<int> winmoneys = new List<int>();
        if (playerCount > 2)
        {
            rank1 = Mathf.RoundToInt(GameManager.Instance.RoomWinMoney * 0.7f);
            rank2 = Mathf.RoundToInt(GameManager.Instance.RoomWinMoney * 0.3f);
            winmoneys.Add(rank1);
            winmoneys.Add(rank2);
            winmoneys.Add(0);
            winmoneys.Add(0);
        }
        else
        {
            winmoneys.Add(GameManager.Instance.RoomWinMoney);
            winmoneys.Add(0);
        }

        if(outerPhone != GameManager.Instance.UserPhone)
        {
            for (int i = 0; i < Ranking.Count; i++)
            {
                int index = Ranking[i];
                RankItem.transform.Find("crawn").gameObject.SetActive(i == 0 ? true : false);
                RankItem.transform.Find("Avatar").GetComponent<UIMaskedTexture>().mainTexture = PlayerControllers[index].Photo.mainTexture;
                RankItem.transform.Find("Username").GetComponent<UILabel>().text = PlayerControllers[index].NickNameLabel.text;
                RankItem.transform.Find("+balance").GetComponent<UILabel>().text = "+" + winmoneys[i];

                for (int j = 0; j < Ranking.Count; j++)
                {
                    print("RAnking " + j + " : " + Ranking[j]);
                }
                NGUITools.AddChild(RankGrid, RankItem);
                if (index == 0)
                {
                    PlayerControllers[index].Timer.gameObject.SetActive(false);
                    if (winmoneys[i] > 0)
                    {
                        Firework.SetActive(true);
                        GameManager.Instance.Points += winmoneys[i];
                        Counting(GameManager.Instance.Points, GameManager.Instance.Points - winmoneys[i]);
                        AddGameHistory(winmoneys[i]);
                    }

                    if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
                    {
                        if (GameManager.Instance._Wifi == WIFI.online)
                        {
                            if (winmoneys[i] > 0)
                            {
                                GameManager.Instance.Online_Multiplayer.won++;
                                GameManager.Instance.WonStreaks.current++;
                                if (GameManager.Instance.WonStreaks.current > GameManager.Instance.WonStreaks.best)
                                    GameManager.Instance.WonStreaks.best = GameManager.Instance.WonStreaks.current;
                            }
                            else
                            {
                                GameManager.Instance.WonStreaks.current = 0;
                            }
                        }
                        else
                        {
                            if (winmoneys[i] > 0)
                            {
                                GameManager.Instance.Friend_Multiplayer.won++;
                                GameManager.Instance.WonStreaks.current++;
                                if (GameManager.Instance.WonStreaks.current > GameManager.Instance.WonStreaks.best)
                                    GameManager.Instance.WonStreaks.best = GameManager.Instance.WonStreaks.current;
                            }
                            else
                            {
                                GameManager.Instance.WonStreaks.current = 0;
                            }
                        }
                        GameManager.Instance.TokensCaptued.mine += PlayerControllers[0].captured_mine;
                        GameManager.Instance.TokensCaptued.opponents += PlayerControllers[0].captured_opponent;
                    }
                    UpdateUserInfo();
                }
            }
        }
        else
        {            
            RankItem.transform.Find("Avatar").GetComponent<UIMaskedTexture>().mainTexture = PlayerControllers[1].Photo.mainTexture;
            RankItem.transform.Find("Username").GetComponent<UILabel>().text = PlayerControllers[1].NickNameLabel.text;
            RankItem.transform.Find("+balance").GetComponent<UILabel>().text = "+" + winmoneys[0];

            NGUITools.AddChild(RankGrid, RankItem);

            RankItem.transform.Find("Avatar").GetComponent<UIMaskedTexture>().mainTexture = PlayerControllers[0].Photo.mainTexture;
            RankItem.transform.Find("Username").GetComponent<UILabel>().text = PlayerControllers[0].NickNameLabel.text;
            RankItem.transform.Find("+balance").GetComponent<UILabel>().text = "+" + winmoneys[1];

            NGUITools.AddChild(RankGrid, RankItem);

            if(RankGrid.GetChild(0).Find("Username").GetComponent<UILabel>().text == GameManager.Instance.UserName)
            {
                Firework.SetActive(true);
                GameManager.Instance.Points += winmoneys[0];
                Counting(GameManager.Instance.Points, GameManager.Instance.Points - winmoneys[0]);
            }

            UpdateUserInfo();
        }


        



        RankGrid.GetComponent<UIGrid>().Reposition();
        StartCoroutine(StopAllPlayers());
    }
    IEnumerator StopAllPlayers()
    {
        yield return new WaitForSeconds(2.0f);
        GameEndPopup.SetActive(true);
        for (int i = 0; i < PlayerControllers.Count; i++)
        {
            PlayerControllers[i].Paused = true;
            PlayerControllers[i].DiceController.Dice_Init();
            DisableEvents();
        }
    }
    #region UPDATE USER PROFILE
    public void UpdateUserInfo()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("username", GameManager.Instance.UserName);
        data.Add("userphone", GameManager.Instance.UserPhone);
        data.Add("points", GameManager.Instance.Points.ToString());
        data.Add("winning_amount", GameManager.Instance.RoomWinMoney.ToString());
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
        print(evt.name + " : " + evt.data);
    }
    #endregion
    public void CloseRankingPanel()
    {
        var animator = GameEndPopup.transform.Find("PopupAnimation").GetComponent<Animator>();
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("PopupOpen"))
            animator.Play("PopupClose");
        StartCoroutine("GoMenu");
    }
    IEnumerator GoMenu()
    {
        GameForceEnd();
        yield return new WaitForSeconds(0.5f);
        ExitGameScene();
    }

    public GameObject ExitPanel;
    public void ClickExitGame()
    {
        ExitPanel.SetActive(true);
    }

    public void ClickYes()
    {
        ExitGameScene();
    }

    public void ClickNo()
    {
        ExitPanel.SetActive(false);
    }

    public void ExitGameScene()
    {
        LeaveGame();
        CancelGame();
       // if (Advertisements.Instance.IsInterstitialAvailable())
       // {
       //     adsmanager.ShowInterstitial();
       // }
       // else
      //  {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
       // }
    }
    
    public void LeaveGame() // request the game end socket to server.
    {
        if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["roomid"] = "" + GameManager.Instance.RoomID;
            data["username"] = GameManager.Instance.UserName;
            data["userphone"] = GameManager.Instance.UserPhone;
            socket.Emit("REQ_LEAVE_ROOM", new JSONObject(data));
        }
    }
    public void LeaveGame(string name, string phone) // request the game end socket to server.
    {
        if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
        {
            if (CoroutineBgrTime != null)
                StopCoroutine(CoroutineBgrTime);
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["roomid"] = "" + GameManager.Instance.RoomID;
            data["username"] = name;
            data["userphone"] = phone;
            socket.Emit("REQ_LEAVE_ROOM", new JSONObject(data));
        }
    }

    public void CancelGame()
    {
        if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
        {
            GameManager.Instance.WonStreaks.current = 0;
            GameManager.Instance.TokensCaptued.mine += PlayerControllers[0].captured_mine;
            GameManager.Instance.TokensCaptued.opponents += PlayerControllers[0].captured_opponent;
            UpdateUserInfo();
        }
    }

    public void AddGameHistory(int point)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("username", GameManager.Instance.UserName);
        data.Add("points", point.ToString());
        JSONObject jdata = new JSONObject(data);
        socket.Emit("REQ_GAME_HIST", jdata);
    }

    public void IncreaseCoin()
    {
        GameManager.Instance.Vibrating();
        CoinEffect.SetActive(false);
        CoinEffect.SetActive(true);
        Invoke("Disable_Effect", 1.0f);
    }
    void Disable_Effect()
    {
        CoinEffect.SetActive(false);
    }
    public void Counting(int target, int current)
    {
        StartCoroutine(Count((float)(target), (float)(current)));
    }

    IEnumerator Count(float target, float current)
    {
        float duration = 0.8f;
        float offset = (target - current) / duration;
        while (current < target)
        {
            current += offset * Time.deltaTime;
            MyPoints.text = ((int)current).ToString("N0");
            yield return null;
        }
        current = target;
        MyPoints.text = ((int)current).ToString("N0");
        IncreaseCoin();
    }
    #region Reconnection
    bool isConnected = true;
    int isCal = 0;
    string diconnected_username = "";

    IEnumerator RequestConnect()
    {
        while (true)
        {
            if (socket.disconnected == true) //when detect the socket disconnect in client.
            {
                ShowWarning(); //display warning screen.
            }
            else
            {
                HidenWarning();//hide warning screen.
                
            }
            yield return null;
        }        
    }

    public void ShowWarning()
    {
        if (isConnected)
        {
            isConnected = false;
            if (GameEndPopup.activeSelf)
                return;
     
            StopPlay(); //when stop current playing.
            StartCoroutine(ShowDisconnected());
        }
    }
    IEnumerator CheckOpponentConnect()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            ShowOppornentWarning();
        }
    }
    public void ShowOppornentWarning()
    {
        if (DisconnectedPanel == null)
            DisconnectedPanel = GameObject.Find("UI Root/Camera/DisconnectedPanel").gameObject;
        int i = 0;
        if (socket.disconnected == false)
        {
            for (int j = 0; j < PlayerControllers.Count; j++)
            {
                if (PlayerControllers[j].Timer.GetComponent<UISprite>().enabled == false)
                {
                    i++;
                }
            }
            if (i == PlayerControllers.Count)
            {
                if (DisconnectedPanel.activeSelf == false)
                {
                    DisconnectedPanel.SetActive(true);
                    DisconnectedPanel.transform.Find("MyConnectError").gameObject.SetActive(false);
                    DisconnectedPanel.transform.Find("OpponentConnectError").gameObject.SetActive(true);
                    Timer.gameObject.SetActive(false);
                }
            }
            else
                DisconnectedPanel.SetActive(false);
        }
        if (GameEndPopup.activeSelf)
            DisconnectedPanel.SetActive(false);
    }
    IEnumerator ShowDisconnected()
    {
        yield return null;
        if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
        {
            DisconnectedPanel.SetActive(true);
            DisconnectedPanel.transform.Find("MyConnectError").gameObject.SetActive(true);
            DisconnectedPanel.transform.Find("OpponentConnectError").gameObject.SetActive(false);

            CoroutineDisconnectTime = CheckDisconnectTime();
            StartCoroutine(CoroutineDisconnectTime);
        }
    }
    public UILabel Timer;

    IEnumerator CheckDisconnectTime() //this is disconnect time counter.
    {
        float estimatedTime = 0;
        string strTime = "";
        Timer.gameObject.SetActive(true);
        while (true)
        {
            yield return null;
            estimatedTime += Time.deltaTime;
            strTime = pad(Mathf.RoundToInt(estimatedTime), 2);
            Timer.text = "00:" + strTime;
            if (strTime == "59") //if timer is 60 secs, then game end.
            {
                ExitGameScene();
            }
        }
    }

    public void HidenWarning() //when reconnected it will be called
    {
        if (!isConnected)
        {
            isConnected = true;
            if (CoroutineDisconnectTime != null)
            {
                StopCoroutine(CoroutineDisconnectTime);
                CoroutineDisconnectTime = null;
            }
            Timer.gameObject.SetActive(false);
            if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
                StartCoroutine(EmitReconnect());
            else
                Replay();
        }
    }
    IEnumerator EmitReconnect()
    {
        float waitTime = 0f;
        while (string.IsNullOrEmpty(socket.sid) && waitTime < 10f)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }
        if (string.IsNullOrEmpty(socket.sid))
            yield break;

        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("roomid", GameManager.Instance.RoomID.ToString());
        data.Add("username", GameManager.Instance.UserName);
        data.Add("userphone", GameManager.Instance.UserPhone);
        data.Add("old_socketID", socketID);
        socket.Emit("RECONNECTED", new JSONObject(data));
        yield return null;
        socketID = socket.sid;
    
    }
    private void Replay()
    {
        for (int i = 0; i < PlayerControllers.Count; i++)
        {
            PlayerControllers[i].ResumePlay();
        }
        DisconnectedPanel.SetActive(false);
    }
    private void StopPlay()
    {
        for (int i = 0; i < PlayerControllers.Count; i++)
        {
            PlayerControllers[i].Stopplay();
        }
    }

    private void GetReconnect(SocketIOEvent evt)
    {
        int roomID = int.Parse(evt.data.GetField("roomid").ToString());
        string reconnecter = Global.JsonToString(evt.data.GetField("reconnecter").ToString(), "\"");
        string status = Global.JsonToString(evt.data.GetField("status").ToString(), "\"");
        string mover = Global.JsonToString(evt.data.GetField("mover").ToString(), "\"");
        string pa = Global.JsonToString(evt.data.GetField("path").ToString(), "\"");
        if (pa == "") pa = "10";
        int path = int.Parse(pa);
       
        if(CoroutineDisconnectTime != null)
            StopCoroutine(CoroutineDisconnectTime);
        if(GameManager.Instance.RoomID == roomID)
            Replay();
        if (reconnecter == GameManager.Instance.UserPhone && GameManager.Instance.UserPhone == turnuser)
            return;
        if (reconnecter != GameManager.Instance.UserPhone && GameManager.Instance.UserPhone != turnuser)
            return;
        
        StartCoroutine(ContinuePlay(roomID, status, path, mover, reconnecter));
    }
    public UILabel log;
    IEnumerator ContinuePlay(int roomID, string status, int path, string mover, string reconnecter)
    {
        yield return null;

        if (GameManager.Instance.RoomID == roomID)
        {
            if (reconnecter == GameManager.Instance.UserPhone && GameManager.Instance.UserPhone != turnuser)
            {

                if (turnuser == mover)
                {
                    int i = 0;
                    for (int j = 0; j < UserDetails.Count; j++)
                    {
                        if (UserDetails[j].userphone == mover)
                        {
                            i = j;
                        }
                    }
                    if (PlayerControllers[i].DiceController.isTurn && !PlayerControllers[i].DiceController.isRolled)
                    {

                        PlayerControllers[i].DiceController.Dice_Roll();
                        yield return new WaitForSeconds(4.0f);
                        if (path != 10)
                        {
                            if (status == "startmove")
                            {
                                log.text += "@ start move after dice";
                                PlayerControllers[i].PawnsStatic[path].GetComponent<LudoPawnController>().OnlineStartMove();
                                log.text += "@" + MoveSteps;

                            }
                            else if (status == "pathmove")
                            {
                                log.text += "@ path move after dice";
                                int currentpath = PlayerControllers[i].PawnsMoved[path].GetComponent<LudoPawnController>().currentPath;
                                PlayerControllers[i].PawnsMoved[path].GetComponent<LudoPawnController>().real_paths[currentpath].GetComponent<LudoPathController>().OnlineClick();

                                log.text += "@" + MoveSteps;
                            }
                        }
                    }
                    else
                    {
                        if (path != 10)
                        {
                            if (status == "startmove")
                            {
                                log.text += "@ start move";
                                PlayerControllers[i].PawnsStatic[path].GetComponent<LudoPawnController>().OnlineStartMove();
                            }
                            else if (status == "pathmove")
                            {
                                log.text += "@ path move";
                                int currentpath = PlayerControllers[i].PawnsMoved[path].GetComponent<LudoPawnController>().currentPath;
                                PlayerControllers[i].PawnsMoved[path].GetComponent<LudoPawnController>().real_paths[currentpath].GetComponent<LudoPathController>().OnlineClick();
                            }
                        }
                    }
                }
                else
                {

                    int i = 0;
                    for (int j = 0; j < UserDetails.Count; j++)
                    {
                        if (UserDetails[j].userphone == turnuser)
                        {
                            i = j;
                        }
                    }
                    if (PlayerControllers[i].DiceController.isTurn && !PlayerControllers[i].DiceController.isRolled)
                    {
                        PlayerControllers[i].DiceController.Dice_Roll();
              
                    }
                  
                }


            }
            else if (reconnecter != GameManager.Instance.UserPhone && GameManager.Instance.UserPhone == turnuser)
            {

            }
        }
    }

    #endregion
    bool isPaused = false;
    void OnApplicationPause(bool pauseStatus)
    {
        isPaused = pauseStatus;
        if (isPaused)
        {
            DisconnectedPanel.SetActive(true);
            DisconnectedPanel.transform.Find("MyConnectError").gameObject.SetActive(true);
            DisconnectedPanel.transform.Find("OpponentConnectError").gameObject.SetActive(false);

            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("roomid", GameManager.Instance.RoomID.ToString());
            data.Add("outerName", GameManager.Instance.UserName);
            data.Add("outerPhone", GameManager.Instance.UserPhone);

            socket.Emit("REQ_PAUSE", new JSONObject(data));
        }
        else
        {
            DisconnectedPanel.SetActive(false);
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("roomid", GameManager.Instance.RoomID.ToString());
            
            socket.Emit("REQ_RESUME", new JSONObject(data));
        }
    }

    string outerPhone = "";
    string outerName = "";
    private void GetPausedResult(SocketIOEvent evt)
    {
        if (GameEndPopup != null)
        {
            if (GameEndPopup.activeSelf)
                return;
        }
      
        int roomID = int.Parse(Global.JsonToString(evt.data.GetField("roomid").ToString(), "\""));

        outerPhone = "";
        outerName = "";

        if (GameManager.Instance.RoomID == roomID)
        {
            outerPhone = Global.JsonToString(evt.data.GetField("outerPhone").ToString(), "\"");
            outerName = Global.JsonToString(evt.data.GetField("outerName").ToString(), "\"");
            print(outerName + " are now background status -------------");

            if (DisconnectedPanel == null)
                DisconnectedPanel = GameObject.Find("UI Root/Camera/DisconnectedPanel").gameObject;
            DisconnectedPanel.SetActive(true);
            DisconnectedPanel.transform.Find("MyConnectError").gameObject.SetActive(false);
            DisconnectedPanel.transform.Find("OpponentConnectError").gameObject.SetActive(true);
            StopPlay();
            CoroutineBgrTime = CheckBackgroundTime();
            StartCoroutine(CoroutineBgrTime);
        }
    }

    IEnumerator CheckBackgroundTime() //this is disconnect time counter.
    {
        print("CheckBackground time ----- ");
        float estimatedTime = 0;
        string strTime = "";
        Timer.text = "";
        Timer.gameObject.SetActive(true);
        while (true)
        {
            yield return null;
            estimatedTime += Time.deltaTime;
            strTime = pad(Mathf.RoundToInt(estimatedTime), 2);
            Timer.text = "00:" + strTime;
            if (strTime == "60") //if timer is 60 secs, then game end.
            {
                //ExitGameScene();
                LeaveGame(outerName, outerPhone);
                CancelGame();
                break;
            }
        }
    }

    private void GetResumedResult(SocketIOEvent evt)
    {
        if (GameEndPopup != null)
        {
            if (GameEndPopup.activeSelf)
                return;
        }
   
        int roomID = int.Parse(Global.JsonToString(evt.data.GetField("roomid").ToString(), "\""));
        if (GameManager.Instance.RoomID == roomID)
        {
            if (DisconnectedPanel == null)
                DisconnectedPanel = GameObject.Find("UI Root/Camera/DisconnectedPanel").gameObject;
            DisconnectedPanel.SetActive(false);
            if (CoroutineBgrTime != null)
                StopCoroutine(CoroutineBgrTime);
            Replay();
        }
    }
    public void GameForceEnd()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("roomid", GameManager.Instance.RoomID.ToString());
        socket.Emit("Game_Fore_End", new JSONObject(data));
        DisconnectedPanel.SetActive(false);
    }
    string pad(int n, int width)
    {
        string n_s = n.ToString();

        if (n_s.Length >= width)
            return n_s;
        else
        {
            string a = "";
            for (int i = 0; i < width - n_s.Length; i++)
            {
                a += "0";
            }
            a += n_s;
            return a;
        }
    }
}
[Serializable]
public class UserDetail
{
    public string username = "";
    public string userphone = "";
    public string userid = "";
    public string photoURL = "";
    public int points;
    public int level;
    public int referral_count;
    public HistoryPlay online_multiplayer = new HistoryPlay();
    public HistoryPlay friend_multiplayer = new HistoryPlay();
    public HistoryToken tokens_captured = new HistoryToken();
    public HistoryWonStreaks won_streaks = new HistoryWonStreaks();
}
public enum GameMode
{
    classic, quick, arrow
}

public enum GamePlayType
{
    two = 2, three = 3, four = 4, five = 5, six = 6
}

public enum PlayerType
{
    blue = 1, red, green, yellow, purple, orange
}