using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static System.Math;
using System.Text.RegularExpressions;

public class SearchAgent : MonoBehaviour
{
    //This variable is needed so that we can reference this class from outside other classes.
    public static SearchAgent instance;

    public InputField agentsSearchField;

    public GameObject agentsListPanel;

    public GameObject emptyAgentCell;

    //The Update() function gets called every new frame.
    //As soon as we detect that the instance is forgotten we have to reset it 
    //(It is forgotten every time the scripts get recompiled.
    //The scripts are recompiled so that the compiler can test the payoffFormulas we have entered in the payoff matrix by putting them in a testing file).
    public void Update()
    {
        if(SearchAgent.instance == null)
        {
            instance = this;
        }
    }

    //This function resets the search field to empty, then (re)loads the full agent list for the user (the same function that gets called before the first frame update)
    public void clearAgents()
    {
        removeAgents(agentsListPanel);
        agentsSearchField.text = "";
        LoadAgentList.instance.Start();
    }

    //This function will search the TOP 10 results which contain the substring in the search input field
    public void searchAgent()
    {
        if (agentsSearchField.text != "") StartCoroutine(searchAgents());
        else clearAgents();
    }

    IEnumerator searchAgents()
    {

        //Create an HTML form with the data
        WWWForm form = new WWWForm();
        form.AddField("class", "AgentsController\\agents");
        form.AddField("function", "read");
        form.AddField("scene", "creation");
        form.AddField("search", agentsSearchField.text);
        form.AddField("fullList", "true");
        form.AddField("onSearch", "true");
        form.AddField("authorID", Buffer.instance.authenticatedUser.ID);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        //we display the success notification and put the received simulation data in the buffer
        if (www.text != "" && www.text[0] == '0')
        {
            removeAgents(agentsListPanel);

            string temp = Regex.Match(www.text, @"0;(.*?){").Value;
            int matchCount = Int32.Parse(temp.Substring(2, temp.Length - 3));
            string[] seperate_entries = new string[matchCount];

            //Example: { agentID: 9, icon: "$#!@$%0^&!!!&^%", agentName: "Dove", agentDescription: "doves are cowardly", authorID: 1 } { agentID: 10, icon: "$#!@$%0^&!!!&^%", agentName: "Hawk", agentDescription: "hawks are aggresive", authorID: 1 } 
            Regex pattern = new Regex(@"\{(.*?)\}");

            //Find every agent object
            for (int i = 0; i < matchCount; i++)
            {
                Match match = pattern.Matches(www.text)[i];
                seperate_entries[i] = match.Value;
            }

            Buffer.instance.newAgentsArray(matchCount);
            //For each agent object 
            for (int i = 0; i < matchCount; i++)
            {

                string seperate_entry = seperate_entries[i];

                //Extract the data in string form
                string agentID = Regex.Match(seperate_entry, @"ID:(.*?),").Value;
                string icon = Regex.Match(seperate_entry, @"icon:(.*?),").Value;
                string agentName = Regex.Match(seperate_entry, @"name:(.*?),").Value;
                string agentDescription = Regex.Match(seperate_entry, @"description:(.*?),").Value;
                string authorID = Regex.Match(seperate_entry, @"authorID:(.*?)}").Value;

                //Convert numbers to their appropriate data types, let the strings stay as strings
                //Save the object we've just got in the buffer
                Buffer.instance.agents[i].agentID = Int32.Parse(agentID.Substring(3, agentID.Length - 4));
                Buffer.instance.agents[i].icon = icon.Substring(6, icon.Length - 8);
                Buffer.instance.agents[i].agentName = agentName.Substring(6, agentName.Length - 8);
                Buffer.instance.agents[i].agentDescription = agentDescription.Substring(13, agentDescription.Length - 15);
                Buffer.instance.agents[i].authorID = Int32.Parse(authorID.Substring(9, authorID.Length - 10));
            }

            displayAgents();
        }
        else
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }
    }


    private void removeAgents(GameObject panel)
    {
        foreach (Transform child in panel.transform)
        {
            if (child.name != "Agent") Destroy(child.gameObject);
        }
    }
    
    private void displayAgents()
    {

        GameObject[] agentCells = new GameObject[Buffer.instance.agents.Length];
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            agentCells[i] = Instantiate(emptyAgentCell);
            agentCells[i].transform.SetParent(agentsListPanel.transform);
            agentCells[i].SetActive(true);
            agentCells[i].name = "Agent_" + i;
            agentCells[i].transform.Find("ID").GetComponent<Text>().text = Buffer.instance.agents[i].agentID.ToString();
            agentCells[i].transform.Find("Icon").GetComponent<RawImage>().texture = applyBase64StringAsTexture(Buffer.instance.agents[i].icon);
            agentCells[i].transform.Find("Name").GetComponent<Text>().text = Buffer.instance.agents[i].agentName;
            agentCells[i].transform.Find("Icon").GetChild(0).Find("Name").GetComponent<Text>().text = Buffer.instance.agents[i].agentName;
            agentCells[i].transform.Find("Icon").GetChild(0).Find("Description").GetComponent<Text>().text = Buffer.instance.agents[i].agentDescription;
        }

    }

    private Texture2D applyBase64StringAsTexture(string textureString)
    {
        Texture2D newTexture = new Texture2D(1, 1);
        newTexture.LoadImage(Convert.FromBase64String(textureString));
        newTexture.Apply();
        return newTexture;
    }
}
