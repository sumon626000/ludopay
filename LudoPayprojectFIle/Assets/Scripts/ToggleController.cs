using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleController : MonoBehaviour
{
    private UIToggle Off;
    private UIToggle On;
    public bool value;

    private void Awake()
    {
        Off = transform.GetChild(0).GetComponent<UIToggle>();
        On = transform.GetChild(1).GetComponent<UIToggle>();
        On.onChange.Add(new EventDelegate(OnValueChange_On));
        Off.onChange.Add(new EventDelegate(OnValueChange_Off));
    }
    private void OnValueChange_On()
    {
        if (On.value == true)
        {
            value = false;
        }
    }
    private void OnValueChange_Off()
    {
        if (Off.value == true)
        {
            value = true;
        }
    }
    public void Set(bool val)
    {
        StartCoroutine(apply(val));
    }
    IEnumerator apply(bool val)
    {
        yield return new WaitForSeconds(0.1f);
        if (val == true)
        {
            Off.value = true;
            value = true;
        }
        else
        {
            On.value = true;
            value = false;
        }
    }
}
