using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour
{

    public InputField usernameField;
    public InputField passwordField;
    public InputField confirmPasswordField;

    public Button submitButton;

    //When the user click on "Submit" button this function gets called
    public void CallRegister()
    {
        if (passwordField.text != confirmPasswordField.text) StartCoroutine(Notification.instance.showNotification("Unmatching passwords!"));
        else StartCoroutine(DoRegister());
    }

    IEnumerator DoRegister()
    {
        //Create an HTML form with the data
        WWWForm form = new WWWForm();
        form.AddField("class", "UsersController\\users");
        form.AddField("function", "create");
        form.AddField("username", usernameField.text);
        form.AddField("password", passwordField.text);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        //we display the success notification and put the received simulation data in the buffer
        if (www.text != "" && www.text[0] == '0')
        {

            //Login the user
            StartCoroutine(Login.instance.LoginPlayer(usernameField.text, passwordField.text));
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine( Notification.instance.showNotification(www.text) );
            else StartCoroutine( Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet.") );
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
