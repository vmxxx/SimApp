using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Registration_old : MonoBehaviour
{

    public InputField usernameField;
    public InputField passwordField;

    public Button submitButton;

    public void CallRegister()
    {
        StartCoroutine(Register());
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("class", "UsersController\\users");
        form.AddField("function", "create");
        form.AddField("username", usernameField.text);
        form.AddField("password", passwordField.text);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code
        if (www.text == "0; Registration succesful")
        {
            Debug.Log("User created succesfully.");

            //TEMP
            //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        else
        {
            Debug.Log("User creation failed. Error #" + www.text);
        }
    }

    public void VerifyInputs()
    {
        submitButton.interactable = (usernameField.text.Length >= 8 && passwordField.text.Length >= 8);
    }
}
