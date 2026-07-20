using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    private MenuManager menuManager;
    public RoomManager roommanager;
    private Transform header_tran;
    private UILabel username_lbl, userid_lbl;
    private UILabel points_lbl;    

    public UILabel Mypoints;
    public ImageDownload photo1;
    public ImageDownload photo2;
    public GameObject coin_effect;
    public bool isAddCoins;
    public int spincoins;

    void Awake()
    {
        menuManager = transform.parent.GetComponent<MenuManager>();
        roommanager = menuManager.transform.Find("RoomManager").GetComponent<RoomManager>();
        header_tran = transform.Find("Header");
        username_lbl = header_tran.Find("UserName").GetComponent<UILabel>();
        userid_lbl = header_tran.Find("UserID").GetComponent<UILabel>(); 
        points_lbl = header_tran.Find("Coins").GetChild(0).GetComponent<UILabel>();        
        GameManager.Instance.Online_Bot_Mode = false;
    }
    private void OnEnable()
    {
        username_lbl.text = GameManager.Instance.UserName;
        points_lbl.text = GameManager.Instance.Points.ToString("N0");
        userid_lbl.text = "ID:" + GameManager.Instance.UserID;

        if (GameManager.Instance.AvatarURL != "")
        {
            photo1.url = photo2.url = GameManager.Instance.AvatarURL;
            photo1.enabled = photo2.enabled = true;
            StartCoroutine(GetMyImage());
        }
        if (isAddCoins)
            Counting(GameManager.Instance.Points, GameManager.Instance.Points - spincoins);

        //StartCoroutine(CheckReferralBounce());
    }

    private void Update()
    {
        Mypoints.text = GameManager.Instance.Points.ToString("N0");
    }

    IEnumerator GetMyImage()
    {
        yield return new WaitForSeconds(2.0f);
        if (photo1.GetComponent<UIMaskedTexture>().mainTexture != GameManager.Instance.AvatarImage)
            GameManager.Instance.AvatarImage = photo1.GetComponent<UIMaskedTexture>().mainTexture;
    }
    //public void OnClick_Store(GameObject clickType)
    //{
    //    if(clickType.name == "PlusCoinButton")
    //        menuManager.On_Store(0);
    //    else
    //        menuManager.On_Store(1);
    //}
    public void IncreaseCoin()
    {
        GameManager.Instance.Vibrating();
        coin_effect.SetActive(false);
        coin_effect.SetActive(true);
        Invoke("Disable_Effect", 1.0f);
    }
    void Disable_Effect()
    {
        coin_effect.SetActive(false);
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
            Mypoints.text = ((int)current).ToString("N0");
            yield return null;
        }
        current = target;
        Mypoints.text = ((int)current).ToString("N0");
        IncreaseCoin();
    }
}
