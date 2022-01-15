using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPanel : MonoBehaviour
{

    public GameObject currentPanel;

    public void switchPanel(GameObject rightPanel)
    {
        if (rightPanel != currentPanel)
        {
            rightPanel.SetActive(true);
            currentPanel.SetActive(false);
            currentPanel = rightPanel;
        }
    }
}
