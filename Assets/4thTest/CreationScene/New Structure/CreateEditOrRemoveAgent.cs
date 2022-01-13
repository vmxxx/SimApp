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
        refreshAgents();
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
            Debug.Log("0; Delete succesful");
        }
        else
        {
            Debug.Log("Loading agents failed. Error #" + www.text);
        }
    }

    public void save()
    {
        Debug.Log("Convert.ToBase64String(ImageConversion.EncodeToPNG((Texture2D)iconSetting.GetComponent<RawImage>().texture)): " + Convert.ToBase64String(ImageConversion.EncodeToPNG((Texture2D)iconSetting.GetComponent<RawImage>().texture)));
        StartCoroutine(saveAgent());
        SearchAgent.instance.clearAgents();
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
        //form.AddField("authorID", Buffer.instance.authenticatedUser.ID);
        form.AddField("authorID", 5);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {
            Debug.Log("0; Update succesful");
        }
        else
        {
            Debug.Log("Loading agents failed. Error #" + www.text);
        }
    }

    public void createAsNew()
    {
        StartCoroutine(createAgentAsNew());
        SearchAgent.instance.clearAgents();
    }

    private IEnumerator createAgentAsNew()
    {

        WWWForm form = new WWWForm();
        form.AddField("class", "AgentsController\\agents");
        form.AddField("function", "create");
        form.AddField("icon", Convert.ToBase64String(ImageConversion.EncodeToPNG((Texture2D)iconSetting.GetComponent<RawImage>().texture)));
        form.AddField("name", nameSetting.text);
        form.AddField("description", descriptionSetting.text);
        //form.AddField("authorID", Buffer.instance.authenticatedUser.ID);
        form.AddField("authorID", 5);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {
            Debug.Log("0; Insert succesful");
        }
        else
        {
            Debug.Log("Loading agents failed. Error #" + www.text);
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

        Debug.Log("Convert.FromBase64String(icon): " + Convert.FromBase64String(icon));
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
        Debug.Log("amountOfAgentsCurrentlyLoaded: " + amountOfAgentsCurrentlyLoaded);
        for (int i = 0; i < amountOfAgentsCurrentlyLoaded; i++)
        {
            GameObject temp = agentsList.transform.Find("Agent_" + i).gameObject;
            Debug.Log(temp);
            Destroy(temp);
        }
        //StartCoroutine(loadAgents());
    }

    public GameObject agentsList;
    /*
    public int minAmountOfEmptyAgentCells = 6;
    public GameObject emptyAgentCell;

    private void displayAgents()
    {

        GameObject[] agentCells = new GameObject[Buffer.instance.agents.Length];
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            agentCells[i] = Instantiate(emptyAgentCell);
            agentCells[i].transform.SetParent(agentsList.transform);
            agentCells[i].SetActive(true);
            agentCells[i].name = "Agent_" + i;
            agentCells[i].transform.Find("ID").GetComponent<Text>().text = Buffer.instance.agents[i].agentID.ToString();
            agentCells[i].transform.Find("Name").GetComponent<Text>().text = Buffer.instance.agents[i].agentName;
        }

    }

    /**/
}