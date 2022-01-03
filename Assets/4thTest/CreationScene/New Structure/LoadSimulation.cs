using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static System.Math;
using System.Text.RegularExpressions;

public class LoadSimulation : MonoBehaviour
{

    public GameObject payoffMatrixPanel;
    public GameObject emptyCell;

    public InputField nameInputField;
    public InputField imageInputField;
    public InputField descriptionInputField;

    private int amountOfFormulas;
    private int amountOfAgents;

    public void Start()
    {
        StartCoroutine(loadSimulation());
    }

    IEnumerator loadSimulation()
    {
        WWWForm form = new WWWForm();
        form.AddField("class", "SimulationsController\\simulations");
        form.AddField("function", "read");
        form.AddField("scene", "creation");
        form.AddField("ID", Buffer.instance.currentSimulation.ID);
        //form.AddField("authorID", Buffer.instance.authenticatedUser.ID);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {
            Debug.Log("0; Load succesful;" + www.text);

            Regex pattern = new Regex(@"{(.*?)}");
            MatchCollection matches = pattern.Matches(www.text);

                Match match = matches[0];

                string ID = Regex.Match(match.Value, @"ID:(.*?),").Value;
                string name = Regex.Match(match.Value, @"name:(.*?),").Value;
                string image = Regex.Match(match.Value, @"image:(.*?),").Value;
                string description = Regex.Match(match.Value, @"description:(.*?),").Value;
                string likesCount = Regex.Match(match.Value, @"likesCount:(.*?),").Value;
                string dislikesCount = Regex.Match(match.Value, @"dislikesCount:(.*?),").Value;
                string authorID = Regex.Match(match.Value, @"authorID:(.*?)}").Value;

                Buffer.instance.currentSimulation.ID = Int32.Parse(ID.Substring(3, ID.Length - 4));
                Buffer.instance.currentSimulation.name = name.Substring(6, name.Length - 8);
                //Buffer.instance.currentSimulation.image = image.Substring(6, image.Length - 7);
                Buffer.instance.currentSimulation.description = description.Substring(13, description.Length - 15);
                Buffer.instance.currentSimulation.likesCount = Int32.Parse(likesCount.Substring(11, likesCount.Length - 12));
                Buffer.instance.currentSimulation.dislikesCount = Int32.Parse(dislikesCount.Substring(14, dislikesCount.Length - 15));
                Buffer.instance.currentSimulation.authorID = Int32.Parse(authorID.Substring(9, authorID.Length - 10));
        }
        else
        {
            Debug.Log("Loading simulations failed. Error #" + www.text);
        }


        form = new WWWForm();
        form.AddField("class", "PayoffFormulasController\\payoffFormulas");
        form.AddField("function", "read");
        form.AddField("scene", "creation");
        form.AddField("simulationID", Buffer.instance.currentSimulation.ID);
        //form.AddField("authorID", Buffer.instance.authenticatedUser.ID);
        string agentsArray = "";

        www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        int i;
        if (www.text != "" && www.text[0] == '0')
        {
            Debug.Log("0; Load succesful;" + www.text);
            amountOfFormulas = Int32.Parse(www.text.Substring(2, 1));
            amountOfAgents = (int)Sqrt(amountOfFormulas);


            Regex pattern = new Regex(@"{(.*?)}");
            Buffer.instance.payoffFormulas = new Dictionary<(int, int), PayoffFormula>();
            MatchCollection matches = pattern.Matches(www.text);
            Debug.Log("Matches.Counttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt:" + matches.Count);
            for (i = 0; i < matches.Count; i++)
            {
                

                Match match = matches[i];

                Buffer.instance.newFormula = new PayoffFormula();

                string ID = Regex.Match(match.Value, @"ID:(.*?),").Value;
                string agent1 = Regex.Match(match.Value, @"agent1:(.*?),").Value;
                string agent2 = Regex.Match(match.Value, @"agent2:(.*?),").Value;
                string payoffFormula = Regex.Match(match.Value, @"payoffFormula:(.*?),").Value;
                string simulationID = Regex.Match(match.Value, @"simulationID:(.*?)}").Value;

                Buffer.instance.newFormula.ID = Int32.Parse(ID.Substring(3, ID.Length - 4));
                Buffer.instance.newFormula.agent1 = Int32.Parse(agent1.Substring(7, agent1.Length - 8));
                Buffer.instance.newFormula.agent2 = Int32.Parse(agent2.Substring(7, agent2.Length - 8));
                Buffer.instance.newFormula.payoffFormula = payoffFormula.Substring(15, payoffFormula.Length - 17);
                Debug.Log("simulationID.Length: " + simulationID.Length);
                Buffer.instance.newFormula.authorID = Int32.Parse(simulationID.Substring(13, simulationID.Length - 14));

                Buffer.instance.payoffFormulas.Add((Buffer.instance.newFormula.agent1, Buffer.instance.newFormula.agent2), Buffer.instance.newFormula);


                if (i < amountOfAgents)
                {
                    agentsArray = agentsArray + agent2.Substring(7, agent2.Length - 7);
                }
            }

            agentsArray = agentsArray.Substring(0, agentsArray.Length - 1);
            Debug.Log("agentsArray: " + agentsArray);
        }
        else
        {
            Debug.Log("Loading simulations failed. Error #" + www.text);
        }


