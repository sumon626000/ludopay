using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankItem : MonoBehaviour
{
    public UILabel rank;
    public GameObject badge;
    public UILabel username;
    public UILabel points;

    public void SetBadge(int ranknum)
    {
        if (ranknum < 4)
        {
            badge.SetActive(true);
            badge.GetComponent<UISprite>().spriteName = string.Format("Badges_{0}", ranknum);
        }
        else
        {
            badge.SetActive(false);
        }
    }
}
