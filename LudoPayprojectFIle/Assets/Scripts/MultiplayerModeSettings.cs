using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class MultiplayerModeSettings : MonoBehaviour
{
    public MenuManager menuManager;
    public GameObject SearchUserManage;
    public UIButton[] GameModes;
    public UIButton[] NumberOfPlayers;
    public UIEventTrigger[] SelectPlayer;
    public UIEventTrigger[] SelectPlayer2;
    public int[] players = new int[6] { 1, 2, 3, 4, 5, 6};
    public string[] playersName;
    public GameObject[] offlinemodes;
    public GameObject[] onlinemodes;
    public GameObject[] vscomputermodes;
    public GameObject[] privatemodes;
    public Transform PlayersNumber;
    public UILabel stake_money;
    public UILabel win_money;
    public List<int> stakes = new List<int>();
    public List<int> wins = new List<int>();

    private void Awake()
    {
        menuManager = transform.parent.GetComponent<MenuManager>();

        GameManager.Instance._GameMode = GameMode.classic;
        GameManager.Instance._PlayerType = PlayerType.blue;
        GameManager.Instance._GamePlayType = GamePlayType.two;
    }
   
    private void OnEnable()
    {
        if(stakes.Count > 0)
        {
            stake_money.text = stakes[0].ToString();
            win_money.text = wins[0].ToString();
        }
        
        onlinemodes[0].SetActive(false); privatemodes[0].SetActive(false);
        PlayersNumber.GetChild(1).GetComponent<UIToggle>().Set(true);
        for (int i = 2; i < 6; i++)
        {
            PlayersNumber.GetChild(i).GetComponent<UIToggle>().Set(false);
        }
        GameManager.Instance._GamePlayType = GamePlayType.two;
        switch (GameManager.Instance._Wifi)
        {
            case WIFI.offline:
                SetUI(offlinemodes, onlinemodes, vscomputermodes);
                PlayersNumber.GetChild(2).gameObject.SetActive(true);
                PlayersNumber.GetChild(3).gameObject.SetActive(true);
                PlayersNumber.GetChild(4).gameObject.SetActive(true);
                PlayersNumber.GetChild(5).gameObject.SetActive(true);
                PlayersNumber.GetChild(1).localPosition = new Vector3(-289, 55, 0);
                PlayersNumber.GetChild(2).localPosition = new Vector3(-289, -31, 0);
                PlayersNumber.GetChild(3).localPosition = new Vector3(-289, -120, 0);
                PlayersNumber.GetChild(4).localPosition = new Vector3(45, 55, 0);
                PlayersNumber.GetChild(5).localPosition = new Vector3(46.4f, -30.6f, 0);
                break;
            case WIFI.online:
                SetUI(onlinemodes, offlinemodes, vscomputermodes);
                PlayersNumber.GetChild(2).gameObject.SetActive(false);                
                PlayersNumber.GetChild(3).gameObject.SetActive(true);
                PlayersNumber.GetChild(4).gameObject.SetActive(false);
                PlayersNumber.GetChild(5).gameObject.SetActive(false);
                PlayersNumber.GetChild(1).localPosition = new Vector3(-271, 42, 0);
                PlayersNumber.GetChild(3).localPosition = new Vector3(-271, -27, 0);
                PlayersNumber.GetChild(5).localPosition = new Vector3(-271, -96, 0);
                break;
            case WIFI.vsComputer:
                SetUI(vscomputermodes, offlinemodes, onlinemodes);
                PlayersNumber.GetChild(2).gameObject.SetActive(true);
                PlayersNumber.GetChild(3).gameObject.SetActive(true);
                PlayersNumber.GetChild(4).gameObject.SetActive(true);
                PlayersNumber.GetChild(5).gameObject.SetActive(true);
                PlayersNumber.GetChild(1).localPosition = new Vector3(-289, 55, 0);
                PlayersNumber.GetChild(2).localPosition = new Vector3(-289, -31, 0);
                PlayersNumber.GetChild(3).localPosition = new Vector3(-289, -120, 0);
                PlayersNumber.GetChild(4).localPosition = new Vector3(45, 55, 0);
                PlayersNumber.GetChild(5).localPosition = new Vector3(46.4f, -30.6f, 0);
                break;
            case WIFI.privateRoom:
                SetUI(privatemodes, offlinemodes, vscomputermodes);
                PlayersNumber.GetChild(2).gameObject.SetActive(false);
                PlayersNumber.GetChild(3).gameObject.SetActive(true);
                PlayersNumber.GetChild(4).gameObject.SetActive(false);
                PlayersNumber.GetChild(5).gameObject.SetActive(false);
                PlayersNumber.GetChild(1).localPosition = new Vector3(-271, 42, 0);
                PlayersNumber.GetChild(3).localPosition = new Vector3(-271, -27, 0);
                PlayersNumber.GetChild(5).localPosition = new Vector3(-271, -96, 0);
                break;
        }
    }
    void SetUI(GameObject[] objs1, GameObject[] objs2, GameObject[] objs3)
    {
        foreach (GameObject item in objs2)
        {
            item.SetActive(false);
        }
        foreach (GameObject item in objs3)
        {
            item.SetActive(false);
        }
        foreach (GameObject item in objs1)
        {
            item.SetActive(true);
        }
    }
    private void Start()
    {
        for (int i = 0; i < GameModes.Length; i++)
        {
            Global.AddOnClickEvent(this, GameModes[i], "OnValueChange_GameMode", i, typeof(int));
        }
        for (int i = 0; i < NumberOfPlayers.Length; i++)
        {
            Global.AddOnClickEvent(this, NumberOfPlayers[i], "OnValueChange_NumberOfPlayers", i, typeof(int));
        }
        for (int i = 0; i < SelectPlayer.Length; i++)
        {
            Global.AddOnClickTriggerEvent(this, SelectPlayer[i], "OnValueChange_SelectedPlayer", i, typeof(int));
            Global.AddOnClickTriggerEvent(this, SelectPlayer2[i], "OnValueChange_SelectedPlayer", i, typeof(int));
            if (i > 3)
            {
                SelectPlayer[i].gameObject.SetActive(false);
                SelectPlayer2[i].gameObject.SetActive(false);
            }
        }
        
    }

    public void OnValueChange_GameMode(int index)
    {
        GameMode mode = (GameMode)index;
        GameManager.Instance._GameMode = mode;
    }
    int playercount = 2;
    public void OnValueChange_NumberOfPlayers(int index)
    {
        int playerNumber = int.Parse(NumberOfPlayers[index].gameObject.name);
        if (playercount <= 4)
        {
            if (playerNumber > 4)
                InitColor();
        }
        else
        {
            if (playerNumber <= 4)
                InitColor();
        }
        playercount = playerNumber;
        if (playercount > 4)
        {
            SelectPlayer[4].gameObject.SetActive(true);            
            SelectPlayer2[4].gameObject.SetActive(true);            

            if(playercount == 5)
            {
                SelectPlayer[5].gameObject.SetActive(false);
                SelectPlayer2[5].gameObject.SetActive(false);
            }
            else
            {
                SelectPlayer[5].gameObject.SetActive(true);
                SelectPlayer2[5].gameObject.SetActive(true);
            }
        }
        else
        {
            SelectPlayer[4].gameObject.SetActive(false);
            SelectPlayer[5].gameObject.SetActive(false);
            SelectPlayer2[4].gameObject.SetActive(false);
            SelectPlayer2[5].gameObject.SetActive(false);
        }
        GamePlayType count = (GamePlayType)playercount;
        GameManager.Instance._GamePlayType = count;
    }
    public void OnValueChange_SelectedPlayer(int index)
    {
        int count = (playercount <= 4) ? 4 : 6;
      
        for (int i = 0; i < count; i++)
        {
            int val = players[i];
            val++;
            if (val > count)
                val = 1;
            players[i] = val;
            string colorString = "";
            switch (val)
            {
                case 1:
                    colorString = "blue";
                    break;
                case 2:
                    colorString = "red";
                    break;
                case 3:
                    colorString = "green";
                    break;
                case 4:
                    colorString = "yellow";
                    break;
                case 5:
                    colorString = "purple";
                    break;
                case 6:
                    colorString = "orange";
                    break;
            }
            SelectPlayer[i].GetComponent<UISprite>().spriteName = string.Format("pawn_{0}", colorString);
            SelectPlayer2[i].GetComponent<UISprite>().spriteName = string.Format("pawn_{0}", colorString);
        }

        PlayerType playertype = (PlayerType)players[0];
        GameManager.Instance._PlayerType = playertype;
    }
    void InitColor()
    {
        players = new int[6] { 1, 2, 3, 4, 5, 6 };
        string colorString = "";
        for (int i = 0; i < players.Length; i++)
        {
            int val = players[i];
            switch (val)
            {
                case 1:
                    colorString = "blue";
                    break;
                case 2:
                    colorString = "red";
                    break;
                case 3:
                    colorString = "green";
                    break;
                case 4:
                    colorString = "yellow";
                    break;
                case 5:
                    colorString = "purple";
                    break;
                case 6:
                    colorString = "orange";
                    break;
            }
            SelectPlayer[i].GetComponent<UISprite>().spriteName = string.Format("pawn_{0}", colorString);
            SelectPlayer2[i].GetComponent<UISprite>().spriteName = string.Format("pawn_{0}", colorString);
        }
    }
    public void OnStartGame()
    {
        if (!GameManager.Instance.Online_Bot_Mode)
        {
            for (int i = 0; i < SelectPlayer.Length; i++)
            {
                playersName[i] = SelectPlayer[i].transform.Find("PlayerName").GetComponent<UIInput>().value;
                playersName[i] = SelectPlayer2[i].transform.Find("PlayerName").GetComponent<UIInput>().value;
            }
            GameManager.Instance.playerNames = playersName;

        }
        GameManager.Instance.playerColors = players;
        SceneManager.LoadScene("GameScene");
    }
    public void OnStart()
    {
        if (GameManager.Instance._Wifi == WIFI.online)
        {
            GameManager.Instance.RoomStakeMoney = int.Parse(stake_money.text);
            GameManager.Instance.RoomWinMoney = int.Parse(win_money.text);            
            menuManager.Roommanager.Check_Match_Way();
        }
        else if (GameManager.Instance._Wifi == WIFI.privateRoom)
        {
            GameManager.Instance.RoomStakeMoney = int.Parse(stake_money.text);
            GameManager.Instance.RoomWinMoney = int.Parse(win_money.text);
            menuManager.Roommanager.CreateRoom();
            gameObject.SetActive(false);
        }
        else
            OnStartGame();
    }

    public void HidenPanel()
    {
        GetComponent<UIPopup>().Close();
        Invoke("Return", 0.4f);
    }
    void Return()
    {
        menuManager.On_Home();
    }

    int count = 0;
    public void Plus()
    {
        if (count < stakes.Count - 1)
            count++;
        stake_money.text = stakes[count].ToString();
        win_money.text = wins[count].ToString();
    }
    public void Minus()
    {
        if (count > 0)
            count--;
        stake_money.text = stakes[count].ToString();
        win_money.text = wins[count].ToString();
    }

    public void SetBids(List<int> bids)
    {        
        stakes.Clear();
        wins.Clear();
        for (int i = 0; i < bids.Count; i++)
        {
            stakes.Add(bids[i]);
            int win_value = bids[i] * 2 * (100 - GameManager.Instance.Commission) / 100;
            wins.Add(win_value);
        }
    }
}

public enum WIFI
{
    offline,
    online,
    vsComputer,
    privateRoom
}
