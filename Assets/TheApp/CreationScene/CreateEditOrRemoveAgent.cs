using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UI;

public class CreateEditOrRemoveAgent : MonoBehaviour
{

    public static CreateEditOrRemoveAgent instance;

    public InputField IDSetting;
    public GameObject iconSetting;
    public InputField nameSetting;
    public InputField descriptionSetting;

    private GameObject selectedAgent;
    private int amountOfAgentsCurrentlyLoaded;

    //1. Deleting an agent
    public void remove(Text IDSetting)
    {
        StartCoroutine(removeAgent(IDSetting));
    }

    private IEnumerator removeAgent(Text IDSetting)
    {
        //Create an HTML form with the data
        WWWForm form = new WWWForm();
        form.AddField("class", "AgentsController\\agents");
        form.AddField("function", "delete");
        form.AddField("ID", IDSetting.text);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        if (www.text != "" && www.text[0] == '0')
        {
            //Refresh the agent list
            SearchAgent.instance.clearAgents();
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }
    }

    //2. Saving an agent
    public void save()
    {
        if (iconSetting.GetComponent<RawImage>().texture != null && nameSetting.text != "" && descriptionSetting.text != "") { StartCoroutine(saveAgent()); SearchAgent.instance.clearAgents(); }
        else StartCoroutine(Notification.instance.showNotification("Agent name, icon and description cannot be NULL!"));
    }

    private IEnumerator saveAgent()
    {
        //Create an HTML form with the data
        WWWForm form = new WWWForm();
        form.AddField("class", "AgentsController\\agents");
        form.AddField("function", "update");
        form.AddField("ID", IDSetting.text);
        form.AddField("icon", Convert.ToBase64String(ImageConversion.EncodeToPNG((Texture2D)iconSetting.GetComponent<RawImage>().texture)));
        form.AddField("name", nameSetting.text);
        form.AddField("description", descriptionSetting.text);
        form.AddField("authorID", Buffer.instance.authenticatedUser.ID);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        if (www.text != "" && www.text[0] == '0')
        {
            StartCoroutine(Notification.instance.showNotification("Agent saved!"));

            //Refresh the agent list
            SearchAgent.instance.clearAgents();
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }
    }

    //3. Creating an agent
    public void createAsNew()
    {
        if(iconSetting.GetComponent<RawImage>().texture != null && nameSetting.text != "" && descriptionSetting.text != "") { StartCoroutine(createAgentAsNew()); }
        else StartCoroutine(Notification.instance.showNotification("Agent name, icon and description cannot be NULL!"));
    }

    private IEnumerator createAgentAsNew()
    {
        //Create an HTML form with the data
        WWWForm form = new WWWForm();
        form.AddField("class", "AgentsController\\agents");
        form.AddField("function", "create");
        form.AddField("icon", Convert.ToBase64String(ImageConversion.EncodeToPNG((Texture2D)iconSetting.GetComponent<RawImage>().texture)));
        form.AddField("name", nameSetting.text);
        form.AddField("description", descriptionSetting.text);
        form.AddField("authorID", Buffer.instance.authenticatedUser.ID);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        if (www.text != "" && www.text[0] == '0')
        {
            StartCoroutine(Notification.instance.showNotification("Agent created!"));

            //Refresh the agent list
            SearchAgent.instance.clearAgents();
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }
    }

    //3. Selecting an agent (to resave it)
    public void edit(GameObject agent)
    {

        int index = Int32.Parse(agent.name.Substring(6));

        //Get the data
        int agentID = Buffer.instance.agents[index].agentID;
        string icon = Buffer.instance.agents[index].icon;
        string agentName = Buffer.instance.agents[index].agentName;
        string agentDescription = Buffer.instance.agents[index].agentDescription;
        int authorID = Buffer.instance.agents[index].authorID;

        //Refrence ID in the invisible input field (so when we save, we can later reference this ID in SQL)
        IDSetting.GetComponent<InputField>().text = agentID.ToString();
        //Display its other current details
        iconSetting.GetComponent<RawImage>().texture = applyBase64StringAsTexture(icon);
        nameSetting.GetComponent<InputField>().text = agentName;
        descriptionSetting.GetComponent<InputField>().text = agentDescription;
    }

    public GameObject agentsList;

    private Texture2D applyBase64StringAsTexture(string textureString)
    {
        Texture2D newTexture = new Texture2D(1, 1);
        newTexture.LoadImage(Convert.FromBase64String(textureString));
        newTexture.Apply();
        return newTexture;
    }
}