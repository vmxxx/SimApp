using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPanel : MonoBehaviour
{

    public GameObject currentPanel;

    public void switchPanel(GameObject rightPanel)
    {
        rightPanel.SetActive(true);
        currentPanel.SetActive(false);
        currentPanel = rightPanel;
    }
}
