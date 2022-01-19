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

    private void Start()
    {
        if (Login.instance == null) instance = this;
    }

    private void Update()
    {
        if (Login.instance == null) instance = this;
    }

    //When the user click on "Submit" button this function gets called
    public void CallLogin()
    {
        StartCoroutine(LoginPlayer(usernameField.text, passwordField.text));
    }

    public IEnumerator LoginPlayer(string username, string password)
    {
        //Create an HTML form with the data
        WWWForm form = new WWWForm();
        form.AddField("class", "UsersController\\users");
        form.AddField("function", "read");
        form.AddField("username", username);
        form.AddField("password", password);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        if (www.text != "" && www.text[0] == '0')
        {

            //Select the data set substring
            //Example: { ID: 1, username: "Ralf", isAdmin: "false"}
            Regex pattern = new Regex(@"{(.*?)}");
            //Find it in the www.text
            MatchCollection matches = pattern.Matches(www.text);
            //Find the first such match (the only one)
            Match match = matches[0];

            //Extract the data in string form
            string ID = Regex.Match(match.Value, @"ID:(.*?),").Value;
            username = Regex.Match(match.Value, @"username:(.*?),").Value;
            string isAdmin = Regex.Match(match.Value, @"isAdmin:(.*?)}").Value;

            //Convert numbers to their appropriate data types, let the strings stay as strings
            //Save the object we've just got in the buffer
            Buffer.instance.authenticatedUser.ID = Int32.Parse(ID.Substring(3, ID.Length - 4));
            Buffer.instance.authenticatedUser.username = username.Substring(10, username.Length - 12);
            Buffer.instance.authenticatedUser.isAdmin = (isAdmin.Substring(8, isAdmin.Length - 9) == "1") ? true : false;

            //Switch to main menu
            SceneManager.LoadScene("MainMenuScene");
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }
    }

    //This function gets automatically called every time the user changes a value in the input fields
    public void VerifyInputs()
    {
        //if the username and the password is less than 8 chars long: submitButton.greyedOut = true
        //else: submitButton.greyedOut = false
        submitButton.interactable = (usernameField.text.Length >= 8 && passwordField.text.Length >= 8);
    }
}
