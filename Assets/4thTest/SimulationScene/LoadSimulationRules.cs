using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static System.Math;
using System.Text.RegularExpressions;
using UnityEditor;

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

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(loadSimlation());
    }

    public void compilePayoffFormulas(int m)
    {

        foreach (KeyValuePair<(int, int), PayoffFormula> entry in Buffer.instance.payoffFormulas)
        {

            txtLines.Insert(m + 25, "payoffResults.Add(" + entry.Key + ", 0 );");
            txtLines.Insert(m + 31, "payoffResults[" + entry.Key + "] = " + Buffer.instance.payoffFormulas[entry.Key].payoffFormula + ";");
            m++;
        }
        /**/

    }

    public void compileSimulationRules()
    {

        Buffer.instance.printAgents();

        int[] indexes = new int[Buffer.instance.agents.Length];
        string totalAgentCount = Buffer.instance.agents[0].agentName + "s.Count";
        indexes[0] = totalAgentCount.Length;
        for (int i = 1; i < Buffer.instance.agents.Length; i++)
        {
            totalAgentCount = totalAgentCount + " + " + Buffer.instance.agents[i].agentName + "s.Count";
            indexes[i] = totalAgentCount.Length;
        }

        int a = 0;
        int b = 0;
        int c = 0;
        int d = 0;
        int e = 0;
        int f = 0;
        int g = 0;
        int h = 0;
        foreach (Agent entry in Buffer.instance.agents)
        {
            //Tiek izveidoti koki ar agentiem (piem., vanagiem un baloziem)
            txtLines.Insert(a + 17, "private SortedSet<agent> " + entry.agentName + "s = new SortedSet<agent>(new agentComparer());"); a = a + 1;
            txtLines.Insert(a + 17, "public int startingNumberOf" + entry.agentName + "s = 10;"); a = a + 1;

            b = b + 2;
            txtLines.Insert(b + 35, "for (int i = 0; i < startingNumberOf" + entry.agentName + "s; i++)"); b = b + 1;
            txtLines.Insert(b + 35, "{"); b = b + 1;
            txtLines.Insert(b + 35, "agent newAgent = new agent();"); b = b + 1;
            txtLines.Insert(b + 35, "newAgent.ID = " + entry.agentID + ";"); b = b + 1;
            txtLines.Insert(b + 35, "newAgent.icon = \"" + entry.icon + "\";"); b = b + 1;
            txtLines.Insert(b + 35, "newAgent.agentName = \"" + entry.agentName + "\";"); b = b + 1;
            txtLines.Insert(b + 35, "newAgent.agentDescription = \"" + entry.agentDescription + "\";"); b = b + 1;
            txtLines.Insert(b + 35, "newAgent.authorID =" + entry.authorID + ";"); b = b + 1;
            txtLines.Insert(b + 35, "newAgent.fitness = 0;"); b = b + 1;
            txtLines.Insert(b + 35, "while (" + entry.agentName + "s.Contains(newAgent)) { newAgent.key++; }"); b = b + 1;
            txtLines.Insert(b + 35, ""); b = b + 1;
            txtLines.Insert(b + 35, entry.agentName + "s.Add(newAgent);"); b = b + 1;
            txtLines.Insert(b + 35, "totalAmountOfIndividuals = totalAmountOfIndividuals + 1;"); b = b + 1;
            txtLines.Insert(b + 35, "}"); b = b + 1;
            txtLines.Insert(b + 35, ""); b = b + 1;

            c = c + 2 + 14;
            txtLines.Insert(c + 61, "killOrDuplicateEachIndividual(" + entry.agentName + "s);"); c = c + 1;

            d = d + 2 + 14 + 2;
            txtLines.Insert(d + 65, "WindoGraph.instance.newValue = " + entry.agentName + "s.Count;"); d = d + 1;
            txtLines.Insert(d + 65, "WindoGraph.instance.addNewValue(\"" + entry.agentName + "s\");"); d = d + 1;

            e = e + 2 + 14 + 2 + 2;
            txtLines.Insert(e + 65, "WindoGraph.instance.yMaximum = (" + entry.agentName + "s.Count > WindoGraph.instance.yMaximum) ? " + entry.agentName + "s.Count : WindoGraph.instance.yMaximum;"); e = e + 1;

            f = f + 2 + 14 + 2 + 2 + 1;
            txtLines.Insert(f + 65, "WindoGraph.instance.realignObjects(\"" + entry.agentName + "s\");"); f = f + 1;

            g = g + 2 + 14 + 2 + 2 + 1 + 1;
            txtLines.Insert(g + 122, "SortedSet<agent> alreadyAssigned" + entry.agentName + "s = new SortedSet<agent>(new agentComparer());"); g = g + 1;

            h = h + 2 + 14 + 2 + 2 + 1 + 1 + 1;
            txtLines.Insert(h + 137, entry.agentName + "s = alreadyAssigned" + entry.agentName + "s;"); h = h + 1;
        }



        int q = b;
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            txtLines.Insert(q + 52, "WindoGraph.instance.addInitialValue(startingNumberOf" + Buffer.instance.agents[i].agentName + "s, \"" + Buffer.instance.agents[i].agentName + "s\");"); q = q + 1;
        }
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            txtLines.Insert(q + 52, "WindoGraph.instance.yMaximum = (" + Buffer.instance.agents[i].agentName + "s.Count > WindoGraph.instance.yMaximum) ? " + Buffer.instance.agents[i].agentName + "s.Count : WindoGraph.instance.yMaximum;"); q = q + 1;
        }
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            txtLines.Insert(q + 52, "WindoGraph.instance.realignObjects(\"" + Buffer.instance.agents[i].agentName + "s\");"); q = q + 1;
        }

        int r = g;
        txtLines.Insert(r + 135, "randomIndex = rand.Next(" + totalAgentCount + ");"); r = r + 1;
        txtLines.Insert(r + 135, "if (randomIndex < " + totalAgentCount.Substring(0, indexes[0]) + ")"); r = r + 1;
        txtLines.Insert(r + 135, "{"); r = r + 1;
        txtLines.Insert(r + 135, "agent1 = " + Buffer.instance.agents[0].agentName + "s.ElementAt(randomIndex);"); r = r + 1;
        txtLines.Insert(r + 135, "alreadyAssigned" + Buffer.instance.agents[0].agentName + "s.Add(agent1);"); r = r + 1;
        txtLines.Insert(r + 135, Buffer.instance.agents[0].agentName + "s.Remove(agent1);"); r = r + 1;
        txtLines.Insert(r + 135, "}"); r = r + 1;
        for (int i = 1; i < Buffer.instance.agents.Length; i++)
        {
            txtLines.Insert(r + 135, "else if (randomIndex < " + totalAgentCount.Substring(0, indexes[i]) + ")"); r = r + 1;
            txtLines.Insert(r + 135, "{"); r = r + 1;
            txtLines.Insert(r + 135, "agent1 = " + Buffer.instance.agents[i].agentName + "s.ElementAt(randomIndex - (" + totalAgentCount.Substring(0, indexes[i-1]) + "));"); r = r + 1;
            txtLines.Insert(r + 135, "alreadyAssigned" + Buffer.instance.agents[i].agentName + "s.Add(agent1);"); r = r + 1;
            txtLines.Insert(r + 135, Buffer.instance.agents[i].agentName + "s.Remove(agent1);"); r = r + 1;
            txtLines.Insert(r + 135, "}"); r = r + 1;
        }

        int s = r;
        txtLines.Insert(s + 139, "randomIndex = rand.Next(" + totalAgentCount + ");"); s = s + 1;
        txtLines.Insert(s + 139, "if (randomIndex < " + totalAgentCount.Substring(0, indexes[0]) + ")"); s = s + 1;
        txtLines.Insert(s + 139, "{"); s = s + 1;
        txtLines.Insert(s + 139, "agent1 = " + Buffer.instance.agents[0].agentName + "s.ElementAt(randomIndex);"); s = s + 1;
        txtLines.Insert(s + 139, "alreadyAssigned" + Buffer.instance.agents[0].agentName + "s.Add(agent1);"); s = s + 1;
        txtLines.Insert(s + 139, Buffer.instance.agents[0].agentName + "s.Remove(agent1);"); s = s + 1;
        txtLines.Insert(s + 139, "}"); s = s + 1;
        for (int i = 1; i < Buffer.instance.agents.Length; i++)
        {
            txtLines.Insert(s + 139, "else if (randomIndex < " + totalAgentCount.Substring(0, indexes[i]) + ")"); s = s + 1;
            txtLines.Insert(s + 139, "{"); s = s + 1;
            txtLines.Insert(s + 139, "agent1 = " + Buffer.instance.agents[i].agentName + "s.ElementAt(randomIndex - (" + totalAgentCount.Substring(0, indexes[i-1]) + "));"); s = s + 1;
            txtLines.Insert(s + 139, "alreadyAssigned" + Buffer.instance.agents[i].agentName + "s.Add(agent1);"); s = s + 1;
            txtLines.Insert(s + 139, Buffer.instance.agents[i].agentName + "s.Remove(agent1);"); s = s + 1;
            txtLines.Insert(s + 139, "}"); s = s + 1;
        }

        int t = s;
        txtLines.Insert(t + 139, "randomIndex = rand.Next(" + totalAgentCount + ");"); t = t + 1;
        txtLines.Insert(t + 139, "if (randomIndex < " + totalAgentCount.Substring(0, indexes[0]) + ")"); t = t + 1;
        txtLines.Insert(t + 139, "{"); t = t + 1;
        txtLines.Insert(t + 139, "agent2 = " + Buffer.instance.agents[0].agentName + "s.ElementAt(randomIndex);"); t = t + 1;
        txtLines.Insert(t + 139, "alreadyAssigned" + Buffer.instance.agents[0].agentName + "s.Add(agent2);"); t = t + 1;
        txtLines.Insert(t + 139, Buffer.instance.agents[0].agentName + "s.Remove(agent2);"); t = t + 1;
        txtLines.Insert(t + 139, "}"); t = t + 1;
        for (int i = 1; i < Buffer.instance.agents.Length; i++)
        {
            txtLines.Insert(t + 139, "else if (randomIndex < " + totalAgentCount.Substring(0, indexes[i]) + ")"); t = t + 1;
            txtLines.Insert(t + 139, "{"); t = t + 1;
            txtLines.Insert(t + 139, "agent2 = " + Buffer.instance.agents[i].agentName + "s.ElementAt(randomIndex - (" + totalAgentCount.Substring(0, indexes[i - 1]) + "));"); t = t + 1;
            txtLines.Insert(t + 139, "alreadyAssigned" + Buffer.instance.agents[i].agentName + "s.Add(agent2);"); t = t + 1;
            txtLines.Insert(t + 139, Buffer.instance.agents[i].agentName + "s.Remove(agent2);"); t = t + 1;
            txtLines.Insert(t + 139, "}"); t = t + 1;
        }

        compilePayoffFormulas(a);
        /**/
    }

    /* 
    public void compileANewBufferInitializer()
    {
        List<string> txtLines = File.ReadAllLines("Assets/4thTest/Buffer.txt").ToList();   //Fill a list with the lines from the txt file.
        txtLines.Insert(37, "authenticatedUser.ID = " + Buffer.instance.authenticatedUser.ID + ";");
        txtLines.Insert(38, "authenticatedUser.username = \"" + Buffer.instance.authenticatedUser.username + "\";");
        txtLines.Insert(39, "currentSimulation.ID = " + Buffer.instance.currentSimulation.ID + ";");
        File.WriteAllLines("Assets/4thTest/Buffer.cs", txtLines);
    }
    /**/

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
            /**/
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
                /**/
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
        imageInputField.text = "Temporary image name" /*Buffer.instance.currentSimulation.image*/;
        descriptionInputField.text = Buffer.instance.currentSimulation.description;

        /*
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
        /**/

        compileSimulationRules();
        File.WriteAllLines("Assets/4thTest/SimulationScene/RunSimulation.cs", txtLines);
        AssetDatabase.ImportAsset("Assets/4thTest/SimulationScene/Buffer.cs");
    }

    /*
    IEnumerator loadPayoffFormulas()
    {
        //Get its payoffFormulas
        WWWForm form = new WWWForm();
        form.AddField("class", "PayoffFormulasController\\payoffFormulas");
        form.AddField("function", "read");
        form.AddField("simulationID", Buffer.instance.currentSimulation.ID);
        //form.AddField("authorID", Buffer.instance.authenticatedUser.ID);
        //form.AddField("simulationID", 1);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {
            Debug.Log(www.text);

            string text = "my string to match";
            Regex pattern = new Regex(@"0;(.*?){(.*?)}}");

            Match formulas = pattern.Match(www.text);   // m is the first match
            Match agents = formulas.NextMatch();

            string temp = Regex.Match(agents.ToString(), @"0;(.*?){").Value;
            Debug.Log("temp: " + temp);
            int agentCount = Int32.Parse(temp.Substring(2, temp.Length - 3));
            Buffer.instance.newAgentsArray(agentCount);


            foreach (Match match in Regex.Matches(formulas.ToString(), @"{(.*?)}"))
            {
                Debug.Log(match.ToString());

                string seperate_entry = match.ToString();

                Debug.Log("seperate_entry: " + seperate_entry);

                string ID = Regex.Match(seperate_entry, @"ID:(.*?),").Value;
                string agent1 = Regex.Match(seperate_entry, @"agent1:(.*?),").Value;
                string agent2 = Regex.Match(seperate_entry, @"agent2:(.*?),").Value;
                string payoffFormula = Regex.Match(seperate_entry, @"payoffFormula:(.*?),").Value;
                string authorID = Regex.Match(seperate_entry, @"authorID:(.*?)}").Value;

                //int a1 = Int32.Parse(agent1);
                //int a2 = Int32.Parse(agent1);

                Debug.Log("PAYOFF_FORMULA: " + payoffFormula);
                if (ID != "")
                {

                    Debug.Log("PAYOFF_FORMULA: " + payoffFormula);
                    Buffer.instance.newFormula = new PayoffFormula();

                    Buffer.instance.newFormula.ID = Int32.Parse(ID.Substring(3, ID.Length - 4));
                    Buffer.instance.newFormula.agent1 = Int32.Parse(agent1.Substring(7, agent1.Length - 8));
                    Buffer.instance.newFormula.agent2 = Int32.Parse(agent2.Substring(7, agent2.Length - 8));
                    Buffer.instance.newFormula.payoffFormula = payoffFormula.Substring(15, payoffFormula.Length - 17);
                    Buffer.instance.newFormula.authorID = Int32.Parse(authorID.Substring(9, authorID.Length - 10));

                    Debug.Log(payoffFormula);
                    Debug.Log(Buffer.instance.newFormula.payoffFormula);

                    Buffer.instance.payoffFormulas.Add((Buffer.instance.newFormula.agent1, Buffer.instance.newFormula.agent2), Buffer.instance.newFormula);
                }
            }

            int i = 0;
            foreach (Match match in Regex.Matches(agents.ToString(), @"{(.*?)}"))
            {
                Debug.Log(match.ToString());

                string seperate_entry = match.ToString();

                Debug.Log("seperate_entry: " + seperate_entry);

                string agentID = Regex.Match(seperate_entry, @"ID:(.*?),").Value;
                string icon = Regex.Match(seperate_entry, @"icon:(.*?),").Value;
                string agentName = Regex.Match(seperate_entry, @"name:(.*?),").Value;
                string agentDescription = Regex.Match(seperate_entry, @"description:(.*?),").Value;
                string authorID = Regex.Match(seperate_entry, @"authorID:(.*?)}").Value;
                if (agentID != "")
                {


                    Buffer.instance.agents[i].agentID = Int32.Parse(agentID.Substring(3, agentID.Length - 4));
                    Buffer.instance.agents[i].icon = icon.Substring(5, icon.Length - 6);
                    Buffer.instance.agents[i].agentName = agentName.Substring(5, agentName.Length - 6);
                    Buffer.instance.agents[i].agentDescription = agentDescription.Substring(13, agentDescription.Length - 15);
                    Buffer.instance.agents[i].authorID = Int32.Parse(authorID.Substring(9, authorID.Length - 10));
                }
                i++;
            }



            Buffer.instance.printPayoffFormulas();

            Debug.Log("WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW1");

            compileSimulationRules();
            File.WriteAllLines("Assets/4thTest/SimulationScene/RunSimulation.cs", txtLines);
            //compileANewBufferInitializer();
            //Destroy(Buffer.instance.gameObject);
            Debug.Log("Buffer Destroyed!");
            Debug.Log("WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW2");
            //AssetDatabase.ImportAsset("Assets/4thTest/SimulationScene/RunSimulation.cs");
            AssetDatabase.ImportAsset("Assets/4thTest/SimulationScene/Buffer.cs");


        }
        else
        {
            Debug.Log("Loading simulations failed. Error #" + www.text);
        }
        //Buffer.instance.reinitialize();

        //(later) Get its agent details
    }
    /**/
}
