using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LudoPathController : MonoBehaviour
{
    public List<GameObject> Pawns = new List<GameObject>();
    public int PawnDepth = 2;
    public bool IsStarPos = false;
    public LudoRoundController RoundController;
    List<float[]> deltaWidth = new List<float[]>();
    float[] reducePawnDeltaX2 = new float[2] {-10, 10};
    float[] reducePawnDeltaX3 = new float[3] { -10, 0, 10 };
    float[] reducePawnDeltaX4 = new float[4] { -10, -3.5f, 3.5f, 10 };
    float[] reducePawnDeltaX5 = new float[5] { -10, -5, 0, 5, 10 };
    float[] reducePawnDeltaX6 = new float[6] { -12, -7.5f, -2.5f, 2.5f, 7.5f, 12 };
    float[] reducePawnDeltaX7 = new float[7] { -12, -8.5f, -4, 0, 4, 8.5f, 12 };
   
    public bool isArrowStartPos = false;

    public int arrowSteps = 1;
    public int arrowID = 10;
    void Start()
    {
        RoundController = GameObject.Find("LudoRoundController").GetComponent<LudoRoundController>();
        deltaWidth.Add(reducePawnDeltaX2);
        deltaWidth.Add(reducePawnDeltaX3);
        deltaWidth.Add(reducePawnDeltaX4);
        deltaWidth.Add(reducePawnDeltaX5);
        deltaWidth.Add(reducePawnDeltaX6);
        deltaWidth.Add(reducePawnDeltaX7);
    }

    public void OnClick()
    {
        if (GameManager.Instance.Pawnmoved == true)
            return;
        if (Pawns.Count == 0)
            return;
        if (RoundController.MoveSteps == 0)
            return;
        
        if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
        {
            bool ischeckpawn = false;
            foreach (GameObject pawn in Pawns)
            {
                if (pawn.GetComponent<LudoPawnController>().turnId != 0)
                    ischeckpawn = false;
                else
                {
                    ischeckpawn = true;
                    break;
                }
            }
            if (ischeckpawn == false)
                return;
        }
        foreach (GameObject pawn in Pawns) 
        {
            if (RoundController.TurnId == pawn.GetComponent<LudoPawnController>().turnId) 
            {
                if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
                {
                    if (pawn.GetComponent<LudoPawnController>().turnId == 0)
                    {
                        if (pawn.GetComponent<LudoPawnController>().IsMovePossible()) 
                        {
                            RoundController.PlayerControllers[pawn.GetComponent<LudoPawnController>().turnId].GetComponent<LudoPlayerController>().OnDisable_PawnHighlight();
                            pawn.transform.localScale = pawn.GetComponent<LudoPawnController>().tokenScale;
                            pawn.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
                            pawn.GetComponent<UISprite>().depth = 3;
                            GameManager.Instance.Pawnmoved = true;
                            pawn.GetComponent<LudoPawnController>().Move();
                            Pawns.Remove(pawn);
                            CheckPawns();
                            if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
                            {
                                if (RoundController.TurnId == 0)
                                {
                                    List<GameObject> moved = RoundController.PlayerControllers[0].PawnsMoved;
                                    for (int i = 0; i < moved.Count; i++)
                                    {
                                        if (moved[i] == pawn)
                                        {
                                            RoundController.Move_Token_Event(i);
                                            break;
                                        }
                                    }
                                }
                            }
                            
                            break;
                        }
                    }
                }
                else if (pawn.GetComponent<LudoPawnController>().IsMovePossible()) 
                {
                    RoundController.PlayerControllers[pawn.GetComponent<LudoPawnController>().turnId].GetComponent<LudoPlayerController>().OnDisable_PawnHighlight();
                    pawn.transform.localScale = pawn.GetComponent<LudoPawnController>().tokenScale;
                    pawn.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
                    pawn.GetComponent<UISprite>().depth = 3;
                    if (GameManager.Instance.Pawnmoved == false)
                    {
                        GameManager.Instance.Pawnmoved = true;
                        pawn.GetComponent<LudoPawnController>().Move();
                    }                        
                    Pawns.Remove(pawn);
                    CheckPawns();
                    break;
                }    
            }
        }
        
        if (Pawns.Count == 1) 
        {
            Pawns[0].transform.localScale = Pawns[0].GetComponent<LudoPawnController>().tokenScale;
            Pawns[0].transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            Pawns[0].GetComponent<UISprite>().depth = PawnDepth;
        }
    }
    public void OnlineClick()
    {
        if (GameManager.Instance.Pawnmoved == true)
            return;
        if (Pawns.Count == 0)
            return;
        if (RoundController.MoveSteps == 0)
            return;
        foreach (GameObject pawn in Pawns)
        {
            if (RoundController.TurnId == pawn.GetComponent<LudoPawnController>().turnId)
            {
                print("move1");
                if (pawn.GetComponent<LudoPawnController>().IsMovePossible())
                {
                    print("move2");
                    RoundController.PlayerControllers[pawn.GetComponent<LudoPawnController>().turnId].GetComponent<LudoPlayerController>().OnDisable_PawnHighlight();
                    pawn.transform.localScale = pawn.GetComponent<LudoPawnController>().tokenScale;
                    pawn.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
                    pawn.GetComponent<UISprite>().depth = 3;
                    GameManager.Instance.Pawnmoved = true;
                    pawn.GetComponent<LudoPawnController>().Move();
                    Pawns.Remove(pawn);
                    CheckPawns();
                    break;
                }
            }
        }
        if (Pawns.Count == 1)
        {
            Pawns[0].transform.localScale = Pawns[0].GetComponent<LudoPawnController>().tokenScale;
            Pawns[0].transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            Pawns[0].GetComponent<UISprite>().depth = PawnDepth;
        }
    }
    public void CheckPawns()
    {
        StartCoroutine("Check_Pawns");
    }
    IEnumerator Check_Pawns()
    {
        yield return new WaitForSeconds(0.2f);
        if (Pawns.Count > 1)
        {
            int index = Pawns.Count - 2;
            for (int i = 0; i < Pawns.Count; i++)
            {
                Pawns[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                Pawns[i].transform.localPosition = new Vector3(transform.localPosition.x + deltaWidth[index][i], transform.localPosition.y, 0);
                
            }
        }
        else if (Pawns.Count == 1)
        {
            Pawns[0].transform.localScale = Pawns[0].GetComponent<LudoPawnController>().tokenScale; 
            Pawns[0].transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            Pawns[0].GetComponent<UISprite>().depth = PawnDepth;
        }
    }
    public void OnFinalPath()
    {
        for (int i = 0; i < Pawns.Count; i++)
        {
            RoundController.PlayerControllers[Pawns[i].GetComponent<LudoPawnController>().turnId].PawnsMoved.Remove(Pawns[i]);
        }
        if (GameManager.Instance._GameMode == GameMode.quick)
        {
            int winner = Pawns[0].GetComponent<LudoPawnController>().turnId;
            RoundController.OnRank(winner);
        }
        else
        {
            if (RoundController.PlayerControllers[Pawns[0].GetComponent<LudoPawnController>().turnId].PawnsMoved.Count == 0 &&
                RoundController.PlayerControllers[Pawns[0].GetComponent<LudoPawnController>().turnId].PawnsStatic.Count == 0)
            {
                int winner = Pawns[0].GetComponent<LudoPawnController>().turnId;
                RoundController.OnRank(winner);
            }
            else
            {
                RoundController.MoveSteps = 6;
            }
        }
    }
}
