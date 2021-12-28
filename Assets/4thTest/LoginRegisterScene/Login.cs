using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{

    public GameObject mainMenu;
    public GameObject loginRegisterMenu; 

    public InputField usernameField;
    public InputField passwordField;

    public Button submitButton;
    public string username;


    public void CallLogin()
    {
        StartCoroutine(LoginPlayer());
    }

    IEnumerator LoginPlayer()
    {
        WWWForm form = new WWWForm();
        form.AddField("class", "UsersController\\users");
        form.AddField("function", "read");
        form.AddField("username", usernameField.text);
        form.AddField("password", passwordField.text);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if(www.text != "" && www.text[0] == '0')
        {
            username = usernameField.text;
            Debug.Log(www.text);
            GoToMainMenu();
        }
        else
        {
            Debug.Log("User login failed. Error #" + www.text);
        }

        //Debug.Log(www.text);

    }

    public void VerifyInputs()
    {
        submitButton.interactable = (usernameField.text.Length >= 8 && passwordField.text.Length >= 8);
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");

        if (username != null)
        {

            //GameObject loginRegisterButton = MainMenu.logoutButton;
            //Debug.Log(loginRegisterButton);
            //Buffer.instance.authenticatedUser.SetActive(false);
            Buffer.instance.authenticatedUser.ID = 5;
            Buffer.instance.authenticatedUser.username = username;

            //GameObject createSimulationButton = GameObject.Find("CreateSimulationButton");
            //MainMenu.createSimulationButton.SetActive(true);

            //GameObject logoutButton = GameObject.Find("LogoutButton");
            //MainMenu.logoutButton.SetActive(true);

        }
    }
}
