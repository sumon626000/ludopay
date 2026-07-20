using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LudoPlayerController : MonoBehaviour
{
    [Header("AI")]
    public bool isAUTO = false;
    public UIToggle AutoModeToggle;
    [Space(15.0f)]
    public float limitTime = 0.0f;
    public bool isChecked = false;

    private LudoRoundController RoundController;
    public LudoDiceController DiceController;
    public GameObject UserProfilePanel;
    public UILabel NickNameLabel;
    public UITexture Photo;
    public UISprite Timer;
    public Transform Chat;
    public int TurnId = 0;
    public int MoveSteps = 0;
    public UILabel rankLabel;
    [Range(10, 40)]
    public float speed = 20.0f;
    public bool Paused = true;
    public int captured_opponent = 0;
    public int captured_mine = 0;
    private float bot_thinkTime = 0.8f;
    
    public bool isCaptureToken = false;
 
    public GameObject arrowPath;
 
    public GameObject TurnMark;
    public Transform Pawns;
    public List<GameObject> PawnsMoved = new List<GameObject>();
    public List<GameObject> PawnsStatic = new List<GameObject>();
    public List<GameObject> PawnsCompleted = new List<GameObject>();
    
    void Awake()
    {
        RoundController = GameObject.Find("LudoRoundController").GetComponent<LudoRoundController>();
        UserProfilePanel = GameObject.Find("UI Root/Camera/UserProfilePanel").gameObject;
        Photo = transform.Find("PhotoPanel").GetChild(0).GetComponent<UITexture>();
        Chat = transform.Find("Chat");
        Timer = transform.Find("Timer").GetChild(0).GetComponent<UISprite>();
        Timer.enabled = false;
        if (Timer.GetComponent<AudioSource>() != null) Timer.GetComponent<AudioSource>().Stop();
        NickNameLabel.modifier = UILabel.Modifier.ToUppercase;
        for (int i = 0; i < Pawns.childCount; i++)
        {
            PawnsStatic.Add(Pawns.GetChild(i).gameObject);
        }
        if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
        {
            if (AutoModeToggle != null)
                AutoModeToggle.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        StartCoroutine(UpdateCoroutine());
        
    }
    public void Initialize()
    {
        Timer.fillAmount = 1.0f;
        Timer.enabled = false;
        DiceController.isTurn = false;
        TurnMark.GetComponent<UISprite>().alpha = 1.0f;
        TurnMark.GetComponent<TweenAlpha>().enabled = false;
        Paused = true;
        OnDisable_PawnHighlight();
    }

    public void TurnOn()
    {
        print("TurnId" + TurnId);
        Timer.enabled = true;
        Timer.fillAmount = 1.0f;
        DiceController.isTurn = true;
        MoveSteps = 0;
        TurnMark.GetComponent<TweenAlpha>().from = 1.0f;
        TurnMark.GetComponent<TweenAlpha>().to = 0.5f;
        TurnMark.GetComponent<TweenAlpha>().duration = 0.3f;
        TurnMark.GetComponent<TweenAlpha>().enabled = true;
        Paused = false;
        Invoke("initChecked", 0.1f);
        DiceController.isRolled = false;
        DiceController.OnEnable_DiceInit();
    }
    void initChecked()
    {
        isChecked = false;
    }

    public void TurnOff()
    {
        Initialize();
        DiceController.OnDisable_DiceInit();
    }

    public void Dice_Init()
    {
        DiceController.DiceValue_Disable();
        MoveSteps = 0;

    }

    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            limitTime = isAUTO ? bot_thinkTime : 0.0f;
            if(!Paused)
                UpdateClock();
            yield return null;
        }
    }
    int x = 0;
    private void UpdateClock()
    {
        Timer.fillAmount -= 1 / speed * Time.deltaTime;
        
        if (!isChecked)
        {
            if (Timer.fillAmount > 0 && Timer.fillAmount < 0.15f)
            {
                if (x == 0)
                {
                    if (Timer.GetComponent<UIPlaySound>() != null) Timer.GetComponent<UIPlaySound>().Play();
                    x++;
                }
            }
            else { x = 0; }
            if (Timer.fillAmount <= limitTime)
            {
                if (!DiceController.isRolled && Timer.enabled)
                {
                    if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
                    {
                        if (TurnId == 0)
                        {
                            print("KKK");
                            DiceController.Dice_Roll();
                        }
                    }
                    else
                        DiceController.Dice_Roll();
                }

                
                if (DiceController.isRolled)
                {
                    if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
                    {
                        if (TurnId == 0)
                        {

                            isChecked = true;
                            CheckDice();
                        }
                    }
                    else
                    {
                        isChecked = true;
                        CheckDice();
                    }
                }
            }
        }
    }
    
    public void CheckDice()
    {

        StartCoroutine(CheckBoard());
         
    }
    IEnumerator CheckBoard()
    {
        yield return new WaitForSeconds(0.5f);
        print("Check tokens after waiting time passed");
    
        if (MoveSteps == 6)
        {
            if (PawnsMoved.Count == 0) 
            {
                PawnsStatic[0].GetComponent<LudoPawnController>().StartMove(); 
            }
            else
            {
                if (PawnsStatic.Count > 0)
                {
                    PawnsStatic[0].GetComponent<LudoPawnController>().StartMove();
                }
                else
                {
                    for (int i = 0; i < PawnsMoved.Count; i++)
                    {
                        if (PawnsMoved[i].GetComponent<LudoPawnController>().IsMovePossible())
                        {
                            int currentpath = PawnsMoved[i].GetComponent<LudoPawnController>().currentPath;
                            PawnsMoved[i].GetComponent<LudoPawnController>().real_paths[currentpath].GetComponent<LudoPathController>().OnClick();
                            break;
                        }
                    }
                    if (PawnsMoved.Count == 1)
                    {
                        if (!PawnsMoved[0].GetComponent<LudoPawnController>().IsMovePossible())
                        {
                            RoundController.Initialize_Dices();
                        }
                    }
                }
            }
        }
        else if (MoveSteps < 6)
        {
            if (PawnsMoved.Count == 0)
            {
                RoundController.Initialize_Dices();
            }
            else
            {
                bool isMove = false;
                for (int i = 0; i < PawnsMoved.Count; i++)
                {
                    if (PawnsMoved[i].GetComponent<LudoPawnController>().IsMovePossible())
                    {
                        isMove = true;
                        int currentpath = PawnsMoved[i].GetComponent<LudoPawnController>().currentPath;
                        PawnsMoved[i].GetComponent<LudoPawnController>().real_paths[currentpath].GetComponent<LudoPathController>().OnClick();
                        break;
                    }
                    else
                        isMove = false;
                }
                if (!isMove)
                {
                    RoundController.Initialize_Dices();
                }
            }
        }
    }
 
    public void CheckMove()
    {
       StartCoroutine(CheckingMove());
    }

    IEnumerator CheckingMove()
    {
        yield return new WaitForSeconds(0.5f);
        if (MoveSteps == 6)
        {
            if (PawnsMoved.Count == 0) 
            {
                print("start move");
                PawnsStatic[0].GetComponent<LudoPawnController>().StartMove();
            }
            else
            {
                for (int i = 0; i < PawnsStatic.Count; i++)
                {
                    OnCheck_PawnHighlight(PawnsStatic[i]);
                }
                for (int i = 0; i < PawnsMoved.Count; i++)
                {
                    if (PawnsMoved[i].GetComponent<LudoPawnController>().IsMovePossible())
                    {
                        OnCheck_PawnHighlight(PawnsMoved[i]);
                    }
                }
                if (PawnsMoved.Count > 0)
                {
                    bool isNoMove = false;
                    for (int i = 0; i < PawnsMoved.Count; i++)
                    {
                        if (!PawnsMoved[i].GetComponent<LudoPawnController>().IsMovePossible())
                        {
                            isNoMove = true;
                        }
                        else
                        {
                            isNoMove = false;
                            break;
                        }
                    }
                    if(isNoMove)
                        RoundController.Initialize_Dices();
                }
            }
        }
        else if (MoveSteps < 6)
        {
            if (PawnsMoved.Count == 0)
            {
                RoundController.Initialize_Dices();
            }
            else 
            {
                if (PawnsMoved.Count == 1)
                {
                    if (PawnsMoved[0].GetComponent<LudoPawnController>().IsMovePossible())
                    {
                        int currentpath = PawnsMoved[0].GetComponent<LudoPawnController>().currentPath;
                        PawnsMoved[0].GetComponent<LudoPawnController>().real_paths[currentpath].GetComponent<LudoPathController>().OnClick();
                    }
                    else
                    {
                        RoundController.Initialize_Dices();
                    }
                }
                else
                {
                    bool isMove = false;
                    for (int i = 0; i < PawnsMoved.Count; i++)
                    {
                        if (PawnsMoved[i].GetComponent<LudoPawnController>().IsMovePossible())
                        {
                            isMove = true;
                            break;
                        }
                        else
                            isMove = false;
                    }
                    if (!isMove)
                    {
                        RoundController.Initialize_Dices();
                    }
                    else
                    {
                        for (int i = 0; i < PawnsMoved.Count; i++)
                        {
                            if (PawnsMoved[i].GetComponent<LudoPawnController>().IsMovePossible())
                            {
                                OnCheck_PawnHighlight(PawnsMoved[i]);
                            }
                        }
                    }
                }
            }
            
        }
    }
    public bool isCheckInit()
    {
        bool isChecked = false;
        if (DiceController.isRolled == false && DiceController.isTurn == true)
            isChecked = true;
        return isChecked;
    }
    void OnCheck_PawnHighlight(GameObject obj)
    {
        if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
        {
            if (RoundController.TurnId != TurnId)
                return;
        }
        obj.transform.GetChild(0).gameObject.SetActive(true);
    }
    public void OnDisable_PawnHighlight()
    {
        for (int i = 0; i < Pawns.childCount; i++)
        {
            disablePawn(Pawns.GetChild(i).gameObject);
        }
        
    }
    void disablePawn(GameObject obj)
    {
        obj.transform.GetChild(0).gameObject.SetActive(false);
    }
    public void ControlPawnsArray(GameObject obj)
    {
        PawnsMoved.Add(obj);
        PawnsStatic.Remove(obj);
    }

    public void OnValueChange_AutoToggle()
    {
        isAUTO = AutoModeToggle.value;
        RoundController.ChangeAutoStatus(isAUTO);
    }
    bool buffAuto = false;
    public void Stopplay()
    {
        Paused = true;
        buffAuto = isAUTO;
        if (isAUTO)
        {
            isAUTO = false;
        }
    }
    public void ResumePlay()
    {
        Paused = false;
        isAUTO = buffAuto;
    }
    public void OnClick_UserDetail()
    {
        if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom || GameManager.Instance.Online_Bot_Mode)
        {
            for (int i = 0; i < RoundController.UserDetails.Count; i++)
            {
                if (RoundController.UserDetails[i].username == NickNameLabel.text)
                {
                    if (RoundController.UserDetails[i].userid == "")
                    {
                        string randomnum1 = "" + Random.Range(100000, 1000000);
                        string randomnum2 = "" + Random.Range(100000, 1000000);
                        string randomnum = randomnum1 + randomnum2;
                        RoundController.UserDetails[i].userid = randomnum;
                        RoundController.UserDetails[i].points = Random.Range(5000, 20000);
                        RoundController.UserDetails[i].level = Random.Range(3, 10);
                        RoundController.UserDetails[i].online_multiplayer.played = Random.Range(50, 200);
                        RoundController.UserDetails[i].online_multiplayer.won = Random.Range(25, 100);
                        RoundController.UserDetails[i].friend_multiplayer.played = Random.Range(50, 200);
                        RoundController.UserDetails[i].friend_multiplayer.won = Random.Range(25, 100);
                        RoundController.UserDetails[i].tokens_captured.mine = Random.Range(30, 100);
                        RoundController.UserDetails[i].tokens_captured.opponents = Random.Range(30, 100);
                        RoundController.UserDetails[i].won_streaks.current = 250;
                        RoundController.UserDetails[i].won_streaks.best = 1000;
                    }
                    UserProfilePanel.SetActive(true);
                    UserProfilePanel.GetComponent<UserProfiler>().SetUI(RoundController.UserDetails[i], Photo.mainTexture);
                    break;
                }
            }
        }
    }
}
