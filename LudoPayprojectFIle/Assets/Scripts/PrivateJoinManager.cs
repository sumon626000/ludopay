using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivateJoinManager : MonoBehaviour
{
    public UIInput PrivateRoomValue;
    public RoomManager Roommanager;
    public GameObject SearchPanel;

    private void OnEnable()
    {
        GameManager.Instance.isCreateRoom = false;
    }

    public void Join()
    {
        if (PrivateRoomValue.value != "" || PrivateRoomValue.value.Length > 7)
        {
            string str = PrivateRoomValue.value;
            str = str.Substring(7);            
            GameManager.Instance.RoomID = int.Parse(str);
            Roommanager.RequestRoomInfo();
        }
    }
    public void joinStart()
    {
        Roommanager.Join_Room();
        SearchPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
