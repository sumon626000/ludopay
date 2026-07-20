using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferralCodeManager : MonoBehaviour
{
    public RoomManager roommanager;
    public MenuManager menuManager;
    public ProfileManager profileManage;
    public UIInput referral_code;
    public GameObject warning;

    public void Confirm()
    {
        if (string.IsNullOrEmpty(referral_code.value))
            return;
        roommanager.Check_Referral_Code(referral_code.value);
    }
    public void SetResult(string result)
    {
        if (result == "success")
        {
            //GameManager.Instance.Points += GameManager.Instance.ReferBonus;

            roommanager.UpdateUserInfo();
            //profileManage.Counting(GameManager.Instance.Points, GameManager.Instance.Points - GameManager.Instance.ReferBonus);
            Exit();
        }
        else
        {
            StartCoroutine(ShowWarning());
        }
    }
    IEnumerator ShowWarning()
    {
        warning.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        warning.SetActive(false);
    }
    public void Exit()
    {
        GetComponent<UIPopup>().Close();
    }

    private void OnDisable()
    {
        menuManager.On_Home();
    }

}
