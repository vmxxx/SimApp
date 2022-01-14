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
    private List<string> txtLines = File.ReadAllLines("Assets/4thTest/SimulationScene/RunSimulation.txt").ToList();

    private int amountOfFormulas;
    private int amountOfAgents;

    public InputField nameInputField;
    public InputField imageInputField;
    public InputField descriptionInputField;

    public List<string> payoffVariables = new List<string>();
    public string newPayoffVariable = "";

    // Start is called before the first frame update
    void Awake()
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
            txtLines.Insert(a + 119, "payoffResults.Add(" + entry.Key + ", 0 );"); a = a + 1;
        }

        //Here we create the "recalculateFormulas()" function
        //where we use "payoffResults[(9, 9)] = some + formula;"
        foreach (KeyValuePair<(int, int), PayoffFormula> entry in Buffer.instance.payoffFormulas)
        {
            txtLines.Insert(a + 128, "payoffResults[" + entry.Key + "] = " + entry.Value.payoffFormula.Replace("$", "") + ";"); a = a + 1;
        }

        //In the Start() function we have create code that creates a "set $variable" button in the settings panel for each variable
        for (int i = 0; i < payoffVariables.Count; i++)
        {
            string entry = payoffVariables[i];
            txtLines.Insert(a + 140, "//We have to create a \"Set" + entry + "\" button"); a = a + 1;
            txtLines.Insert(a + 140, "newVariableSetting = Instantiate(variableSetting);"); a = a + 1;
            txtLines.Insert(a + 140, "newVariableSetting.SetActive(true);"); a = a + 1;
            txtLines.Insert(a + 140, "newVariableSetting.name = \"" + entry + "Setting\";"); a = a + 1;
            txtLines.Insert(a + 140, "newVariableSetting.transform.Find(\"CurrentValue\").GetComponent<Text>().text = \"" + entry + ": 0\";"); a = a + 1;
            txtLines.Insert(a + 140, "newVariableSetting.transform.Find(\"InputField\").GetComponent<InputField>().text = \"0\";"); a = a + 1;
            txtLines.Insert(a + 140, "newVariableSetting.transform.Find(\"Button\").GetChild(0).GetComponent<Text>().text = \"Set " + entry + "\";"); a = a + 1;
            txtLines.Insert(a + 140, "newVariableSetting.transform.Find(\"Button\").GetComponent<Button>().onClick.AddListener(delegate"); a = a + 1;
            txtLines.Insert(a + 140, "{"); a = a + 1;
            txtLines.Insert(a + 140, "val = setVariable(simulationVariables.transform.Find(\"" + entry + "Setting\").Find(\"InputField\").gameObject);"); a = a + 1;
            txtLines.Insert(a + 140, entry.Substring(1) + " = val;"); a = a + 1;
            txtLines.Insert(a + 140, "simulationVariables.transform.Find(\"" + entry + "Setting\").Find(\"CurrentValue\").GetComponent<Text>().text = \"" + entry + ": \" + val;"); a = a + 1;
            txtLines.Insert(a + 140, "});"); a = a + 1;
            txtLines.Insert(a + 140, "newVariableSetting.transform.SetParent(simulationVariables.transform);"); a = a + 1;
            txtLines.Insert(a + 140, "newVariableSetting.transform.position = variableSetting.transform.position - new Vector3(0f, (60 * varItr), 0);"); a = a + 1;
            txtLines.Insert(a + 140, "varItr++;"); a = a + 1;
            txtLines.Insert(a + 140, ""); a = a + 1;
        }

        //In the "initialize()" function (which is called every time we (re)start the simulation)
        //we have to delete the current agent sets and replace them with the fresh new ones (with 10 agents in each)
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 147, "//These next 15 lines are created by the code generator"); a = a + 1;
            txtLines.Insert(a + 147, "//The simulation starts with 10 " + entry.agentName + "s (at the start (or at the restart))"); a = a + 1;
            txtLines.Insert(a + 147, entry.agentName + "s = new SortedSet<agent>(new agentComparer());"); a = a + 1;
            txtLines.Insert(a + 147, "for (int i = 0; i < startingNumberOf" + entry.agentName + "s; i++)"); a = a + 1;
            txtLines.Insert(a + 147, "{"); a = a + 1;
            txtLines.Insert(a + 147, "agent newAgent = new agent();"); a = a + 1;
            txtLines.Insert(a + 147, "newAgent.ID = " + entry.agentID + ";"); a = a + 1;
            txtLines.Insert(a + 147, "newAgent.agentName = \"" + entry.agentName + "\";"); a = a + 1;
            txtLines.Insert(a + 147, "newAgent.agentDescription = \"\";"); a = a + 1;
            txtLines.Insert(a + 147, "newAgent.authorID = " + entry.authorID + ";"); a = a + 1;
            txtLines.Insert(a + 147, "newAgent.fitness = 0;"); a = a + 1;
            txtLines.Insert(a + 147, ""); a = a + 1;
            txtLines.Insert(a + 147, "while (" + entry.agentName + "s.Contains(newAgent)) { newAgent.key++; }"); a = a + 1;
            txtLines.Insert(a + 147, ""); a = a + 1;
            txtLines.Insert(a + 147, entry.agentName + "s.Add(newAgent);"); a = a + 1;
            txtLines.Insert(a + 147, "totalAmountOfIndividuals = totalAmountOfIndividuals + 1;"); a = a + 1;
            txtLines.Insert(a + 147, "}"); a = a + 1;
            txtLines.Insert(a + 147, ""); a = a + 1;
        }

        //If the simulation was restarted, then after the "initialize()" function, we have to add the first initial values to the graph
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 166, "WindoGraph.instance.addInitialValue(startingNumberOf" + entry.agentName + "s, \"" + entry.agentName + "s\", new Color(1f, 1f, 1f));"); a = a + 1;
        }
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 166, "WindoGraph.instance.yMaximum = (" + entry.agentName + "s.Count > WindoGraph.instance.yMaximum) ? " + entry.agentName + "s.Count : WindoGraph.instance.yMaximum;"); a = a + 1;
        }
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 166, "WindoGraph.instance.realignObjects(\"" + entry.agentName + "s\");"); a = a + 1;
        }



        //In the third phase we have specify to judge every individual's fate
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 180, "killOrDuplicateEachIndividual(" + entry.agentName + "s);"); a = a + 1;
        }




        //Add the new set of values to the graph in the 4th phase (right before we redraw the graph)
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 182, "WindoGraph.instance.newValue = " + entry.agentName + "s.Count;"); a = a + 1; string entryColor = entry.color.ToString();
            txtLines.Insert(a + 182, "WindoGraph.instance.addNewValue(\"" + entry.agentName + "s\", new Color(" + entry.color.r + "f, " + entry.color.g + "f, " + entry.color.b + "f));"); a = a + 1;
        }
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 182, "WindoGraph.instance.yMaximum = (" + entry.agentName + "s.Count > WindoGraph.instance.yMaximum) ? " + entry.agentName + "s.Count : WindoGraph.instance.yMaximum;"); a = a + 1;
        }
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 182, "WindoGraph.instance.realignObjects(\"" + entry.agentName + "s\");"); a = a + 1;
        }




        //If we want to assign a individuals in contests we have to take note of which ones are already assigned
        //Thats why we create extra sets for this reason "alreadyAssignedAgents";
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 246, "SortedSet<agent> alreadyAssigned" + entry.agentName + "s = new SortedSet<agent>(new agentComparer());"); a = a + 1;
        }

        //If totalNumberOfIndividuals in the simulation is odd
        //we have to randomly select an agent from all agent sets and put it in the alreadyAssignedAgent set.
        txtLines.Insert(a + 258, "randomIndex = rand.Next(" + totalAgentCount + ");"); a = a + 1;
        txtLines.Insert(a + 258, "if (randomIndex < " + totalAgentCount.Substring(0, indexes[0]) + ")"); a = a + 1;
        txtLines.Insert(a + 258, "{"); a = a + 1;
        txtLines.Insert(a + 258, "agent1 = " + Buffer.instance.agents[0].agentName + "s.ElementAt(randomIndex);"); a = a + 1;
        txtLines.Insert(a + 258, "alreadyAssigned" + Buffer.instance.agents[0].agentName + "s.Add(agent1);"); a = a + 1;
        txtLines.Insert(a + 258, Buffer.instance.agents[0].agentName + "s.Remove(agent1);"); a = a + 1;
        txtLines.Insert(a + 258, "}"); a = a + 1;
        for (int i = 1; i < Buffer.instance.agents.Length; i++)
        {
            txtLines.Insert(a + 258, "else if (randomIndex < " + totalAgentCount.Substring(0, indexes[i]) + ")"); a = a + 1;
            txtLines.Insert(a + 258, "{"); a = a + 1;
            txtLines.Insert(a + 258, "agent1 = " + Buffer.instance.agents[i].agentName + "s.ElementAt(randomIndex - (" + totalAgentCount.Substring(0, indexes[i - 1]) + "));"); a = a + 1;
            txtLines.Insert(a + 258, "alreadyAssigned" + Buffer.instance.agents[i].agentName + "s.Add(agent1);"); a = a + 1;
            txtLines.Insert(a + 258, Buffer.instance.agents[i].agentName + "s.Remove(agent1);"); a = a + 1;
            txtLines.Insert(a + 258, "}"); a = a + 1;
        }

        //To select the first agent
        //we have to randomly select an agent from all agent sets and put it in the alreadyAssignedAgent set.
        txtLines.Insert(a + 264, "randomIndex = rand.Next(" + totalAgentCount + ");"); a = a + 1;
        txtLines.Insert(a + 264, "if (randomIndex < " + totalAgentCount.Substring(0, indexes[0]) + ")"); a = a + 1;
        txtLines.Insert(a + 264, "{"); a = a + 1;
        txtLines.Insert(a + 264, "agent1 = " + Buffer.instance.agents[0].agentName + "s.ElementAt(randomIndex);"); a = a + 1;
        txtLines.Insert(a + 264, "alreadyAssigned" + Buffer.instance.agents[0].agentName + "s.Add(agent1);"); a = a + 1;
        txtLines.Insert(a + 264, Buffer.instance.agents[0].agentName + "s.Remove(agent1);"); a = a + 1;
        txtLines.Insert(a + 264, "}"); a = a + 1;
        for (int i = 1; i < Buffer.instance.agents.Length; i++)
        {
            txtLines.Insert(a + 264, "else if (randomIndex < " + totalAgentCount.Substring(0, indexes[i]) + ")"); a = a + 1;
            txtLines.Insert(a + 264, "{"); a = a + 1;
            txtLines.Insert(a + 264, "agent1 = " + Buffer.instance.agents[i].agentName + "s.ElementAt(randomIndex - (" + totalAgentCount.Substring(0, indexes[i - 1]) + "));"); a = a + 1;
            txtLines.Insert(a + 264, "alreadyAssigned" + Buffer.instance.agents[i].agentName + "s.Add(agent1);"); a = a + 1;
            txtLines.Insert(a + 264, Buffer.instance.agents[i].agentName + "s.Remove(agent1);"); a = a + 1;
            txtLines.Insert(a + 264, "}"); a = a + 1;
        }

        //To select the second agent
        //we have to randomly select an agent from all agent sets and put it in the alreadyAssignedAgent set.
        txtLines.Insert(a + 264, "randomIndex = rand.Next(" + totalAgentCount + ");"); a = a + 1;
        txtLines.Insert(a + 264, "if (randomIndex < " + totalAgentCount.Substring(0, indexes[0]) + ")"); a = a + 1;
        txtLines.Insert(a + 264, "{"); a = a + 1;
        txtLines.Insert(a + 264, "agent2 = " + Buffer.instance.agents[0].agentName + "s.ElementAt(randomIndex);"); a = a + 1;
        txtLines.Insert(a + 264, "alreadyAssigned" + Buffer.instance.agents[0].agentName + "s.Add(agent2);"); a = a + 1;
        txtLines.Insert(a + 264, Buffer.instance.agents[0].agentName + "s.Remove(agent2);"); a = a + 1;
        txtLines.Insert(a + 264, "}"); a = a + 1;
        for (int i = 1; i < Buffer.instance.agents.Length; i++)
        {
            txtLines.Insert(a + 264, "else if (randomIndex < " + totalAgentCount.Substring(0, indexes[i]) + ")"); a = a + 1;
            txtLines.Insert(a + 264, "{"); a = a + 1;
            txtLines.Insert(a + 264, "agent2 = " + Buffer.instance.agents[i].agentName + "s.ElementAt(randomIndex - (" + totalAgentCount.Substring(0, indexes[i - 1]) + "));"); a = a + 1;
            txtLines.Insert(a + 264, "alreadyAssigned" + Buffer.instance.agents[i].agentName + "s.Add(agent2);"); a = a + 1;
            txtLines.Insert(a + 264, Buffer.instance.agents[i].agentName + "s.Remove(agent2);"); a = a + 1;
            txtLines.Insert(a + 264, "}"); a = a + 1;
        }

        //After we're done with the contests we put all the assigned agents back into their original sets
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            Agent entry = Buffer.instance.agents[i];
            txtLines.Insert(a + 272, entry.agentName + "s = alreadyAssigned" + entry.agentName + "s;"); a = a + 1;
        }

        //Generate the simulation code
        File.WriteAllLines("Assets/4thTest/SimulationScene/RunSimulation.cs", txtLines);
        AssetDatabase.Refresh();
        yield return new RecompileScripts();
    }

    IEnumerator loadSimlation()
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

            Buffer.instance.currentSimulationID = Int32.Parse(ID.Substring(3, ID.Length - 4));
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
                Buffer.instance.newFormula.authorID = Int32.Parse(simulationID.Substring(13, simulationID.Length - 14));

                Buffer.instance.payoffFormulas.Add((Buffer.instance.newFormula.agent1, Buffer.instance.newFormula.agent2), Buffer.instance.newFormula);


                if (i < amountOfAgents)
                {
                    agentsArray = agentsArray + agent2.Substring(7, agent2.Length - 7);
                }
            }

            agentsArray = agentsArray.Substring(0, agentsArray.Length - 1);
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
                Buffer.instance.agents[i].agentDescription = agentDescription.Substring(13, agentDescription.Length - 15);
                Buffer.instance.agents[i].authorID = Int32.Parse(authorID.Substring(9, authorID.Length - 10));
            }

            Debug.Log("0; Load succesful;" + www.text);
        }
        else
        {
            Debug.Log("Loading simulations failed. Error #" + www.text);
        }

        nameInputField.text = Buffer.instance.currentSimulation.name;
        imageInputField.text = "Temporary image name";
        descriptionInputField.text = Buffer.instance.currentSimulation.description;

        bool variableFound = false;
        foreach (KeyValuePair<(int, int), PayoffFormula> entry in Buffer.instance.payoffFormulas)
        {
            Debug.Log("entry: " + entry.Value.payoffFormula);
            string formula = entry.Value.payoffFormula;
            for (int j = 0; j < formula.Length; j++)
            {
                Debug.Log("(formula[j] == '$'): " + (formula[j] == '$'));
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

        Debug.Log("payoffVariables.Count: " + payoffVariables.Count);
        foreach(string entry in payoffVariables)
        {
            Debug.Log("PayoffVariable: " + entry);
        }
        PayoffMatrix_2.instance.initialize();
        StartCoroutine(compileSimulationRules());
    }
}
