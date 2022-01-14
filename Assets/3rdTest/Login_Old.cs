using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login_old : MonoBehaviour
{

    public GameObject mainMenu;
    public GameObject loginRegisterMenu;
    public GameObject loginRegisterButton;
    public GameObject createSimulationButton;
    public GameObject logoutButton;

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

        /*
        if(www.text[0] == '0')
        {
            username = usernameField.text;
            Debug.Log(www.text);
        }
        else
        {
            Debug.Log("User login failed. Error #" + www.text);
        }
        */

        Debug.Log(www.text);

        GoToMainMenu();
    }

    public void VerifyInputs()
    {
        submitButton.interactable = (usernameField.text.Length >= 8 && passwordField.text.Length >= 8);
    }

    private void GoToMainMenu()
    {
        mainMenu.SetActive(true);
        loginRegisterMenu.SetActive(false);

        if(username != null)
        {
            loginRegisterButton.SetActive(false);
            createSimulationButton.SetActive(true);
            logoutButton.SetActive(true);
        }
    }
}
