using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingController2 : MonoBehaviour
{
    public ToggleController musicToggle;
    public ToggleController soundToggle;
    public ToggleController vibratToggle;
    public ToggleController notifiToggle;

    void Start()
    {
        musicToggle.Set(GameManager.Instance.settingData.music);
        soundToggle.Set(GameManager.Instance.settingData.sound);
        vibratToggle.Set(GameManager.Instance.settingData.vibration);
        notifiToggle.Set(GameManager.Instance.settingData.notification);
    }

    public void Exit()
    {
        GameManager.Instance.settingData.music = musicToggle.value;
        GameManager.Instance.settingData.sound = soundToggle.value;
        GameManager.Instance.settingData.vibration = vibratToggle.value;
        GameManager.Instance.settingData.notification = notifiToggle.value;
        GameManager.Instance.WriteSettingData();
        GetComponent<TweenPosition>().PlayReverse();
    }
}
