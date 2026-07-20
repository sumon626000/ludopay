using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowEffect : MonoBehaviour
{
    public GameObject effect;

    public void On()
    {
        effect.SetActive(false);
        effect.SetActive(true);
    }
}
