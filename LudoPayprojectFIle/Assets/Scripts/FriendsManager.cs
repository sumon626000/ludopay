using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsManager : MonoBehaviour
{
    private MenuManager menuManager;

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
}
