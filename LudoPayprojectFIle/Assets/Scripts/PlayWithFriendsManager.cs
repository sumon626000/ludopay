using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayWithFriendsManager : MonoBehaviour
{
    public GameObject JoinRoomPanel, CreateRoomPanel, JoinRoomLobbyPanel, searchPanel, Rommanager;
    void OnEnable()
    {
        GameManager.Instance._Wifi = WIFI.privateRoom;
    }

    public void Open_JoinRoomPanel()
    {
        JoinRoomPanel.SetActive(true);
        gameObject.SetActive(false);
    }
    public void Close_JoinRoomPanel()
    {
        JoinRoomPanel.SetActive(false);
        gameObject.SetActive(true);
    }
    public void Open_CreateRoomPanel()
    {
        gameObject.SetActive(false);
        CreateRoomPanel.SetActive(true);
    }
    public void Close_CreateRoomPanel()
    {
        CreateRoomPanel.SetActive(false);
        gameObject.SetActive(true);
    }

    public void CreateRoom()
    {
        Rommanager.GetComponent<RoomManager>().Join_Room();
        JoinRoomLobbyPanel.SetActive(false);
        searchPanel.SetActive(true);
    }
    public void Close_JoinRoomLobbyPanel()
    {
        Rommanager.GetComponent<RoomManager>().LeaveRoom();
        JoinRoomLobbyPanel.GetComponent<UIPopup>().Close();
        Invoke("Return", 0.4f);
    }
    void Return()
    {
        CreateRoomPanel.SetActive(true);
    }
}
