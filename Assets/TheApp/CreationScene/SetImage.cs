using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SetImage : MonoBehaviour
{
    string path;

    public void setImage(RawImage rawImage)
    {
        //Opens file explorer
        path = EditorUtility.OpenFilePanel("Select image", "", "");

        //Sets the image as a texture on the game object
        StartCoroutine(getTexture(rawImage));
    }

    IEnumerator getTexture(RawImage rawImage)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file:///" + path);

        //Sends a local request to the image folder directory
        yield return www.SendWebRequest();

        //Convert the .png or .jpg (or whatever kind of) file to a texture data type
        Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

        //Set the image/texture on the game object
        rawImage.texture = myTexture;
    }
}
