using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMainMenu : MonoBehaviour
{

    public GameObject loginRegisterButton;
    public GameObject createSimulationButton;
    public GameObject logoutButton;
    public GameObject profileSettingsButton;

    //Start gets called before the first frame update on scene (re)load
    void Start()
    {
        //If guest user
        if (Buffer.instance.authenticatedUser.ID != 0)
        {
            loginRegisterButton.SetActive(false);
            createSimulationButton.SetActive(true);
            logoutButton.SetActive(true);
            profileSettingsButton.SetActive(true);
        } 
        else //If registered user
        {
            loginRegisterButton.SetActive(true);
            createSimulationButton.SetActive(false);
            logoutButton.SetActive(false);
            profileSettingsButton.SetActive(false);
        }
    }
}
