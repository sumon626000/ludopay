using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CircleHealth : MonoBehaviour {
	public Image bar;
	public RectTransform button;
	public float health;
	public float Divident;
	/// <summary>\
	/// thimer start
	/// </summary>
	DateTime currentDate;
	DateTime NowDate;
	public Text Mins;
	public Text Secs;

	int mins;
	int secs;

	public int minsDefine;
	public int secsDefine;

	int changeinseconds;

	public GameObject image;
	// Use this for initialization
	void Start () {
		image.SetActive(false);
		NowDate = System.DateTime.Now;
		changeinseconds = 0;
	}
	
	// Update is called once per frame
	void Update () {
		timeGet ();
		healthChange (health);
	}

	void healthChange(float healthValue){
		float amount = (healthValue / Divident);
		bar.fillAmount = amount;
	}
	void timeGet(){
		currentDate = System.DateTime.Now;
;
		TimeSpan difference = currentDate.Subtract(NowDate);
		//Debug.Log (difference);
		mins = minsDefine - (int)difference.Minutes;
		secs = secsDefine - (int)difference.Seconds;
		if((int) difference.Seconds > changeinseconds){
			health--;
			changeinseconds = (int)difference.Seconds;
		}
		if (mins < 0) {
		//	Mins.text = "00";
			image.SetActive(true);
			Secs.text = "00";
			// Application.LoadLevel ("Levelmenu");
		} else{
			/// uncomment and assign the mins field to get minutes countdown
//			if (mins < 10) {
//				Mins.text =  "0" + mins.ToString ();
//			} else {
//				Mins.text = mins.ToString ();
//			}
			if (secs < 10) {
				Secs.text = "0" + secs.ToString ();
			} else {
				Secs.text = secs.ToString ();
			}
		}
	}
}
