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

        //Debug.Log("GAMEOBJECT NAME" + gameObject.name);

        //Debug.Log("SIMULATION IDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD");
        //Debug.Log("SIMULATION_ID: " + EventSystem.current.currentSelectedGameObject.transform.GetChild(0).name);
        GameObject objectClicked = EventSystem.current.currentSelectedGameObject;
        if (objectClicked.name == "Edit" || objectClicked.name == "Run")
        {
            string simulationID = objectClicked.transform.parent.GetChild(0).GetComponent<Text>().text;
            if (simulationID != "Text")
            {
                Buffer.instance.currentSimulation.ID = Int32.Parse(simulationID);
                //Set the name too
                //Set the image too
                //Set the description too
                //Set the likesCount too
                //Set the dislikesCount too
                //Set the authorID too
            }
        }
        else if (objectClicked.name == "CreateSimulation")
        {
            Buffer.instance.currentSimulation.ID = 0;
        }


        SceneManager.LoadScene(scene_name);
    }
}
