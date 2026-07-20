using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LudoPawnController : MonoBehaviour
{
    public int moveSteps = 6;
    public List<Transform> real_paths = new List<Transform>(); 
    public Transform[] paths; 
    public List<Transform> goal_paths = new List<Transform>(); 
    public Transform path_quick;
    public int currentPath = 0;
    public int turnId = 0;
    public GameObject particle_arrive;
    public GameObject particle_capture;
    private GameObject particle_caputre_instance;
    [Space(10.0f)]
    public LudoRoundController RoundController;
    public Vector3 initPosition;
    float tweenScaleDuration = 0.1f;
    float tweenPositionDuration = 0.2f;
    public bool is6p = true;
    public Vector3 tokenScale = new Vector3(1.2f, 1.2f, 1.0f);
    public Vector3 tokenScale_init = new Vector3(1.0f, 1.0f, 1.0f);
    [Header("Arrow")]
    public Transform myArrowPath;

    private void Awake()
    {
        for (int i = paths.Length - 6; i < paths.Length; i++)
        {
            goal_paths.Add(paths[i]);
        }

        Init_Paths();
    }
    private void Init_Paths()
    {
        real_paths.Clear();
        if(GameManager.Instance._GameMode == GameMode.quick)
        {
            for (int i = 0; i < paths.Length - 6; i++)
            {
                real_paths.Add(paths[i]);
            }
        }
        else
        {
            for (int i = 0; i < paths.Length; i++)
            {
                real_paths.Add(paths[i]);
            }
        }
    }
    void Start()
    {
        initPosition = transform.localPosition;
        particle_arrive = Resources.Load("particle_arrive") as GameObject;
        particle_capture = Resources.Load("particle_capture") as GameObject;
        particle_caputre_instance = Instantiate(particle_capture, Vector3.zero, Quaternion.identity) as GameObject;
        particle_caputre_instance.transform.parent = transform;
        particle_caputre_instance.transform.localPosition = Vector3.zero;
        RoundController = GameObject.Find("LudoRoundController").GetComponent<LudoRoundController>();
        GetComponent<UIEventTrigger>().onClick.Add(new EventDelegate(StartMove));
        
        GetComponent<TweenScale>().duration = tweenScaleDuration;
        GetComponent<TweenPosition>().duration = tweenPositionDuration;

        if (is6p)
        {
            tokenScale = new Vector3(1.0f, 1.0f, 1.0f);
            tokenScale_init = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else
        {
            tokenScale = new Vector3(1.11f, 1.11f, 1.11f);
            tokenScale_init = new Vector3(1.06f, 1.06f, 1.0f);
        }
    }

    public void StartMove() 
    {
        if (RoundController.TurnId == turnId && RoundController.MoveSteps == 6)
        {
            if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
            {
                if (turnId == 0)
                {
                        for (int i = 0; i < RoundController.PlayerControllers[turnId].PawnsStatic.Count; i++)
                        {
                            if (RoundController.PlayerControllers[turnId].PawnsStatic[i] == gameObject)
                            {
                                RoundController.Move_StartToken_Event(i);
                                break;
                            }
                        }
                    
                }
                else
                    return;
            }
            RoundController.GetComponent<UIPlaySound>().audioClip = RoundController.token_start;
            RoundController.GetComponent<UIPlaySound>().Play();
            GameManager.Instance.Vibrating();
            transform.localScale = tokenScale;
            RoundController.PlayerControllers[turnId].ControlPawnsArray(gameObject);
            this.GetComponent<TweenPosition>().from = initPosition;
            this.GetComponent<TweenPosition>().to = real_paths[0].localPosition;
            this.GetComponent<TweenPosition>().duration = 0.2f;
            this.GetComponent<TweenPosition>().ResetToBeginning();
            this.GetComponent<TweenPosition>().PlayForward();
            real_paths[0].GetComponent<LudoPathController>().Pawns.Add(gameObject);
            this.GetComponent<UISprite>().depth = real_paths[0].GetComponent<LudoPathController>().PawnDepth;
            currentPath = 0;
            GetComponent<BoxCollider>().enabled = false;
            real_paths[0].GetComponent<LudoPathController>().CheckPawns();
            RoundController.Initialize_Dices();
        }

    }
    public void OnlineStartMove()
    {
        if (RoundController.TurnId == turnId && RoundController.MoveSteps == 6)
        {
            RoundController.GetComponent<UIPlaySound>().audioClip = RoundController.token_start;
            RoundController.GetComponent<UIPlaySound>().Play();
            GameManager.Instance.Vibrating();
            transform.localScale = tokenScale;
            RoundController.PlayerControllers[turnId].ControlPawnsArray(gameObject);
            this.GetComponent<TweenPosition>().from = initPosition;
            this.GetComponent<TweenPosition>().to = real_paths[0].localPosition;
            this.GetComponent<TweenPosition>().duration = 0.2f;
            this.GetComponent<TweenPosition>().ResetToBeginning();
            this.GetComponent<TweenPosition>().PlayForward();
            real_paths[0].GetComponent<LudoPathController>().Pawns.Add(gameObject);
            this.GetComponent<UISprite>().depth = real_paths[0].GetComponent<LudoPathController>().PawnDepth;
            currentPath = 0;
            GetComponent<BoxCollider>().enabled = false;
            real_paths[0].GetComponent<LudoPathController>().CheckPawns();
            RoundController.Initialize_Dices();
        }

    }
    public bool IsMovePossible()
    {
        int finalPaths = currentPath + RoundController.MoveSteps;
        if (GameManager.Instance._GameMode == GameMode.quick) 
        {
            if (finalPaths >= real_paths.Count) 
            {
                if (!RoundController.PlayerControllers[turnId].isCaptureToken)
                {
                    real_paths.Add(path_quick);
                    for (int i = 0; i < paths.Length - 6; i++)
                    {
                        real_paths.Add(paths[i]);
                    }
                }
                else 
                {
                    if (real_paths[real_paths.Count - 1].parent.gameObject.name != "Center")
                    {
                        for (int i = 0; i < goal_paths.Count; i++)
                        {
                            real_paths.Add(goal_paths[i]);
                        }
                    }
                }
                
            }
        }
        
        if (finalPaths < real_paths.Count)
            return true;
        else
            return false;
    }
    public void Move()
    {        
        moveSteps = RoundController.MoveSteps;
        RoundController.TurnOff(); 
        StartCoroutine(OnMove(moveSteps));        
    }
    IEnumerator OnMove(int steps = 0)
    {
        int startPath = currentPath + 1;
        int finalPath = startPath + steps - 1;
        
        if (finalPath < real_paths.Count)
        {
            for (int i = startPath; i < startPath + steps; i++)
            {
                currentPath++;
                RoundController.GetComponent<UIPlaySound>().audioClip = RoundController.token_walk;
                RoundController.GetComponent<UIPlaySound>().Play();
                this.GetComponent<TweenPosition>().from = transform.localPosition;
                this.GetComponent<TweenPosition>().to = real_paths[i].localPosition;
                this.GetComponent<TweenPosition>().duration = tweenPositionDuration;
                this.GetComponent<TweenPosition>().ResetToBeginning();
                this.GetComponent<TweenPosition>().PlayForward();
                this.GetComponent<TweenScale>().from = tokenScale;
                this.GetComponent<TweenScale>().to = new Vector3(1.5f, 1.5f, 1.0f);
                this.GetComponent<TweenScale>().duration = tweenScaleDuration;
                this.GetComponent<TweenScale>().ResetToBeginning();
                this.GetComponent<TweenScale>().PlayForward();
                yield return new WaitForSeconds(tweenScaleDuration);
                this.GetComponent<TweenScale>().from = new Vector3(1.5f, 1.5f, 1.0f);
                this.GetComponent<TweenScale>().to = tokenScale;
                this.GetComponent<TweenScale>().ResetToBeginning();
                this.GetComponent<TweenScale>().PlayForward();
                //yield return new WaitForSeconds(tweenPositionDuration);
                yield return new WaitForSeconds(tweenScaleDuration);
                transform.localRotation = real_paths[i].localRotation;
            }
            if (GameManager.Instance._GameMode == GameMode.arrow) 
            {
                if (real_paths[finalPath].GetComponent<LudoPathController>().isArrowStartPos) 
                {
                    int addMoveSteps = real_paths[finalPath].GetComponent<LudoPathController>().arrowSteps;
                    bool isPossibleMove = false;
                    if (addMoveSteps == 3)
                    {
                        if (real_paths[currentPath].gameObject == RoundController.PlayerControllers[turnId].arrowPath)
                        {
                            isPossibleMove = true;
                        }
                    }
                    else
                        isPossibleMove = true;
                    if (isPossibleMove)
                    {
                        GameManager.Instance.Vibrating();
                        finalPath = currentPath + addMoveSteps;
                        startPath = currentPath + 1;
                        
                        real_paths[finalPath].GetComponent<ArrowEffect>().On();

                        for (int i = startPath; i < startPath + addMoveSteps; i++)
                        {
                            currentPath++;
                            this.GetComponent<TweenPosition>().from = transform.localPosition;
                            this.GetComponent<TweenPosition>().to = real_paths[i].localPosition;
                            this.GetComponent<TweenPosition>().duration = tweenPositionDuration;
                            this.GetComponent<TweenPosition>().ResetToBeginning();
                            this.GetComponent<TweenPosition>().PlayForward();
                            yield return new WaitForSeconds(tweenPositionDuration);
                            transform.localRotation = real_paths[i].localRotation;
                        }
                        RoundController.MoveSteps = 6;
                    }
                    
                }
            }
        
            if (!real_paths[finalPath].GetComponent<LudoPathController>().IsStarPos)
            {
                if (real_paths[finalPath].GetComponent<LudoPathController>().Pawns.Count == 1) 
                {
                  
                    RoundController.PlayerControllers[turnId].isCaptureToken = true;
                    RoundController.PlayerControllers[turnId].transform.Find("QuickMode").gameObject.SetActive(false);
                    GameManager.Instance.Vibrating();
                    
                    List<GameObject> removeObjs = new List<GameObject>();
                    for (int j = 0; j < real_paths[finalPath].GetComponent<LudoPathController>().Pawns.Count; j++)
                    {
                        GameObject obj = real_paths[finalPath].GetComponent<LudoPathController>().Pawns[j];
                        if (obj.GetComponent<LudoPawnController>().turnId != turnId)
                        {
                            RoundController.GetComponent<UIPlaySound>().audioClip = RoundController.token_cut;
                            RoundController.GetComponent<UIPlaySound>().Play();
                            GameManager.Instance.Vibrating();
                            RoundController.PlayerControllers[turnId].captured_opponent++;
                            obj.transform.localScale = tokenScale_init;
                            obj.transform.localRotation = Quaternion.identity;
                            obj.GetComponent<LudoPawnController>().currentPath = 0;
                            obj.GetComponent<LudoPawnController>().Init_Paths();
                            obj.GetComponent<TweenPosition>().from = obj.transform.localPosition;
                            obj.GetComponent<TweenPosition>().to = obj.GetComponent<LudoPawnController>().initPosition;
                            obj.GetComponent<TweenPosition>().ResetToBeginning();
                            obj.GetComponent<TweenPosition>().PlayForward();
                            removeObjs.Add(obj);
                            RoundController.MoveSteps = 6;
                        }
                    }
                    if (removeObjs.Count > 0)
                    {
                        particle_caputre_instance.SetActive(true);
                        StartCoroutine(ResetParticle());
                        for (int k = 0; k < removeObjs.Count; k++)
                        {
                            removeObjs[k].GetComponent<BoxCollider>().enabled = true;
                            RoundController.PlayerControllers[removeObjs[k].GetComponent<LudoPawnController>().turnId].PawnsMoved.Remove(removeObjs[k]);
                            RoundController.PlayerControllers[removeObjs[k].GetComponent<LudoPawnController>().turnId].PawnsStatic.Add(removeObjs[k]);
                            real_paths[finalPath].GetComponent<LudoPathController>().Pawns.Remove(removeObjs[k]);
                            RoundController.PlayerControllers[removeObjs[k].GetComponent<LudoPawnController>().turnId].captured_mine++;
                        }
                    }
                }
            }
            else
            {
                RoundController.GetComponent<UIPlaySound>().audioClip = RoundController.star_path;
                RoundController.GetComponent<UIPlaySound>().Play();
            }
            real_paths[finalPath].GetComponent<LudoPathController>().Pawns.Add(gameObject);
            this.GetComponent<UISprite>().depth = real_paths[finalPath].GetComponent<LudoPathController>().PawnDepth;
            real_paths[finalPath].GetComponent<LudoPathController>().CheckPawns();
            
            if ((finalPath == real_paths.Count - 1) && (real_paths[real_paths.Count - 1].parent.gameObject.name == "Center"))
            {
                RoundController.GetComponent<UIPlaySound>().audioClip = RoundController.token_finish;
                RoundController.GetComponent<UIPlaySound>().Play();
                GameManager.Instance.Vibrating();
                GameObject particle_arrive_instance = Instantiate(particle_arrive, Vector3.zero, Quaternion.identity) as GameObject;
                particle_arrive_instance.transform.parent = transform;
                particle_arrive_instance.transform.localPosition = Vector3.zero;
                real_paths[finalPath].GetComponent<LudoPathController>().OnFinalPath();
            }
        }

        GameManager.Instance.Pawnmoved = false;
        RoundController.Initialize_Dices();
        
    }

    IEnumerator ResetParticle()
    {
        yield return new WaitForSeconds(3.0f);
        particle_caputre_instance.SetActive(false);
    }
    
}
