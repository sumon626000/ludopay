using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopup : MonoBehaviour
{
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Open();
    }

    public void Open()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("PopupClose"))
            animator.Play("PopupOpen");
        else if(GetComponent<TweenAlpha>() != null)
        {
            GetComponent<TweenAlpha>().ResetToBeginning();
            GetComponent<TweenAlpha>().PlayForward();
            GetComponent<TweenScale>().ResetToBeginning();
            GetComponent<TweenScale>().PlayForward();
            GetComponent<TweenPosition>().ResetToBeginning();
            GetComponent<TweenPosition>().PlayForward();
            Invoke("OnFinish_Tween", 0.8f);
        }
    }
    
    public void Close()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("PopupOpen"))
            animator.Play("PopupClose");
        Invoke("DisableGameObject", 0.5f);
    }
    void DisableGameObject()
    {
        gameObject.SetActive(false);
    }

    public void OnFinish_Tween()
    {
        GetComponent<TweenPosition>().enabled = false;
    }
}
