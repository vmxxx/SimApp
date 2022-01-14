using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginRegisterPanel : MonoBehaviour
{

    public GameObject mainMenu;
    public GameObject loginRegisterMenu;

    public void index()
    {
        mainMenu.SetActive(false);
        loginRegisterMenu.SetActive(true);
    }
}
