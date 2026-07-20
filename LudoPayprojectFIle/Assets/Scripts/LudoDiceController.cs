using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LudoDiceController : MonoBehaviour
{
    public GameObject diceInit;
    private GameObject diceRoll;
    private GameObject diceValue;
    private LudoRoundController RoundController;
    public bool isTurn = false;
    public bool isRolled = false;
    public int value = 1;
    private List<int> diceHistory = new List<int>();


    void Awake()
    {
        diceRoll = transform.Find("diceRoll").gameObject;
        diceValue = transform.Find("diceValue").gameObject;
        RoundController = GameObject.Find("LudoRoundController").GetComponent<LudoRoundController>();
        OnDisable_DiceInit();
        Dice_Init();
        diceInit.GetComponent<UIEventTrigger>().onClick.Add(new EventDelegate(Dice_Roll));
    }
    private void Update()
    {
        if (GameManager.Instance._GamePlayType == GamePlayType.five || GameManager.Instance._GamePlayType == GamePlayType.six)
        {
            if (GameObject.Find("UI Root/Camera").GetComponent<GameUIController>().isOpenAnyPanel)
                diceRoll.GetComponent<SpriteRenderer>().enabled = false;
            else
                diceRoll.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
    public void Dice_Roll() 
    {
        if (isTurn && !isRolled)
        {
            if (diceRoll.activeSelf)
                return;
            diceInit.SetActive(false);
            diceRoll.SetActive(true);
            if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
            {
                if(RoundController.TurnId == 0)
                    RoundController.Roll_Event();
            }
        }
    }
    public void Dice_RandomValue() 
    {
        if (GameManager.Instance._Wifi != WIFI.online && GameManager.Instance._Wifi != WIFI.privateRoom)
        {
            value = Random.Range(1, 7);
      
            diceHistory.Add(value);
            if (diceHistory.Count > 2)
            {
                if (value == 6)
                {
                    bool isDoubleSix = false;
                    for (int i = 0; i < diceHistory.Count; i++)
                    {
                        if (diceHistory[i] == 6)
                            isDoubleSix = true;
                        else
                            break;
                    }
                    if (isDoubleSix)
                    {
                        value = Random.Range(1, 5);
                    }
                }
                diceHistory.Clear();
            }
        }
        RoundController.MoveSteps = value;
        diceValue.GetComponent<UISprite>().spriteName = string.Format("Dice{0}", value);
        diceValue.SetActive(true);
        RoundController.PlayerControllers[RoundController.TurnId].MoveSteps = value;
       
        RoundController.PlayerControllers[RoundController.TurnId].CheckMove();
        isRolled = true;
    }

    public void Dice_Init()
    {
        diceInit.SetActive(true); diceRoll.SetActive(false);
        diceValue.GetComponent<UISprite>().alpha = 1.0f;
        diceValue.SetActive(false);
        isRolled = false;
    }

    public void DiceValue_Disable()
    {
        diceInit.SetActive(false); diceRoll.SetActive(false);
        diceValue.SetActive(true);
        diceValue.GetComponent<UISprite>().alpha = 0.5f;
        isRolled = false;
    }
    
    public void NoneActive_DiceInit_Click()
    {
        diceInit.GetComponent<BoxCollider>().enabled = false;
    }
    public void Active_DiceInit_Click()
    {
        diceInit.GetComponent<BoxCollider>().enabled = true;
    }
    public void OnDisable_DiceInit()
    {
        diceInit.GetComponent<UISprite>().alpha = 0.5f;
    }
    public void OnEnable_DiceInit()
    {
        diceInit.GetComponent<UISprite>().alpha = 1.0f;
        Dice_Init();
    }

}
