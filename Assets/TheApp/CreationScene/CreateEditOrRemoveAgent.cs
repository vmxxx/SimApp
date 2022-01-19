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

    public void remove(Text IDSetting)
    {
        StartCoroutine(removeAgent(IDSetting));
    }

    private IEnumerator removeAgent(Text IDSetting)
    {

        WWWForm form = new WWWForm();
        form.AddField("class", "AgentsController\\agents");
        form.AddField("function", "delete");
        form.AddField("ID", IDSetting.text);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {
            SearchAgent.instance.clearAgents();
        }
        else
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }
    }

    public void save()
    {
        if (iconSetting.GetComponent<RawImage>().texture != null && nameSetting.text != "" && descriptionSetting.text != "") { StartCoroutine(saveAgent()); SearchAgent.instance.clearAgents(); }
        else StartCoroutine(Notification.instance.showNotification("Agent name, icon and description cannot be NULL!"));
    }

    private IEnumerator saveAgent()
    {
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

        if (www.text != "" && www.text[0] == '0')
        {
            StartCoroutine(Notification.instance.showNotification("Agent saved!"));
            SearchAgent.instance.clearAgents();
        }
        else
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }
    }

    public void createAsNew()
    {
        if(iconSetting.GetComponent<RawImage>().texture != null && nameSetting.text != "" && descriptionSetting.text != "") { StartCoroutine(createAgentAsNew()); }
        else StartCoroutine(Notification.instance.showNotification("Agent name, icon and description cannot be NULL!"));
    }

    private IEnumerator createAgentAsNew()
    {
        WWWForm form = new WWWForm();
        form.AddField("class", "AgentsController\\agents");
        form.AddField("function", "create");
        form.AddField("icon", Convert.ToBase64String(ImageConversion.EncodeToPNG((Texture2D)iconSetting.GetComponent<RawImage>().texture)));
        form.AddField("name", nameSetting.text);
        form.AddField("description", descriptionSetting.text);
        form.AddField("authorID", Buffer.instance.authenticatedUser.ID);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {
            StartCoroutine(Notification.instance.showNotification("Agent created!"));
            SearchAgent.instance.clearAgents();
        }
        else
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }
    }

    public void edit(GameObject agent)
    {

        string temp = agent.name.Substring(6, agent.name.Length - 6);
        int index = Int32.Parse(temp);

        int agentID = Buffer.instance.agents[index].agentID;
        string icon = Buffer.instance.agents[index].icon;
        string agentName = Buffer.instance.agents[index].agentName;
        string agentDescription = Buffer.instance.agents[index].agentDescription;
        int authorID = Buffer.instance.agents[index].authorID;

        IDSetting.GetComponent<InputField>().text = agentID.ToString();

        Texture2D newTexture = new Texture2D(1, 1);
        newTexture.LoadImage(Convert.FromBase64String(icon));
        newTexture.Apply();
        iconSetting.GetComponent<RawImage>().texture = newTexture;
        nameSetting.GetComponent<InputField>().text = agentName;
        descriptionSetting.GetComponent<InputField>().text = agentDescription;
    }

    private void refreshAgents()
    {
        amountOfAgentsCurrentlyLoaded = Buffer.instance.agents.Length;
        for (int i = 0; i < amountOfAgentsCurrentlyLoaded; i++)
        {
            GameObject temp = agentsList.transform.Find("Agent_" + i).gameObject;
            Destroy(temp);
        }
    }

    public GameObject agentsList;
}