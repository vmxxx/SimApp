using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.EventSystems;

public class SwitchScene : MonoBehaviour
{

    //public Button testSceneButton;

    public void switchScene(string scene_name)
    {
        GameObject objectClicked = EventSystem.current.currentSelectedGameObject;
        if (objectClicked.name == "Edit" || objectClicked.name == "Run")
        {
            string simulationID = objectClicked.transform.parent.Find("ID").GetComponent<Text>().text;
            string simulationName = objectClicked.transform.parent.Find("Name").GetComponent<Text>().text;
            //Texture2D simulationImageTexture = (Texture2D)objectClicked.transform.parent.Find("Image").GetComponent<RawImage>().texture;
            //string simulationImage = (simulationImageTexture != null) ? Convert.ToBase64String( ImageConversion.EncodeToPNG(simulationImageTexture)) : "";
            string simulationLikesCount = objectClicked.transform.parent.Find("LikesCount").GetComponent<Text>().text;
            string simulationDislikesCount = objectClicked.transform.parent.Find("DislikesCount").GetComponent<Text>().text;
            string simulationDescription = objectClicked.transform.parent.Find("Description").GetComponent<Text>().text;
            /**/
            if (simulationID != "Text")
            {
                Buffer.instance.currentSimulation.ID = Int32.Parse(simulationID);
                Buffer.instance.currentSimulation.name = simulationName;
                //Buffer.instance.currentSimulation.image = simulationImage;
                Buffer.instance.currentSimulation.description = simulationDescription;
                Buffer.instance.currentSimulation.likesCount = Int32.Parse(simulationLikesCount);
                Buffer.instance.currentSimulation.dislikesCount = Int32.Parse(simulationDislikesCount);
            }
        }
        else if (objectClicked.name == "CreateSimulation")
        {
            Buffer.instance.currentSimulation.ID = 0;
        }


        SceneManager.LoadScene(scene_name);
    }
}
