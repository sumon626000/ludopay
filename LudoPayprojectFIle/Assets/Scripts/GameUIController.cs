using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    public bool isOpenAnyPanel = false;
    public GameObject ChatPanel;
    public GameObject SettingsPanel;
    public GameObject GameRulePanel;
    public GameObject ChatIcon1;
    public GameObject ChatIcon2;
    public GameObject Warning;
    private Transform start_transform;

    void Start()
    {
        if (GameManager.Instance._Wifi == WIFI.offline || GameManager.Instance._Wifi == WIFI.vsComputer)
        {
            ChatIcon1.SetActive(false);
            ChatIcon2.SetActive(false);
        }
        if (GameManager.Instance._Wifi == WIFI.online || GameManager.Instance._Wifi == WIFI.privateRoom)
            StartCoroutine(ReplayWarning());
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void OpenChat()
    {
        ChatPanel.GetComponent<TweenPosition>().PlayForward();
        isOpenAnyPanel = true;
    }
    public void CloseChat()
    {
        ChatPanel.GetComponent<TweenPosition>().PlayReverse();
        isOpenAnyPanel = false;
    }
    public void OpenSettings()
    {
        SettingsPanel.GetComponent<TweenPosition>().PlayForward();
        isOpenAnyPanel = true;
    }
    public void CloseSettings()
    {
        SettingsPanel.GetComponent<TweenPosition>().PlayReverse();
        isOpenAnyPanel = false;
    }
    public void OpenGameRule()
    {
        GameRulePanel.SetActive(true);
        GameRulePanel.GetComponent<TweenPosition>().PlayForward();
        isOpenAnyPanel = true;
    }
    public void CloseGameRule()
    {
        GameRulePanel.GetComponent<TweenPosition>().PlayReverse();
        isOpenAnyPanel = false;
    }

    IEnumerator ReplayWarning()
    {
        while (true)
        {
            Warning.GetComponent<TweenTransform>().ResetToBeginning();
            Warning.GetComponent<TweenTransform>().PlayForward();
            yield return new WaitForSeconds(300.0f);
        }
    }
}
