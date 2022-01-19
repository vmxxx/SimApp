using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class SetImage : MonoBehaviour
{
    private string path;
    private string extension;

    public void setImage(RawImage rawImage)
    {
        //Opens file explorer
        path = EditorUtility.OpenFilePanel("Select image", "", "");
        string extension = Path.GetExtension(path);
        Debug.Log("extension: " + extension);

        if (extension != ".jpg" && extension != ".bmp" && extension != ".png") StartCoroutine(Notification.instance.showNotification("The file must be of formats: .jpg, .bmp, .png"));
        else
        {
            //Sets the image as a texture on the game object
            StartCoroutine(getTexture(rawImage));
        }

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
