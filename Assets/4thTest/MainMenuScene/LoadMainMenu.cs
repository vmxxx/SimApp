using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMainMenu : MonoBehaviour
{

    public GameObject loginRegisterButton;
    public GameObject createSimulationButton;
    public GameObject logoutButton;
    public GameObject profileSettingsButton;

    public void showBuffer()
    {
        Debug.Log("Buffer: ");
        //Debug.Log(authenticatedUser.ID);
        //Debug.Log(authenticatedUser.username);
        Debug.Log("AuthenticatedUser (ID): " + Buffer.instance.authenticatedUser.ID);
        Debug.Log("authenticatedUser (Username): " + Buffer.instance.authenticatedUser.username);
        Debug.Log("CurrentSimulation (ID): " + Buffer.instance.currentSimulation.ID);
        //Buffer.instance.printPayoffFormulas();
    }

    void Start()
    {
        if (Buffer.instance.authenticatedUser.ID != 0)
        {
            loginRegisterButton.SetActive(false);
            createSimulationButton.SetActive(true);
            logoutButton.SetActive(true);
            profileSettingsButton.SetActive(true);
        } 
        else
        {
            loginRegisterButton.SetActive(true);
            createSimulationButton.SetActive(false);
            logoutButton.SetActive(false);
            profileSettingsButton.SetActive(false);
        }
    }
}
