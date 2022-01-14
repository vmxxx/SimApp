using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelOpener : MonoBehaviour
{

    public GameObject Panel;

    public void OpenPanel()
    {
        if(Panel.activeSelf == false) { Panel.SetActive(true); }
        else { Panel.SetActive(false); }
    }
}
