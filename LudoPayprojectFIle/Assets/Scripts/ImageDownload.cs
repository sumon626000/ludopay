using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageDownload : MonoBehaviour
{

    public string url;
    public bool pixelPerfect = true;
    public GameObject loading;
    Texture2D mTex;

    void OnEnable()
    {
        StartCoroutine("Download");
    }

    UITexture ut;
    UIMaskedTexture umt;

    IEnumerator Download()
    {
        loading.SetActive(true);
        yield return null;
        WWW www = new WWW(url);
        yield return www;
        mTex = www.texture;

        if (mTex != null)
        {
            loading.SetActive(false);
            if (GetComponent<UITexture>() != null)
            {
                ut = GetComponent<UITexture>();
                ut.mainTexture = mTex;
                if (pixelPerfect) ut.MakePixelPerfect();
            }
            else if (GetComponent<UIMaskedTexture>() != null)
            {
                umt = GetComponent<UIMaskedTexture>();
                umt.mainTexture = mTex;
                if (pixelPerfect) umt.MakePixelPerfect();
            }
     
        }
        www.Dispose();
    }

    void OnDestroy()
    {
        if (mTex != null) Destroy(mTex);
   
    }

}
