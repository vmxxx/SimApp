using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logout : MonoBehaviour
{
    public void logOut()
    {
        Buffer.instance.authenticatedUser = new User();
        SceneManager.LoadScene("MainMenuScene"); //Reload the scene so that his data (his simulations) are also hidden
    }
}
