using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContestManager : MonoBehaviour
{
    private MenuManager menuManager;
    public RoomManager Roommanager;
    public GameObject WinnerListPanel;
    public Transform DailyGrid;
    public Transform WeeklyGrid;
    public Transform MonthlyGrid;
    public UISlider USlider;
    public GameObject item;

    private List<UserInfo> UserList = new List<UserInfo>();
    private List<UserInfo> UserList_ranked = new List<UserInfo>();
    private string type = "daily";
    
    void Start()
    {
        menuManager = transform.parent.GetComponent<MenuManager>();
    }
    public void OnExit()
    {
        GetComponent<UIPopup>().Close();
        Invoke("Return", 0.4f);
    }
    void Return()
    {
        menuManager.On_Home();
    }
    public void On_WinnerList()
    {
        WinnerListPanel.SetActive(true);
        gameObject.SetActive(false);
    }
    public void Off_WinnerList()
    {
        WinnerListPanel.SetActive(false);
        gameObject.SetActive(true);
    }
    private void OnEnable()
    {
        if (DailyGrid.gameObject.activeSelf)
            Daily();
        else if (WeeklyGrid.gameObject.activeSelf)
            Weekly();
        else
            Monthly();
    }
    public void Daily()
    {
        for (int i = 0; i < DailyGrid.childCount; i++)
        {
            Destroy(DailyGrid.GetChild(i).gameObject);
        }
        type = "daily";
        Roommanager.Request_RankList(type);
    }
    public void Weekly()
    {
        for (int i = 0; i < WeeklyGrid.childCount; i++)
        {
            Destroy(WeeklyGrid.GetChild(i).gameObject);
        }
        type = "week";
        Roommanager.Request_RankList(type);
    }
    public void Monthly()
    {
        for (int i = 0; i < MonthlyGrid.childCount; i++)
        {
            Destroy(MonthlyGrid.GetChild(i).gameObject);
        }
        type = "month";
        Roommanager.Request_RankList(type);
    }
    public void InitUI(JSONObject UserArray)
    {
        UserList.Clear(); UserList_ranked.Clear();
        for (int i = 0; i < UserArray.Count; i++)
        {
            JSONObject jsonItem = UserArray[i];
            string username = Global.JsonToString(jsonItem.GetField("username").ToString(), "\"");
            int points = int.Parse(Global.JsonToString(jsonItem.GetField("points").ToString(), "\""));
            int level = int.Parse(Global.JsonToString(jsonItem.GetField("level").ToString(), "\""));
            UserInfo info = new UserInfo();
            info.name = username;
            info.points = points;
            info.level = level;
            UserList.Add(info);
        }
        while (UserList.Count > 0)
        {
            float max = 0;
            int index = 0;
            for (int j = 0; j < UserList.Count; j++)
            {
                if (max < UserList[j].points)
                {
                    max = UserList[j].points;
                    index = j;
                }
            }
            UserList_ranked.Add(UserList[index]);
            UserList.RemoveAt(index);
        }
        Transform transf;
        if (type == "daily")
            transf = DailyGrid;
        else if (type == "week")
            transf = WeeklyGrid;
        else
            transf = MonthlyGrid;
        for (int i = 0; i < UserList_ranked.Count; i++)
        {
            print("@" + UserList_ranked[i].name + " @" + UserList_ranked[i].points + " @" + UserList_ranked[i].level);
            item.GetComponent<RankItem>().rank.text = (i + 1).ToString();
            item.GetComponent<RankItem>().SetBadge((i + 1));
            item.GetComponent<RankItem>().username.text = UserList_ranked[i].name;
            item.GetComponent<RankItem>().points.text = UserList_ranked[i].points.ToString("N0");
            NGUITools.AddChild(transf, item);
        }
        transf.GetComponent<UIGrid>().repositionNow = true;
        //DailyGrid.GetComponent<UIGrid>().Reposition();
    }
   
}
