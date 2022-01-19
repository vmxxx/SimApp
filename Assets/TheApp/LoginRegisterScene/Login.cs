using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System;

public class Login : MonoBehaviour
{

    public static Login instance;

    public GameObject mainMenu;
    public GameObject loginRegisterMenu; 

    public InputField usernameField;
    public InputField passwordField;

    public Button submitButton;

    public void Start()
    {
        if (Login.instance == null) instance = this;
    }

    public void CallLogin()
    {
        StartCoroutine(LoginPlayer(usernameField.text, passwordField.text));
    }

    public IEnumerator LoginPlayer(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("class", "UsersController\\users");
        form.AddField("function", "read");
        form.AddField("username", username);
        form.AddField("password", password);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {

            Regex pattern = new Regex(@"{(.*?)}");
            MatchCollection matches = pattern.Matches(www.text);
            Match match = matches[0];


            string ID = Regex.Match(match.Value, @"ID:(.*?),").Value;
            username = Regex.Match(match.Value, @"username:(.*?),").Value;
            string isAdmin = Regex.Match(match.Value, @"isAdmin:(.*?)}").Value;

            Buffer.instance.authenticatedUser.ID = Int32.Parse(ID.Substring(3, ID.Length - 4));
            Buffer.instance.authenticatedUser.username = username.Substring(10, username.Length - 12);
            Buffer.instance.authenticatedUser.isAdmin = (isAdmin.Substring(8, isAdmin.Length - 9) == "1") ? true : false;

            SceneManager.LoadScene("MainMenuScene");
        }
        else
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }
    }

    public void VerifyInputs()
    {
        submitButton.interactable = (usernameField.text.Length >= 8 && passwordField.text.Length >= 8);
    }
}