        form = new WWWForm();
        form.AddField("class", "AgentsController\\agents");
        form.AddField("function", "read");
        form.AddField("fullList", "false");
        form.AddField("simulationID", Buffer.instance.currentSimulation.ID);
        form.AddField("IDsArray", agentsArray);
        //form.AddField("authorID", Buffer.instance.authenticatedUser.ID);

        www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {
            Buffer.instance.newAgentsArray(amountOfAgents);

            Regex pattern = new Regex(@"{(.*?)}");
            MatchCollection matches = pattern.Matches(www.text);
            for (i = 0; i < amountOfAgents; i++)
            {

                Match match = matches[i];

                string ID = Regex.Match(match.Value, @"ID:(.*?),").Value;
                string icon = Regex.Match(match.Value, @"icon:(.*?),").Value;
                string agentName = Regex.Match(match.Value, @"name:(.*?),").Value;
                string agentDescription = Regex.Match(match.Value, @"description:(.*?),").Value;
                string authorID = Regex.Match(match.Value, @"authorID:(.*?)}").Value;

                Buffer.instance.agents[i].agentID = Int32.Parse(ID.Substring(3, ID.Length - 4));
                Buffer.instance.agents[i].icon = icon.Substring(5, icon.Length - 6);
                Buffer.instance.agents[i].agentName = agentName.Substring(5, agentName.Length - 6);
                Buffer.instance.agents[i].agentDescription = agentDescription.Substring(12, agentDescription.Length - 13);
                Buffer.instance.agents[i].authorID = Int32.Parse(authorID.Substring(9, authorID.Length - 10));
            }

            Debug.Log("0; Load succesful;" + www.text);
        }
        else
        {
            Debug.Log("Loading simulations failed. Error #" + www.text);
        }

        Debug.Log(Buffer.instance.currentSimulation.name);
        nameInputField.text = Buffer.instance.currentSimulation.name;
        imageInputField.text = "Temporary image name"; //Buffer.instance.currentSimulation.image
        descriptionInputField.text = Buffer.instance.currentSimulation.description;

        for (i = 1; i < amountOfAgents; i++) { PayoffMatrix.instance.addExtra(); }

        Debug.Log("Buffer.instance.payoffFormulas.Count: " + Buffer.instance.payoffFormulas.Count);
        Buffer.instance.printPayoffFormulas();

        i = 0;
        for (int j = 1; j <= amountOfAgents; j++)
        {
            for (int k = 1; k <= amountOfAgents; k++)
            {
                int jj = Buffer.instance.agents[j - 1].agentID;
                int kk = Buffer.instance.agents[k - 1].agentID;

                PayoffMatrix.instance.tableCells[j, k].transform.Find("Formula").GetComponent<InputField>().text = Buffer.instance.payoffFormulas[(jj, kk)].payoffFormula;
                i = i + 1;
            }



            string agentID = Buffer.instance.agents[j - 1].agentID.ToString();
            string agentName = Buffer.instance.agents[j - 1].agentName;
            PayoffMatrix.instance.tableCells[j, 0].transform.Find("AgentID").GetComponent<Text>().text = agentID;
            PayoffMatrix.instance.tableCells[j, 0].transform.Find("Button").GetChild(0).GetComponent<Text>().text = agentName;
            PayoffMatrix.instance.tableCells[0, j].transform.Find("AgentID").GetComponent<Text>().text = agentID;
            PayoffMatrix.instance.tableCells[0, j].transform.Find("Button").GetChild(0).GetComponent<Text>().text = agentName;
        }
    }
}
        
        /**/