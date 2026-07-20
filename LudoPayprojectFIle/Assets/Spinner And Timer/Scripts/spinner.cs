using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinner : MonoBehaviour {

	public float reducer;
	public float multiplier = 0;
	bool round1 = false;
	public bool isStoped = false;

	AudioSource audioSource;
	int spinnerdelayvalue = 0;

	
    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        reducer = Random.Range(0.01f, 0.5f);
        audioSource.enabled = enabled;
        if (audioSource.enabled == true)
        {
            if (GameManager.Instance.settingData.sound == false)
                audioSource.enabled = false;
            else
                audioSource.enabled = true;
        }
    }
    private void OnDisable()
    {
        audioSource.enabled = false;
    }
    // Update is called once per frameQ
    void FixedUpdate () {
        
        if (Input.GetKey (KeyCode.Space)) 
		{
			Reset ();
		}

		if (multiplier > 0)
		{
			if(multiplier < 7)
			audioSource.pitch = multiplier / 2.0f;


			transform.Rotate (Vector3.forward, 1 * multiplier);
		} else
		{
			isStoped = true;
			audioSource.Stop ();
		}

		if (multiplier < 20 && !round1) 
		{
			multiplier += 0.1f;
		} else 
		{
			round1 = true;
		}

		if (round1 && multiplier > 0)
		{
			multiplier -= reducer;
		} 
	}


	public void Reset()
	{
        if (enabled != true)
            return;
        if (GameManager.Instance.wheelSpin == false)
            return;
		multiplier = 1;
		reducer = Random.Range (0.01f, 0.5f);
		round1 = false;
		isStoped = false;
		audioSource.Play ();
	}
}