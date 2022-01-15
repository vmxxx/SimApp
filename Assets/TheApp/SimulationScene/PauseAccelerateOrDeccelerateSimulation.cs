using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseAccelerateOrDeccelerateSimulation : MonoBehaviour
{
    
    public void pauseUnpause()
    {
        GameObject buttonCLicked = EventSystem.current.currentSelectedGameObject;
        string buttonName = buttonCLicked.transform.GetChild(0).GetComponent<Text>().text;
        if (buttonName == "Start") { buttonName = "Pause"; RunSimulation.instance.paused = false; }
        else if (buttonName == "Pause") { buttonName = "Unpause"; RunSimulation.instance.paused = true; }
        else if (buttonName == "Unpause") { buttonName = "Pause"; RunSimulation.instance.paused = false; }
    }

    public void restart(GameObject pauseButton)
    {
        pauseButton.transform.GetChild(0).GetComponent<Text>().text = "Start"; RunSimulation.instance.paused = true;
    }

    public void accelerate(GameObject speedText)
    {
        RunSimulation.instance.speed = RunSimulation.instance.speed * 2;
        speedText.GetComponent<Text>().text = "Speed: " + ((float)RunSimulation.instance.speed / 50f);
    }

    public void deccelerate(GameObject speedText)
    {
        RunSimulation.instance.speed = RunSimulation.instance.speed / 2;
        speedText.GetComponent<Text>().text = "Speed: " + ((float)RunSimulation.instance.speed / 50f);
    }
    /**/
}
