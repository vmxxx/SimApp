using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SetImage : MonoBehaviour
{
    string path;
    public RawImage rawImage;

    public void setImage()
    {
        path = EditorUtility.OpenFilePanel("Show all images (.png) ", "", "png");
        StartCoroutine(getTexture());
    }

    IEnumerator getTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file:///" + path);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            rawImage.texture = myTexture;
        }
    }
}
