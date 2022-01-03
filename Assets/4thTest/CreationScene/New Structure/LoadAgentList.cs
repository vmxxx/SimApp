using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UI;

public class LoadAgentList : MonoBehaviour
{
    public static LoadAgentList instance;

    public GameObject agentsList;
    public GameObject emptyAgentCell;

    private GameObject selectedAgent;
    private int amountOfAgentsCurrentlyLoaded;

    /*
    private void refreshAgents()
    {
        amountOfAgentsCurrentlyLoaded = Buffer.instance.agents.Length;
        //Debug.Log("amountOfAgentsCurrentlyLoaded: " + amountOfAgentsCurrentlyLoaded);
        for (int i = 0; i < amountOfAgentsCurrentlyLoaded; i++)
        {
            GameObject temp = myAgentsList.transform.Find("Agent_" + i).gameObject;
            Debug.Log(temp);
            Destroy(temp);
        }
        StartCoroutine(loadAgents());
    }
    /**/

    public void Awake()
    {
        instance = this;
        StartCoroutine(loadAgents());
        //refreshAgents();
    }


    IEnumerator loadAgents()
    {

        WWWForm form = new WWWForm();
        form.AddField("class", "AgentsController\\agents");
        form.AddField("function", "read");
        form.AddField("fullList", "true");
        form.AddField("onSearch", "false");
        form.AddField("authorID", Buffer.instance.authenticatedUser.ID);
        //form.AddField("authorID", 5);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {
            //int matchCount = (int)(www.text[2] - '0');
            string temp = Regex.Match(www.text, @"0;(.*?){").Value;
            int matchCount = Int32.Parse(temp.Substring(2, temp.Length - 3));
            Debug.Log(matchCount);
            string[] seperate_entries = new string[matchCount];

            Regex pattern = new Regex(@"\{(.*?)\}");
            for (int i = 0; i < matchCount; i++)
            {
                Match match = pattern.Matches(www.text)[i];
                seperate_entries[i] = match.Value;
            }

            Buffer.instance.newAgentsArray(matchCount);
            for (int i = 0; i < matchCount; i++)
            {

                string seperate_entry = seperate_entries[i];

                string agentID = Regex.Match(seperate_entry, @"ID:(.*?),").Value;
                string icon = Regex.Match(seperate_entry, @"icon:(.*?),").Value;
                string agentName = Regex.Match(seperate_entry, @"name:(.*?),").Value;
                string agentDescription = Regex.Match(seperate_entry, @"description:(.*?),").Value;
                string authorID = Regex.Match(seperate_entry, @"authorID:(.*?)}").Value;

                Buffer.instance.agents[i].agentID = Int32.Parse(agentID.Substring(3, agentID.Length - 4));
                Buffer.instance.agents[i].icon = icon.Substring(6, icon.Length - 8);
                Buffer.instance.agents[i].agentName = agentName.Substring(6, agentName.Length - 8);
                Buffer.instance.agents[i].agentDescription = agentDescription.Substring(13, agentDescription.Length - 15);
                Buffer.instance.agents[i].authorID = Int32.Parse(authorID.Substring(9, authorID.Length - 10));
            }

            Debug.Log("Agents initialized!");
            Debug.Log("Authed usr: " + Buffer.instance.authenticatedUser.ID);
            Debug.Log("Buffer.instance.agents.Length: " + Buffer.instance.agents.Length);
            Debug.Log("AGENTS LOADEDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD");
            Buffer.instance.printAgents();
            displayAgents();

        }
        else
        {
            Debug.Log("Loading agents failed. Error #" + www.text);
        }
    }

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

    /*
    private void displayAgents()
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
    /**/
}

/**/