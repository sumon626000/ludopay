using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingComponent : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine("Checking");
    }
    IEnumerator Checking()
    {
        yield return new WaitForSeconds(15.0f);
        gameObject.SetActive(false);
    }
}
