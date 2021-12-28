using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

using UnityEditor;

using System.IO;
using System.Linq;


public class LoadSimulationRules : MonoBehaviour
{
    public bool assetDatabaseRefreshed = false;
    private List<string> txtLines = File.ReadAllLines("Assets/4thTest/SimulationScene/RunSimulation.txt").ToList();

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(loadPayoffFormulas());
    }

    public void compilePayoffFormulas(int m)
    {

        foreach (KeyValuePair<(int, int), PayoffFormula> entry in Buffer.instance.payoffFormulas)
        {

            txtLines.Insert(m + 25, "payoffResults.Add(" + entry.Key + ", 0 );");
            txtLines.Insert(m + 30, "payoffResults[" + entry.Key + "] = " + Buffer.instance.payoffFormulas[entry.Key].payoffFormula + ";");
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

        int m = 0;
        int n = 0;
        int o = 0;
        int p = 0;
        int q = 0;
        foreach (Agent entry in Buffer.instance.agents)
        {
            //Tiek izveidoti koki ar agentiem (piem., vanagiem un baloziem)
            txtLines.Insert(m + 17, "private SortedSet<agent> " + entry.agentName + "s = new SortedSet<agent>(new agentComparer());"); m = m + 1;
            txtLines.Insert(m + 17, "public int startingNumberOf" + entry.agentName + "s = 10;"); m = m + 1;

            n = n + 2;
            txtLines.Insert(n + 34, "for (int i = 0; i < startingNumberOf" + entry.agentName + "s; i++)"); n = n + 1;
            txtLines.Insert(n + 34, "{"); n = n + 1;
            txtLines.Insert(n + 34, "agent newAgent = new agent();"); n = n + 1;
            txtLines.Insert(n + 34, "newAgent.ID = " + entry.agentID + ";"); n = n + 1;
            txtLines.Insert(n + 34, "newAgent.icon = \"" + entry.icon + "\";"); n = n + 1;
            txtLines.Insert(n + 34, "newAgent.agentName = \"" + entry.agentName + "\";"); n = n + 1;
            txtLines.Insert(n + 34, "newAgent.agentDescription = \"" + entry.agentDescription + "\";"); n = n + 1;
            txtLines.Insert(n + 34, "newAgent.authorID =" + entry.authorID + ";"); n = n + 1;
            txtLines.Insert(n + 34, "newAgent.fitness = 0;"); n = n + 1; 
            txtLines.Insert(n + 34, "while (" + entry.agentName + "s.Contains(newAgent)) { newAgent.key++; }"); n = n + 1;
            txtLines.Insert(n + 34, ""); n = n + 1;
            txtLines.Insert(n + 34, entry.agentName + "s.Add(newAgent);"); n = n + 1;
            txtLines.Insert(n + 34, "totalAmountOfIndividuals = totalAmountOfIndividuals + 1;"); n = n + 1;
            txtLines.Insert(n + 34, "}"); n = n + 1;
            txtLines.Insert(n + 34, ""); n = n + 1;

            o = o + 2 + 14;
            txtLines.Insert(o + 60, "killOrDuplicateEachIndividual(" + entry.agentName + "s);"); o = o + 1;

            p = p + 2 + 14 + 1;
            txtLines.Insert(p + 127, "SortedSet<agent> alreadyAssigned" + entry.agentName + "s = new SortedSet<agent>(new agentComparer());"); p = p + 1;

            q = q + 2 + 14 + 1 + 1;
            txtLines.Insert(q + 144, entry.agentName + "s = alreadyAssigned" + entry.agentName + "s;"); q = q + 1;
        }

        int r = p;
        txtLines.Insert(r + 133, "randomIndex = rand.Next(" + totalAgentCount + ");"); r = r + 1;
        txtLines.Insert(r + 133, "if (randomIndex < " + totalAgentCount.Substring(0, indexes[0]) + ")"); r = r + 1;
        txtLines.Insert(r + 133, "{"); r = r + 1;
        txtLines.Insert(r + 133, "agent1 = " + Buffer.instance.agents[0].agentName + "s.ElementAt(randomIndex);"); r = r + 1;
        txtLines.Insert(r + 133, "alreadyAssigned" + Buffer.instance.agents[0].agentName + "s.Add(agent1);"); r = r + 1;
        txtLines.Insert(r + 133, Buffer.instance.agents[0].agentName + "s.Remove(agent1);"); r = r + 1;
        txtLines.Insert(r + 133, "}"); r = r + 1;
        for (int i = 1; i < Buffer.instance.agents.Length; i++)
        {
            txtLines.Insert(r + 133, "else if (randomIndex < " + totalAgentCount.Substring(0, indexes[i]) + ")"); r = r + 1;
            txtLines.Insert(r + 133, "{"); r = r + 1;
            txtLines.Insert(r + 133, "agent1 = " + Buffer.instance.agents[i].agentName + "s.ElementAt(randomIndex - (" + totalAgentCount.Substring(0, indexes[i-1]) + "));"); r = r + 1;
            txtLines.Insert(r + 133, "alreadyAssigned" + Buffer.instance.agents[i].agentName + "s.Add(agent1);"); r = r + 1;
            txtLines.Insert(r + 133, Buffer.instance.agents[i].agentName + "s.Remove(agent1);"); r = r + 1;
            txtLines.Insert(r + 133, "}"); r = r + 1;
        }



        int s = r;
        txtLines.Insert(s + 140, "randomIndex = rand.Next(" + totalAgentCount + ");"); s = s + 1;
        txtLines.Insert(s + 140, "if (randomIndex < " + totalAgentCount.Substring(0, indexes[0]) + ")"); s = s + 1;
        txtLines.Insert(s + 140, "{"); s = s + 1;
        txtLines.Insert(s + 140, "agent1 = " + Buffer.instance.agents[0].agentName + "s.ElementAt(randomIndex);"); s = s + 1;
        txtLines.Insert(s + 140, "alreadyAssigned" + Buffer.instance.agents[0].agentName + "s.Add(agent1);"); s = s + 1;
        txtLines.Insert(s + 140, Buffer.instance.agents[0].agentName + "s.Remove(agent1);"); s = s + 1;
        txtLines.Insert(s + 140, "}"); s = s + 1;
        for (int i = 1; i < Buffer.instance.agents.Length; i++)
        {
            txtLines.Insert(s + 140, "else if (randomIndex < " + totalAgentCount.Substring(0, indexes[i]) + ")"); s = s + 1;
            txtLines.Insert(s + 140, "{"); s = s + 1;
            txtLines.Insert(s + 140, "agent1 = " + Buffer.instance.agents[i].agentName + "s.ElementAt(randomIndex - (" + totalAgentCount.Substring(0, indexes[i-1]) + "));"); s = s + 1;
            txtLines.Insert(s + 140, "alreadyAssigned" + Buffer.instance.agents[i].agentName + "s.Add(agent1);"); s = s + 1;
            txtLines.Insert(s + 140, Buffer.instance.agents[i].agentName + "s.Remove(agent1);"); s = s + 1;
            txtLines.Insert(s + 140, "}"); s = s + 1;
        }

        int t = s;
        txtLines.Insert(t + 141, "randomIndex = rand.Next(" + totalAgentCount + ");"); t = t + 1;
        txtLines.Insert(t + 141, "if (randomIndex < " + totalAgentCount.Substring(0, indexes[0]) + ")"); t = t + 1;
        txtLines.Insert(t + 141, "{"); t = t + 1;
        txtLines.Insert(t + 141, "agent2 = " + Buffer.instance.agents[0].agentName + "s.ElementAt(randomIndex);"); t = t + 1;
        txtLines.Insert(t + 141, "alreadyAssigned" + Buffer.instance.agents[0].agentName + "s.Add(agent2);"); t = t + 1;
        txtLines.Insert(t + 141, Buffer.instance.agents[0].agentName + "s.Remove(agent2);"); t = t + 1;
        txtLines.Insert(t + 141, "}"); t = t + 1;
        for (int i = 1; i < Buffer.instance.agents.Length; i++)
        {
            txtLines.Insert(t + 141, "else if (randomIndex < " + totalAgentCount.Substring(0, indexes[i]) + ")"); t = t + 1;
            txtLines.Insert(t + 141, "{"); t = t + 1;
            txtLines.Insert(t + 141, "agent2 = " + Buffer.instance.agents[i].agentName + "s.ElementAt(randomIndex - (" + totalAgentCount.Substring(0, indexes[i - 1]) + "));"); t = t + 1;
            txtLines.Insert(t + 141, "alreadyAssigned" + Buffer.instance.agents[i].agentName + "s.Add(agent2);"); t = t + 1;
            txtLines.Insert(t + 141, Buffer.instance.agents[i].agentName + "s.Remove(agent2);"); t = t + 1;
            txtLines.Insert(t + 141, "}"); t = t + 1;
        }

        compilePayoffFormulas(m);
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

    // Update is called once per frame
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
                    /**/

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
}
