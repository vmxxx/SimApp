using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static System.Math;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine.SceneManagement;

using System.IO;
using System.Linq;


public class LoadSimulationRules : MonoBehaviour
{
    
    public bool assetDatabaseRefreshed = false;
    private List<string> txtLines = File.ReadAllLines("Assets/TheApp/SimulationScene/RunSimulation.txt").ToList();

    private int amountOfFormulas;
    private int amountOfAgents;

    public GameObject loadingFilter;

    public Text likesCountText;
    public Text dislikesCountText;
    public Text approvedValText;
    public GameObject approveButton;
    public InputField nameInputField;
    public RawImage imageInputField;
    public InputField descriptionInputField;

    public List<string> payoffVariables = new List<string>();
    public string newPayoffVariable = "";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(loadSimlation());
    }


    public IEnumerator compileSimulationRules()
    {

        int[] indexes = new int[Buffer.instance.agents.Length];
        string totalAgentCount = Buffer.instance.agents[0].agentName + "s.Count";
        indexes[0] = totalAgentCount.Length;
        for (int i = 1; i < Buffer.instance.agents.Length; i++)
        {
            totalAgentCount = totalAgentCount + " + " + Buffer.instance.agents[i].agentName + "s.Count";
            indexes[i] = totalAgentCount.Length;
        }

        int a = 0;
        for (int i = 0; i < payoffVariables.Count; i++)
        {
            string entry = payoffVariables[i];
            txtLines.Insert(a + 32, "public float " + entry.Substring(1) + " = 0f;"); a = a + 1;
        }


        //We should be able to initialize the agent sets
        //All sets will start 10 individuals in each one
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 38, "private SortedSet<agent> " + entry.agentName + "s = new SortedSet<agent>(new agentComparer());"); a = a + 1;
            txtLines.Insert(a + 38, "public int startingNumberOf" + entry.agentName + "s = 10;"); a = a + 1;
        }

        //We initialize EMPTY cells for the payoffResults, because to recalculate the result (when changing the variables) we will instead use:
        //"payoffResults[(9, 9)] = some + formula;", not "payoffResults.Add((9, 9), some + formula );"
        foreach (KeyValuePair<(int, int), PayoffFormula> entry in Buffer.instance.payoffFormulas)
        {
            txtLines.Insert(a + 123, "payoffResults.Add(" + entry.Key + ", 0 );"); a = a + 1;
        }

        //Here we create the "recalculateFormulas()" function
        //where we use "payoffResults[(9, 9)] = some + formula;"
        foreach (KeyValuePair<(int, int), PayoffFormula> entry in Buffer.instance.payoffFormulas)
        {
            txtLines.Insert(a + 132, "payoffResults[" + entry.Key + "] = " + entry.Value.payoffFormula.Replace("$", "") + ";"); a = a + 1;
        }

        //In the Start() function we have create code that creates a "set $variable" button in the settings panel for each variable
        for (int i = 0; i < payoffVariables.Count; i++)
        {
            string entry = payoffVariables[i];
            txtLines.Insert(a + 144, "//We have to create a \"Set" + entry + "\" button"); a = a + 1;
            txtLines.Insert(a + 144, "newVariableSetting = Instantiate(variableSetting);"); a = a + 1;
            txtLines.Insert(a + 144, "newVariableSetting.SetActive(true);"); a = a + 1;
            txtLines.Insert(a + 144, "newVariableSetting.name = \"" + entry + "Setting\";"); a = a + 1;
            txtLines.Insert(a + 144, "newVariableSetting.transform.Find(\"CurrentValue\").GetComponent<Text>().text = \"" + entry + ": 0\";"); a = a + 1;
            txtLines.Insert(a + 144, "newVariableSetting.transform.Find(\"InputField\").GetComponent<InputField>().text = \"0\";"); a = a + 1;
            txtLines.Insert(a + 144, "newVariableSetting.transform.Find(\"Button\").GetChild(0).GetComponent<Text>().text = \"Set " + entry + "\";"); a = a + 1;
            txtLines.Insert(a + 144, "newVariableSetting.transform.Find(\"Button\").GetComponent<Button>().onClick.AddListener(delegate"); a = a + 1;
            txtLines.Insert(a + 144, "{"); a = a + 1;
            txtLines.Insert(a + 144, "if(pauseButton.transform.GetChild(0).GetComponent<Text>().text == \"Start\")"); a = a + 1;
            txtLines.Insert(a + 144, "{"); a = a + 1;
            txtLines.Insert(a + 144, "val = setVariable(simulationVariables.transform.Find(\"" + entry + "Setting\").Find(\"InputField\").gameObject);"); a = a + 1;
            txtLines.Insert(a + 144, entry.Substring(1) + " = val;"); a = a + 1;
            txtLines.Insert(a + 144, "simulationVariables.transform.Find(\"" + entry + "Setting\").Find(\"CurrentValue\").GetComponent<Text>().text = \"" + entry + ": \" + val;"); a = a + 1;
            txtLines.Insert(a + 144, "}"); a = a + 1;
            txtLines.Insert(a + 144, "else { StartCoroutine(Notification.instance.showNotification(\"You may only change this value, if the simulation isn't (re)started yet.\")); }"); a = a + 1;
            txtLines.Insert(a + 144, "});"); a = a + 1;
            txtLines.Insert(a + 144, "newVariableSetting.transform.SetParent(simulationVariables.transform);"); a = a + 1;
            txtLines.Insert(a + 144, "newVariableSetting.transform.position = variableSetting.transform.position - new Vector3(0f, (60 * varItr), 0);"); a = a + 1;
            txtLines.Insert(a + 144, "varItr++;"); a = a + 1;
            txtLines.Insert(a + 144, ""); a = a + 1;
        }

        //In the "initialize()" function (which is called every time we (re)start the simulation)
        //we have to delete the current agent sets and replace them with the fresh new ones (with 10 agents in each)
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 151, "//These next 15 lines are created by the code generator"); a = a + 1;
            txtLines.Insert(a + 151, "//The simulation starts with 10 " + entry.agentName + "s (at the start (or at the restart))"); a = a + 1;
            txtLines.Insert(a + 151, entry.agentName + "s = new SortedSet<agent>(new agentComparer());"); a = a + 1;
            txtLines.Insert(a + 151, "for (int i = 0; i < startingNumberOf" + entry.agentName + "s; i++)"); a = a + 1;
            txtLines.Insert(a + 151, "{"); a = a + 1;
            txtLines.Insert(a + 151, "agent newAgent = new agent();"); a = a + 1;
            txtLines.Insert(a + 151, "newAgent.ID = " + entry.agentID + ";"); a = a + 1;
            txtLines.Insert(a + 151, "newAgent.agentName = \"" + entry.agentName + "\";"); a = a + 1;
            txtLines.Insert(a + 151, "newAgent.agentDescription = \"\";"); a = a + 1;
            txtLines.Insert(a + 151, "newAgent.authorID = " + entry.authorID + ";"); a = a + 1;
            txtLines.Insert(a + 151, "newAgent.fitness = 0;"); a = a + 1;
            txtLines.Insert(a + 151, ""); a = a + 1;
            txtLines.Insert(a + 151, "while (" + entry.agentName + "s.Contains(newAgent)) { newAgent.key++; }"); a = a + 1;
            txtLines.Insert(a + 151, ""); a = a + 1;
            txtLines.Insert(a + 151, entry.agentName + "s.Add(newAgent);"); a = a + 1;
            txtLines.Insert(a + 151, "totalAmountOfIndividuals = totalAmountOfIndividuals + 1;"); a = a + 1;
            txtLines.Insert(a + 151, "}"); a = a + 1;
            txtLines.Insert(a + 151, ""); a = a + 1;
        }

        //If the simulation was restarted, then after the "initialize()" function, we have to add the first initial values to the graph
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 170, "WindoGraph.instance.addInitialValue(startingNumberOf" + entry.agentName + "s, \"" + entry.agentName + "s\", new Color(1f, 1f, 1f));"); a = a + 1;
        }
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 170, "WindoGraph.instance.yMaximum = (" + entry.agentName + "s.Count > WindoGraph.instance.yMaximum) ? " + entry.agentName + "s.Count : WindoGraph.instance.yMaximum;"); a = a + 1;
        }
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 170, "WindoGraph.instance.realignObjects(\"" + entry.agentName + "s\");"); a = a + 1;
        }



        //In the third phase we have specify to judge every individual's fate
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 184, "killOrDuplicateEachIndividual(" + entry.agentName + "s);"); a = a + 1;
        }




        //Add the new set of values to the graph in the 4th phase (right before we redraw the graph)
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 186, "WindoGraph.instance.newValue = " + entry.agentName + "s.Count;"); a = a + 1; string entryColor = entry.color.ToString();
            txtLines.Insert(a + 186, "WindoGraph.instance.addNewValue(\"" + entry.agentName + "s\", new Color(" + entry.color.r + "f, " + entry.color.g + "f, " + entry.color.b + "f));"); a = a + 1;
        }
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 186, "WindoGraph.instance.yMaximum = (" + entry.agentName + "s.Count > WindoGraph.instance.yMaximum) ? " + entry.agentName + "s.Count : WindoGraph.instance.yMaximum;"); a = a + 1;
        }
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 186, "WindoGraph.instance.realignObjects(\"" + entry.agentName + "s\");"); a = a + 1;
        }




        //If we want to assign a individuals in contests we have to take note of which ones are already assigned
        //Thats why we create extra sets for this reason "alreadyAssignedAgents";
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 248, "SortedSet<agent> alreadyAssigned" + entry.agentName + "s = new SortedSet<agent>(new agentComparer());"); a = a + 1;
        }

        //If totalNumberOfIndividuals in the simulation is odd
        //we have to randomly select an agent from all agent sets and put it in the alreadyAssignedAgent set.
        txtLines.Insert(a + 262, "randomIndex = rand.Next(" + totalAgentCount + ");"); a = a + 1;
        txtLines.Insert(a + 262, "if (randomIndex < " + totalAgentCount.Substring(0, indexes[0]) + ")"); a = a + 1;
        txtLines.Insert(a + 262, "{"); a = a + 1;
        txtLines.Insert(a + 262, "agent1 = " + Buffer.instance.agents[0].agentName + "s.ElementAt(randomIndex);"); a = a + 1;
        txtLines.Insert(a + 262, "alreadyAssigned" + Buffer.instance.agents[0].agentName + "s.Add(agent1);"); a = a + 1;
        txtLines.Insert(a + 262, Buffer.instance.agents[0].agentName + "s.Remove(agent1);"); a = a + 1;
        txtLines.Insert(a + 262, "}"); a = a + 1;
        for (int i = 1; i < Buffer.instance.agents.Length; i++)
        {
            txtLines.Insert(a + 262, "else if (randomIndex < " + totalAgentCount.Substring(0, indexes[i]) + ")"); a = a + 1;
            txtLines.Insert(a + 262, "{"); a = a + 1;
            txtLines.Insert(a + 262, "agent1 = " + Buffer.instance.agents[i].agentName + "s.ElementAt(randomIndex - (" + totalAgentCount.Substring(0, indexes[i - 1]) + "));"); a = a + 1;
            txtLines.Insert(a + 262, "alreadyAssigned" + Buffer.instance.agents[i].agentName + "s.Add(agent1);"); a = a + 1;
            txtLines.Insert(a + 262, Buffer.instance.agents[i].agentName + "s.Remove(agent1);"); a = a + 1;
            txtLines.Insert(a + 262, "}"); a = a + 1;
        }

        //To select the first agent
        //we have to randomly select an agent from all agent sets and put it in the alreadyAssignedAgent set.
        txtLines.Insert(a + 268, "randomIndex = rand.Next(" + totalAgentCount + ");"); a = a + 1;
        txtLines.Insert(a + 268, "if (randomIndex < " + totalAgentCount.Substring(0, indexes[0]) + ")"); a = a + 1;
        txtLines.Insert(a + 268, "{"); a = a + 1;
        txtLines.Insert(a + 268, "agent1 = " + Buffer.instance.agents[0].agentName + "s.ElementAt(randomIndex);"); a = a + 1;
        txtLines.Insert(a + 268, "alreadyAssigned" + Buffer.instance.agents[0].agentName + "s.Add(agent1);"); a = a + 1;
        txtLines.Insert(a + 268, Buffer.instance.agents[0].agentName + "s.Remove(agent1);"); a = a + 1;
        txtLines.Insert(a + 268, "}"); a = a + 1;
        for (int i = 1; i < Buffer.instance.agents.Length; i++)
        {
            txtLines.Insert(a + 268, "else if (randomIndex < " + totalAgentCount.Substring(0, indexes[i]) + ")"); a = a + 1;
            txtLines.Insert(a + 268, "{"); a = a + 1;
            txtLines.Insert(a + 268, "agent1 = " + Buffer.instance.agents[i].agentName + "s.ElementAt(randomIndex - (" + totalAgentCount.Substring(0, indexes[i - 1]) + "));"); a = a + 1;
            txtLines.Insert(a + 268, "alreadyAssigned" + Buffer.instance.agents[i].agentName + "s.Add(agent1);"); a = a + 1;
            txtLines.Insert(a + 268, Buffer.instance.agents[i].agentName + "s.Remove(agent1);"); a = a + 1;
            txtLines.Insert(a + 268, "}"); a = a + 1;
        }

        //To select the second agent
        //we have to randomly select an agent from all agent sets and put it in the alreadyAssignedAgent set.
        txtLines.Insert(a + 268, "randomIndex = rand.Next(" + totalAgentCount + ");"); a = a + 1;
        txtLines.Insert(a + 268, "if (randomIndex < " + totalAgentCount.Substring(0, indexes[0]) + ")"); a = a + 1;
        txtLines.Insert(a + 268, "{"); a = a + 1;
        txtLines.Insert(a + 268, "agent2 = " + Buffer.instance.agents[0].agentName + "s.ElementAt(randomIndex);"); a = a + 1;
        txtLines.Insert(a + 268, "alreadyAssigned" + Buffer.instance.agents[0].agentName + "s.Add(agent2);"); a = a + 1;
        txtLines.Insert(a + 268, Buffer.instance.agents[0].agentName + "s.Remove(agent2);"); a = a + 1;
        txtLines.Insert(a + 268, "}"); a = a + 1;
        for (int i = 1; i < Buffer.instance.agents.Length; i++)
        {
            txtLines.Insert(a + 268, "else if (randomIndex < " + totalAgentCount.Substring(0, indexes[i]) + ")"); a = a + 1;
            txtLines.Insert(a + 268, "{"); a = a + 1;
            txtLines.Insert(a + 268, "agent2 = " + Buffer.instance.agents[i].agentName + "s.ElementAt(randomIndex - (" + totalAgentCount.Substring(0, indexes[i - 1]) + "));"); a = a + 1;
            txtLines.Insert(a + 268, "alreadyAssigned" + Buffer.instance.agents[i].agentName + "s.Add(agent2);"); a = a + 1;
            txtLines.Insert(a + 268, Buffer.instance.agents[i].agentName + "s.Remove(agent2);"); a = a + 1;
            txtLines.Insert(a + 268, "}"); a = a + 1;
        }

        //After we're done with the contests we put all the assigned agents back into their original sets
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 276, entry.agentName + "s = alreadyAssigned" + entry.agentName + "s;"); a = a + 1;
        }

        //Generate the simulation code
        File.WriteAllLines("Assets/TheApp/SimulationScene/RunSimulation.cs", txtLines);
        AssetDatabase.Refresh();
        yield return new RecompileScripts();
        loadingFilter.SetActive(false);
    }

    IEnumerator loadSimlation()
    {
        //1. Get the simulation object

        //Create an HTML form with the data
        WWWForm form = new WWWForm();
        form.AddField("class", "SimulationsController\\simulations");
        form.AddField("function", "read");
        form.AddField("scene", "creation");
        form.AddField("ID", Buffer.instance.currentSimulation.ID);
        //form.AddField("authorID", Buffer.instance.authenticatedUser.ID);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        if (www.text != "" && www.text[0] == '0')
        {
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
            string approved = Regex.Match(match.Value, @"approved:(.*?),").Value;
            string authorID = Regex.Match(match.Value, @"authorID:(.*?)}").Value;

            //Convert numbers to their appropriate data types, let the strings stay as strings
            //Save the object we've just got in the buffer
            Buffer.instance.currentSimulation.ID = Int32.Parse(ID.Substring(3, ID.Length - 4));
            Buffer.instance.currentSimulation.name = name.Substring(6, name.Length - 8);
            Buffer.instance.currentSimulation.image = image.Substring(7, image.Length - 9);
            Buffer.instance.currentSimulation.description = description.Substring(13, description.Length - 15);
            Buffer.instance.currentSimulation.likesCount = Int32.Parse(likesCount.Substring(11, likesCount.Length - 12));
            Buffer.instance.currentSimulation.dislikesCount = Int32.Parse(dislikesCount.Substring(14, dislikesCount.Length - 15));
            Buffer.instance.currentSimulation.approved = (approved.Substring(9, approved.Length - 10) == "1") ? true : false;
            Buffer.instance.currentSimulation.authorID = Int32.Parse(authorID.Substring(9, authorID.Length - 10));

            Buffer.instance.currentSimulationID = Int32.Parse(ID.Substring(3, ID.Length - 4));
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }


        //2. Get the simulation formulas
        form = new WWWForm();
        form.AddField("class", "PayoffFormulasController\\payoffFormulas");
        form.AddField("function", "read");
        form.AddField("scene", "creation");
        form.AddField("simulationID", Buffer.instance.currentSimulation.ID);
        string agentsArray = "";

        www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        int i;
        //If there is no NULL notification AND if the notification code is 0 (no error)
        if (www.text != "" && www.text[0] == '0')
        {
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
                if (i < amountOfAgents)
                {
                    agentsArray = agentsArray + agent2.Substring(7, agent2.Length - 7);
                }
            }

            //delete the last coma 
            //9, 10, 11, 12, -> 9, 10, 11, 12
            agentsArray = agentsArray.Substring(0, agentsArray.Length - 1);
        }
        else
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }


        //3. Get the agents objects


        //If there is no NULL notification AND if the notification code is 0 (no error)
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
            //Initialize a new agents array in the buffer
            Buffer.instance.newAgentsArray(amountOfAgents);

            //Select the data set substring
            //Example: { agentID: 9, icon: "$#!@$%0^&!!!&^%", agentName: "Dove", agentDescription: "doves are cowardly", authorID: 1 } { agentID: 10, icon: "$#!@$%0^&!!!&^%", agentName: "Hawk", agentDescription: "hawks are aggresive", authorID: 1 } 

            //Find every agent object
            Regex pattern = new Regex(@"{(.*?)}");
            //For each agent object
            MatchCollection matches = pattern.Matches(www.text);
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
                Buffer.instance.agents[i].agentDescription = agentDescription.Substring(13, agentDescription.Length - 15);
                Buffer.instance.agents[i].authorID = Int32.Parse(authorID.Substring(9, authorID.Length - 10));
            }
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }

        //In the simulation details panel
        //write down its name, image and description
        nameInputField.text = Buffer.instance.currentSimulation.name;
        imageInputField.texture = applyBase64StringAsTexture(Buffer.instance.currentSimulation.image);
        descriptionInputField.text = Buffer.instance.currentSimulation.description;
        likesCountText.text = Buffer.instance.currentSimulation.likesCount.ToString();
        dislikesCountText.text = Buffer.instance.currentSimulation.dislikesCount.ToString();
        approvedValText.text = (Buffer.instance.currentSimulation.approved == true) ? "yes" : "no";
        approveButton.transform.GetChild(0).GetComponent<Text>().text = (Buffer.instance.currentSimulation.approved == true) ? "Unapprove" : "Approve";


        bool variableFound = false;
        foreach (KeyValuePair<(int, int), PayoffFormula> entry in Buffer.instance.payoffFormulas)
        {
            string formula = entry.Value.payoffFormula;
            for (int j = 0; j < formula.Length; j++)
            {
                if (formula[j] == '$') variableFound = true;
                if (variableFound == true)
                {
                    if(!((j + 1 < formula.Length) && ((formula[j + 1] >= 'a' && formula[j + 1] <= 'z') || (formula[j + 1] >= 'A' && formula[j + 1] <= 'Z') || (formula[j + 1] >= '0' && formula[j + 1] <= '9'))))
                    {
                        newPayoffVariable = newPayoffVariable + formula[j];
                        if (!payoffVariables.Contains(newPayoffVariable)) { payoffVariables.Add(newPayoffVariable); } //Push
                        newPayoffVariable = "";
                        variableFound = false;
                    }
                    else
                    {
                        newPayoffVariable = newPayoffVariable + formula[j];
                    }
                }
            }
        }

        PayoffMatrix_2.instance.initialize();
        StartCoroutine(compileSimulationRules());
    }

    private Texture2D applyBase64StringAsTexture(string textureString)
    {
        Texture2D newTexture = new Texture2D(1, 1);
        newTexture.LoadImage(Convert.FromBase64String(textureString));
        newTexture.Apply();
        return newTexture;
    }
}
