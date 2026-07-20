using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchUserManager : MonoBehaviour
{
    public MultiplayerModeSettings MMS;
    public UILabel GameDetals;
    public UILabel Timer;
    public Transform[] players;
    public Transform[] avatar_List;
    public UITexture[] real_avatar;
    public GameObject VS;
    public GameObject CoinBox;
    public GameObject Coin;
    public GameObject WaitLabel;
    private float estimatedTime = 40.0f;
    private float roolTime = 4.0f;
    public bool searching = false;
    public RoomManager Roommanager;
    public GameObject JoinRoomPanel;
    public GameObject PrivateRoomID, ShareButton;
    public AudioSource audiosource;
    public string[] randomNames;

    void OnEnable()
    {
        if(GameManager.Instance._Wifi == WIFI.privateRoom && GameManager.Instance.isCreateRoom)
        {
            PrivateRoomID.SetActive(true);
            ShareButton.SetActive(true);
            Timer.gameObject.SetActive(false);
        }
        else
        {            
            PrivateRoomID.SetActive(false);
            ShareButton.SetActive(false);
            Timer.gameObject.SetActive(true);
        }

        if(GameManager.Instance.settingData.sound)
            audiosource.Play();
        GameDetals.text = GameManager.Instance._GameMode.ToString() + " > Entry Amount : " + GameManager.Instance.RoomStakeMoney.ToString("N0");
        estimatedTime = 40.0f;
        players[0].GetComponent<UIMaskedTexture>().mainTexture = GameManager.Instance.AvatarImage;
        players[0].GetChild(1).GetComponent<UILabel>().text = GameManager.Instance.UserName;
        int count = (int)GameManager.Instance._GamePlayType;
        for (int i = 0; i < count; i++)
        {
            players[i].gameObject.SetActive(true);
        }
        switch (GameManager.Instance._GamePlayType)
        {
            case GamePlayType.two:
                players[0].localPosition = new Vector2(-314f, 35);
                players[1].localPosition = new Vector2(314f, 35);
                break;
            case GamePlayType.three:
                players[0].localPosition = new Vector2(0, 293);
                players[1].localPosition = new Vector2(-261, -130);
                players[2].localPosition = new Vector2(261, -130);
                break;
            case GamePlayType.four:
                players[0].localPosition = new Vector2(0, 293);
                players[1].localPosition = new Vector2(-314, -93);
                players[2].localPosition = new Vector2(0, -279);
                players[3].localPosition = new Vector2(314, -93);
                break;
            case GamePlayType.five:
                players[0].localPosition = new Vector2(0, 293);
                players[1].localPosition = new Vector2(-314, 0);
                players[2].localPosition = new Vector2(-166, -293);
                players[3].localPosition = new Vector2(166, -293);
                players[4].localPosition = new Vector2(314, 0);
                break;
            case GamePlayType.six:
                players[0].localPosition = new Vector2(0, 293);
                players[1].localPosition = new Vector2(-314, 147);
                players[2].localPosition = new Vector2(-314, -171);
                players[3].localPosition = new Vector2(0, -293);
                players[4].localPosition = new Vector2(314, -171);
                players[5].localPosition = new Vector2(314, 147);
                break;
            default:
                break;
        }
        for (int k = 0; k < count-1; k++)
        {
            Transform avatars = avatar_List[k];
            if (avatars.childCount == 1)
            {
                GameObject obj = avatars.GetChild(0).gameObject;
                for (int j = 0; j < 3; j++)
                {
                    int[] randomAvatars = GetRandomInt(12, 10, 22);
                    for (int i = 0; i < randomAvatars.Length; i++)
                    {
                        GameObject ava = Instantiate(obj, avatars) as GameObject;
                        ava.GetComponent<UISprite>().spriteName = randomAvatars[i].ToString();
                    }
                }
            }
            avatars.GetComponent<UIGrid>().Reposition();
            StartCoroutine(SearchEffect(avatars));
        };
        GameManager.Instance.Online_Bot_Mode = false;
        StartCoroutine("CoinEffect");
    }

    private void OnDisable()
    {
        searching = false;
        for (int i = 1; i < players.Length; i++)
        {
            players[i].Find("username").gameObject.SetActive(false);
            avatar_List[i - 1].localPosition = Vector3.zero;
            avatar_List[i - 1].parent.Find("userphoto").gameObject.SetActive(false);
            players[i].gameObject.SetActive(false);
        }
        VS.SetActive(true);
        CoinBox.SetActive(false);
        WaitLabel.GetComponent<UILabel>().text = "Searching for player...";
    }

    IEnumerator SearchEffect(Transform avatars)
    {
        yield return new WaitForSeconds(0.1f);
        Vector3 vec = avatars.GetChild(avatars.childCount - 1).localPosition;
        avatars.GetComponent<TweenPosition>().to = new Vector3(vec.x, -vec.y, vec.z);
        avatars.GetComponent<TweenPosition>().duration = roolTime;
        avatars.GetComponent<TweenPosition>().ResetToBeginning();
        avatars.GetComponent<TweenPosition>().PlayForward();
    }
    string strTime;
    IEnumerator CoinEffect()
    {
        int randBotSearch = Random.Range(10, 15);
        while (estimatedTime > 0)
        {
            if (searching)
            {
                int count = (int)GameManager.Instance._GamePlayType;

                if (GameManager.Instance.Online_Bot_Mode)
                {
                    int[] randomInts = GetRandomInt(count, 0, 20);
                    for (int i = 0; i < randomInts.Length; i++)
                    {
                        GameManager.Instance.playerNames[i] = randomNames[randomInts[i]];
                    }
                    GameManager.Instance.playerNames[0] = GameManager.Instance.UserName;
                    for (int i = 0; i < count-1; i++)
                    {
                        OnFindPlayer(i, GameManager.Instance.playerNames[i+1], "");
                    }
                }

                estimatedTime = 0;
                VS.SetActive(false);
                CoinBox.SetActive(true);
                audiosource.Stop();
                WaitLabel.GetComponent<UILabel>().text = "Loading...";

                for (int i = 0; i < count; i++)
                {
                    Collect_Coins(players[i], i);
                }
                
                StartCoroutine(StartGame());
            }
            else
            {
                if (GameManager.Instance._Wifi != WIFI.privateRoom)
                    Timer.gameObject.SetActive(true);

                estimatedTime -= Time.deltaTime;
                strTime = pad(Mathf.RoundToInt(estimatedTime), 2);
                Timer.text = "00:" + strTime;

                if (GameManager.Instance.EnableBot == "1")
                {
                    if (strTime == randBotSearch.ToString())
                    {
                        Roommanager.LeaveRoom();
                        GameManager.Instance._Wifi = WIFI.vsComputer;
                        GameManager.Instance.Online_Bot_Mode = true;
                        GameManager.Instance.Points -= GameManager.Instance.RoomStakeMoney;
                        Roommanager.UpdateUserInfo();
                        searching = true;
                    }
                }
                else
                {
                    if(GameManager.Instance._Wifi != WIFI.privateRoom)
                    {
                        if (strTime == "00")
                        {
                            Roommanager.LeaveRoom();
                            Roommanager.Menumanager.On_MessagePanel("No any players online");
                            gameObject.SetActive(false);
                            MMS.gameObject.SetActive(true);
                        }
                    }                    
                }
            }
            yield return null;
        }
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
    public void FinishSearch(Transform ava)
    {
        if (!searching)
        {
            int index = int.Parse(ava.gameObject.name) - 2;
            avatar_List[index].transform.localPosition = Vector3.zero;
            avatar_List[index].GetComponent<TweenPosition>().ResetToBeginning();
            avatar_List[index].GetComponent<TweenPosition>().PlayForward();
        }
    }
    public void OnFindPlayer(int index, string name, string photoURL)
    {
        players[index + 1].Find("username").gameObject.SetActive(true);
        players[index + 1].Find("username").gameObject.GetComponent<UILabel>().text = name;
        avatar_List[index].GetComponent<TweenPosition>().enabled = false;
        if (photoURL != "")
        {
            avatar_List[index].parent.Find("userphoto").GetComponent<ImageDownload>().url = photoURL;
            avatar_List[index].parent.Find("userphoto").GetComponent<ImageDownload>().enabled = true;
        }
        avatar_List[index].parent.Find("userphoto").gameObject.SetActive(true);
    }
    public void OnLeavePlayer(int index)
    {
        if (gameObject.activeSelf)
        {
            players[index + 1].Find("username").gameObject.SetActive(false);
            players[index + 1].Find("username").gameObject.GetComponent<UILabel>().text = "";
            avatar_List[index].GetComponent<TweenPosition>().enabled = true;
            avatar_List[index].parent.Find("userphoto").GetComponent<ImageDownload>().url = "";
            avatar_List[index].parent.Find("userphoto").GetComponent<ImageDownload>().enabled = false;
            avatar_List[index].parent.Find("userphoto").gameObject.SetActive(false);
        }
    }
    void Collect_Coins(Transform ava, int index)
    {
        ava.Find("username").gameObject.SetActive(true);
        if (GameManager.Instance.Online_Bot_Mode)
        {
            ava.Find("username").GetComponent<UILabel>().text = GameManager.Instance.playerNames[index];
        }
        if (index != 0)
        {
            avatar_List[index - 1].GetComponent<TweenPosition>().enabled = false;
            avatar_List[index - 1].parent.Find("userphoto").gameObject.SetActive(true);
        }
        for (int i = 0; i < 7; i++)
        {
            GameObject obj = Instantiate(Coin, transform);
            obj.SetActive(true);
            obj.transform.localPosition = ava.localPosition;
            obj.GetComponent<TweenPosition>().from = obj.transform.localPosition;
            obj.GetComponent<TweenPosition>().to = Vector3.zero;
            obj.GetComponent<TweenPosition>().duration = 1.5f;
            obj.GetComponent<TweenPosition>().delay = 0.1f * i;
            obj.GetComponent<TweenPosition>().ResetToBeginning();
            obj.GetComponent<TweenPosition>().PlayForward();
            Destroy(obj, obj.GetComponent<TweenPosition>().duration + 0.1f * i);
        }
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(2.5f);
        MMS.OnStartGame();
    }
    public void Exit()
    {
        if (GameManager.Instance._Wifi == WIFI.online)
        {
            gameObject.SetActive(false);
            Roommanager.LeaveRoom();
        }
        else if (GameManager.Instance._Wifi == WIFI.privateRoom)
        {
            gameObject.SetActive(false);
            Roommanager.LeaveRoom();
            if (GameManager.Instance.isCreateRoom)
                MMS.gameObject.SetActive(true);
            else
                JoinRoomPanel.SetActive(true);
        }
    }

    private int[] GetRandomInt(int length, int min, int max)
    {
        int[] randArray = new int[length];
        bool isSame;
        for (int i = 0; i < length; ++i)
        {
            while (true)
            {
                randArray[i] = UnityEngine.Random.Range(min, max);
                isSame = false;
                for (int j = 0; j < i; ++j)
                {
                    if (randArray[j] == randArray[i])
                    {
                        isSame = true;
                        break;
                    }
                }
                if (!isSame)
                    break;
            }
        }
        return randArray;
    }
}
