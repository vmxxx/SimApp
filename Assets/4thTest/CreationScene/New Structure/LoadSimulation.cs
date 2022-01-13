using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static System.Math;
using System.Text.RegularExpressions;

public class LoadSimulation : MonoBehaviour
{
    //This variable is needed so that we can reference this class from outside other classes.
    public static LoadSimulation instance;

    public GameObject payoffMatrixPanel;
    public GameObject emptyCell;

    public InputField nameInputField;
    public GameObject imageInputField;
    public InputField descriptionInputField;

    private int amountOfFormulas;
    private int amountOfAgents;

    //The Update() function gets called every new frame.
    //As soon as we detect that the instance is forgotten we have to reset it 
    //(It is forgotten every time the scripts get recompiled.
    //The scripts are recompiled so that the compiler can test the payoffFormulas we have entered in the payoff matrix by putting them in a testing file).
    private void Update()
    {
        if (LoadSimulation.instance == null) instance = this;
    }

    //Start() gets called before the first frame update.
    private void Start()
    {
        StartCoroutine(loadSimulation());
    }

    IEnumerator loadSimulation()
    {
        //1. Get the simulation object

        WWWForm form = new WWWForm();
        form.AddField("class", "SimulationsController\\simulations");
        form.AddField("function", "read");
        form.AddField("scene", "creation");
        form.AddField("ID", Buffer.instance.currentSimulation.ID);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //Tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code.

        //If there is no NULL notification AND if the notification code is 0 (no error)
        //we display the success notification and put the received simulation data in the buffer
        if (www.text != "" && www.text[0] == '0')
        {
            //Display the notification
            //StartCoroutine(Notification.instance.showNotification(www.text));

            //Select the data set substring
            //Example: { ID: 1, name: "Hawks Doves", image: "@!$#-&%!*9&a7^*!#@", description: "The most basic game. Frequency of hawks is V/C.", likesCount: 10, dislikesCount: 2, authorID: 10 }
            Regex pattern = new Regex(@"{(.*?)}");

            //Find it in the www.text
            MatchCollection matches = pattern.Matches(www.text);

            //Find the first such match (the only one)
            Match match = matches[0];

            //Extract the data in string form
            string ID = Regex.Match(match.Value, @"ID:(.*?),").Value;
            string name = Regex.Match(match.Value, @"name:(.*?),").Value;
            string image = Regex.Match(match.Value, @"image:(.*?),").Value;
            string description = Regex.Match(match.Value, @"description:(.*?),").Value;
            string likesCount = Regex.Match(match.Value, @"likesCount:(.*?),").Value;
            string dislikesCount = Regex.Match(match.Value, @"dislikesCount:(.*?),").Value;
            string authorID = Regex.Match(match.Value, @"authorID:(.*?)}").Value;

            //Convert numbers to their appropriate data types, let the strings stay as strings
            //Save the object we've just got in the buffer
            Buffer.instance.currentSimulation.ID = Int32.Parse(ID.Substring(3, ID.Length - 4));
            Buffer.instance.currentSimulation.name = name.Substring(6, name.Length - 8);
            Buffer.instance.currentSimulation.image = image.Substring(7, image.Length - 9);
            Buffer.instance.currentSimulation.description = description.Substring(13, description.Length - 15);
            Buffer.instance.currentSimulation.likesCount = Int32.Parse(likesCount.Substring(11, likesCount.Length - 12));
            Buffer.instance.currentSimulation.dislikesCount = Int32.Parse(dislikesCount.Substring(14, dislikesCount.Length - 15));
            Buffer.instance.currentSimulation.authorID = Int32.Parse(authorID.Substring(9, authorID.Length - 10));
        }
        else //Display error notification
        {
            //if (www.text != "") StartCoroutine(Notification.instance.showNotification("Loading simulations failed. Error #" + www.text));
            //else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
        }

        //2. Get the formulas objects

        form = new WWWForm();
        form.AddField("class", "PayoffFormulasController\\payoffFormulas");
        form.AddField("function", "read");
        form.AddField("scene", "creation");
        form.AddField("simulationID", Buffer.instance.currentSimulation.ID);
        string agentsArray = "";

        www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //Tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code.

        int i;
        //If there is no NULL notification AND if the notification code is 0 (no error)
        //we display the success notification and put the received simulation data in the buffer
        if (www.text != "" && www.text[0] == '0')
        {
            //Display the notification
            //StartCoroutine(Notification.instance.showNotification(www.text));

            //Determine how many formulas there are in the query
            amountOfFormulas = Int32.Parse(www.text.Substring(2, 1));
            //amountOfAgents = sqrt(amountOfFormulas)
            amountOfAgents = (int)Sqrt(amountOfFormulas);

            //DoveID = 9
            //HawkID = 10

            //Select the data set substring
            //Example: { ID: 1, agent1: 9, agent2: 10, payoffFormula: "0", simulationID: 1 } { ID: 2, agent1: 10, agent2: 10, payoffFormula: "($V - $C) / 2", simulationID: 1 }
            Regex pattern = new Regex(@"{(.*?)}");
            Buffer.instance.payoffFormulas = new Dictionary<(int, int), PayoffFormula>();

            //Find every formula object
            MatchCollection matches = pattern.Matches(www.text);

            //For each formula object
            for (i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];

                //Initialize a new formula
                Buffer.instance.newFormula = new PayoffFormula();

                //Extract the data in string form
                string ID = Regex.Match(match.Value, @"ID:(.*?),").Value;
                string agent1 = Regex.Match(match.Value, @"agent1:(.*?),").Value;
                string agent2 = Regex.Match(match.Value, @"agent2:(.*?),").Value;
                string payoffFormula = Regex.Match(match.Value, @"payoffFormula:(.*?),").Value;
                string simulationID = Regex.Match(match.Value, @"simulationID:(.*?)}").Value;

                //Convert numbers to their appropriate data types, let the strings stay as strings
                //Save the object we've just got in the buffer
                Buffer.instance.newFormula.ID = Int32.Parse(ID.Substring(3, ID.Length - 4));
                Buffer.instance.newFormula.agent1 = Int32.Parse(agent1.Substring(7, agent1.Length - 8));
                Buffer.instance.newFormula.agent2 = Int32.Parse(agent2.Substring(7, agent2.Length - 8));
                Buffer.instance.newFormula.payoffFormula = payoffFormula.Substring(15, payoffFormula.Length - 17);
                Buffer.instance.newFormula.authorID = Int32.Parse(simulationID.Substring(13, simulationID.Length - 14));

                //Add the new formula object to the hash table
                Buffer.instance.payoffFormulas.Add((Buffer.instance.newFormula.agent1, Buffer.instance.newFormula.agent2), Buffer.instance.newFormula);

                //Take note of every unique agent ID we've got from the data set, and store them in a string in this format:
                //9, 10, 11, 12,
                if (i < amountOfAgents) agentsArray = agentsArray + agent2.Substring(7, agent2.Length - 7); 
            }

            //delete the last coma 
            //9, 10, 11, 12, -> 9, 10, 11, 12
            Debug.Log(agentsArray);
            agentsArray = agentsArray.Substring(0, agentsArray.Length - 1);
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification("Loading agents failed. Error #" + www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
        }

        //2. Get the agents objects

        form = new WWWForm();
        form.AddField("class", "AgentsController\\agents");
        form.AddField("function", "read");
        form.AddField("fullList", "false");
        form.AddField("simulationID", Buffer.instance.currentSimulation.ID);
        form.AddField("IDsArray", agentsArray);

        www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        //we display the success notification and put the received simulation data in the buffer
        if (www.text != "" && www.text[0] == '0')
        {
            //Display the notification
            //StartCoroutine(Notification.instance.showNotification(www.text));

            //Initialize a new agents array in the buffer
            Buffer.instance.newAgentsArray(amountOfAgents);

            //Select the data set substring
            //Example: { agentID: 9, icon: "$#!@$%0^&!!!&^%", agentName: "Dove", agentDescription: "doves are cowardly", authorID: 1 } { agentID: 10, icon: "$#!@$%0^&!!!&^%", agentName: "Hawk", agentDescription: "hawks are aggresive", authorID: 1 } 
            Regex pattern = new Regex(@"{(.*?)}");

            //Find every agent object
            MatchCollection matches = pattern.Matches(www.text);
            //For each agent object
            for (i = 0; i < amountOfAgents; i++)
            {

                Match match = matches[i];

                //Extract the data in string form
                string ID = Regex.Match(match.Value, @"ID:(.*?),").Value;
                string icon = Regex.Match(match.Value, @"icon:(.*?),").Value;
                string agentName = Regex.Match(match.Value, @"name:(.*?),").Value;
                string agentDescription = Regex.Match(match.Value, @"description:(.*?),").Value;
                string authorID = Regex.Match(match.Value, @"authorID:(.*?)}").Value;

                //Convert numbers to their appropriate data types, let the strings stay as strings
                //Save the object we've just got in the buffer
                Buffer.instance.agents[i].agentID = Int32.Parse(ID.Substring(3, ID.Length - 4));
                Buffer.instance.agents[i].icon = icon.Substring(5, icon.Length - 6);
                Buffer.instance.agents[i].agentName = agentName.Substring(5, agentName.Length - 6);
                Buffer.instance.agents[i].agentDescription = agentDescription.Substring(12, agentDescription.Length - 13);
                Buffer.instance.agents[i].authorID = Int32.Parse(authorID.Substring(9, authorID.Length - 10));
            }

        }
        else //Display error notification
        {
            //if (www.text != "") StartCoroutine(Notification.instance.showNotification("Loading agents failed. Error #" + www.text));
            //else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
        }

        //In the simulation details panel
        //write down its name, image and description
        nameInputField.text = Buffer.instance.currentSimulation.name;

        Debug.Log(Buffer.instance.currentSimulation.image);
        Debug.Log( Convert.FromBase64String(Buffer.instance.currentSimulation.image) );
        Texture2D newTexture = new Texture2D(1, 1);
        newTexture.LoadImage(Convert.FromBase64String(Buffer.instance.currentSimulation.image));
        newTexture.Apply();
        imageInputField.GetComponent<RawImage>().texture = newTexture;
        descriptionInputField.text = Buffer.instance.currentSimulation.description;

        //Add extra columns in the payoff matrix for the amount of agents this simulation has
        for (i = 1; i < amountOfAgents; i++) { PayoffMatrix.instance.addExtra(); }

        //Write a payoff formula in every matrix cell
        //We iterate through the agents array n^2 times (where n = amount of agents or n^2 = amount of payoff formulas)
        for (int j = 1; j <= amountOfAgents; j++)
        {
            for (int k = 1; k <= amountOfAgents; k++)
            {
                int jj = Buffer.instance.agents[j - 1].agentID;
                int kk = Buffer.instance.agents[k - 1].agentID;

                //Write a payoff formula in every matrix cell
                PayoffMatrix.instance.tableCells[j].cell[k].transform.Find("Formula").GetComponent<InputField>().text = Buffer.instance.payoffFormulas[(jj, kk)].payoffFormula;
            }

            string agentID = Buffer.instance.agents[j - 1].agentID.ToString();
            string agentIcon = Buffer.instance.agents[j - 1].icon;
            string agentName = Buffer.instance.agents[j - 1].agentName;
            string agentDescription = Buffer.instance.agents[j - 1].agentDescription;


            //In the zeroth table column we set the agent names (and hide their IDs, so its easier to reference them later)
            PayoffMatrix.instance.tableCells[j].cell[0].transform.Find("AgentID").GetComponent<Text>().text = agentID;
            PayoffMatrix.instance.tableCells[j].cell[0].transform.Find("Button").GetChild(0).GetComponent<Text>().text = agentName;
            try
            {
                newTexture = new Texture2D(1, 1);
                newTexture.LoadImage(Convert.FromBase64String(agentIcon));
                newTexture.Apply();
                PayoffMatrix.instance.tableCells[j].cell[0].transform.Find("Button").GetChild(1).GetComponent<RawImage>().texture = newTexture;
            }
            catch { }
            PayoffMatrix.instance.tableCells[j].cell[0].transform.Find("Button").GetChild(1).GetChild(0).Find("Name").GetComponent<Text>().text = agentName;
            PayoffMatrix.instance.tableCells[j].cell[0].transform.Find("Button").GetChild(1).GetChild(0).Find("Description").GetComponent<Text>().text = agentDescription;

            //In the zeroth table row we set the agent names (and hide their IDs, so its easier to reference them later)
            PayoffMatrix.instance.tableCells[0].cell[j].transform.Find("AgentID").GetComponent<Text>().text = agentID;
            PayoffMatrix.instance.tableCells[0].cell[j].transform.Find("Button").GetChild(0).GetComponent<Text>().text = agentName;

            try
            {
                newTexture = new Texture2D(1, 1);
                newTexture.LoadImage(Convert.FromBase64String(agentIcon));
                newTexture.Apply();
                PayoffMatrix.instance.tableCells[0].cell[j].transform.Find("Button").GetChild(1).GetComponent<RawImage>().texture = newTexture;
            }
            catch { }
            PayoffMatrix.instance.tableCells[0].cell[j].transform.Find("Button").GetChild(1).GetChild(0).Find("Name").GetComponent<Text>().text = agentName;
            PayoffMatrix.instance.tableCells[0].cell[j].transform.Find("Button").GetChild(1).GetChild(0).Find("Description").GetComponent<Text>().text = agentDescription;
        }

        SearchAgent.instance.clearAgents();
    }
}