using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class needle : MonoBehaviour {
	public spinner _spinner;
	public UILabel scoretext;
    public GameObject coincrash_effect;
    public RoomManager roommanager;
    public ProfileManager prf;
    public int index = 0;
    private void OnEnable()
    {
        if (GameManager.Instance.wheelSpin == true)
            index = 0;
        else
        {
            index = 1; prf.isAddCoins = false;
        }
    }
    void Start () {
        scoretext.text = "";
	}
    private void OnDisable()
    {
        coincrash_effect.SetActive(false);
    }
    void OnTriggerStay2D(Collider2D col){
		if (!_spinner.isStoped)
			return;
        scoretext.text = col.gameObject.name;
        if (index == 0)
        {
            GameManager.Instance.wheelSpin = false;
            GameManager.Instance.Points += int.Parse(scoretext.text);
            roommanager.UpdateUserInfo();
            coincrash_effect.SetActive(true);
            prf.isAddCoins = true;
            prf.spincoins = int.Parse(scoretext.text);
        }
        index++;
    }
}
