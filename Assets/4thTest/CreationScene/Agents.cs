using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UI;

/*
public class Agents : MonoBehaviour
{
public InputField IDSetting;
public InputField iconSetting;
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
    StartCoroutine(saveAgent());
    refreshAgents();
}

private IEnumerator saveAgent()
{
    WWWForm form = new WWWForm();
    form.AddField("class", "AgentsController\\agents");
    form.AddField("function", "update");
    form.AddField("ID", IDSetting.text);
    form.AddField("icon", iconSetting.text);
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
    refreshAgents();
}

private IEnumerator createAgentAsNew()
{

    WWWForm form = new WWWForm();
    form.AddField("class", "AgentsController\\agents");
    form.AddField("function", "create");
    form.AddField("icon", iconSetting.text);
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

    Debug.Log(icon);
    Debug.Log(agentName);
    Debug.Log(agentDescription);

    iconSetting.GetComponent<UnityEngine.UI.Text>().text = icon;
    nameSetting.GetComponent<UnityEngine.UI.Text>().text = agentName;
    descriptionSetting.GetComponent<UnityEngine.UI.Text>().text = agentDescription;
    IDSetting.text = agentID.ToString();
    iconSetting.text = icon;
    nameSetting.text = agentName;
    descriptionSetting.text = agentDescription;

}

private void refreshAgents()
{
    amountOfAgentsCurrentlyLoaded = Buffer.instance.agents.Length;
    Debug.Log("amountOfAgentsCurrentlyLoaded: " + amountOfAgentsCurrentlyLoaded);
    for (int i = 0; i < amountOfAgentsCurrentlyLoaded; i++)
    {
        GameObject temp = myAgentsList.transform.Find("Agent_" + i).gameObject;
        Debug.Log(temp);
        Destroy(temp);
    }
    StartCoroutine(loadAgents());
}


public int minAmountOfEmptyAgentCells = 6;
public GameObject myAgentsList;
public GameObject emptyAgentCell;

public void Awake()
{
    Debug.Log("LoadAgents awake!");
    testFunction();
    refreshAgents();
}

public void displayAgents()
{
    GameObject[] agentCells = new GameObject[minAmountOfEmptyAgentCells];
    for (int i = 0; i < minAmountOfEmptyAgentCells; i++)
    {
        agentCells[i] = Instantiate(emptyAgentCell);
        agentCells[i].transform.SetParent(myAgentsList.transform);
        agentCells[i].SetActive(true);
        agentCells[i].name = "Agent_" + i;
    }

    Debug.Log("Buffer.instance.agents.Length - agentCells.Length: " + (Buffer.instance.agents.Length - agentCells.Length));
    GameObject[] extraCells = new GameObject[Buffer.instance.agents.Length - agentCells.Length];
    for (int i = 0; i < extraCells.Length; i++)
    {
        extraCells[i] = Instantiate(emptyAgentCell);
        extraCells[i].transform.SetParent(myAgentsList.transform);
        extraCells[i].SetActive(true);
        extraCells[i].name = "Agent_" + (agentCells.Length + i);
    }

    GameObject[] totalCells = new GameObject[Buffer.instance.agents.Length];
    agentCells.CopyTo(totalCells, 0);
    extraCells.CopyTo(totalCells, agentCells.Length);


    for (int i = 0; i < Buffer.instance.agents.Length; i++)
    {
        totalCells[i].transform.GetChild(0).GetComponent<Text>().text = Buffer.instance.agents[i].agentID.ToString();
        totalCells[i].transform.GetChild(1).GetComponent<Text>().text = Buffer.instance.agents[i].agentName;
    }

}

}
/**/