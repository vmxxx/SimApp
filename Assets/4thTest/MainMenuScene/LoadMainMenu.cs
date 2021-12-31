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

        Debug.Log(Buffer.instance.currentSimulation.ID);
        Debug.Log(Buffer.instance.currentSimulation.name);
        Debug.Log(Buffer.instance.currentSimulation.image);
        Debug.Log(Buffer.instance.currentSimulation.description);
        Debug.Log(Buffer.instance.currentSimulation.likesCount);
        Debug.Log(Buffer.instance.currentSimulation.dislikesCount);
        Debug.Log(Buffer.instance.currentSimulation.authorID);


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
