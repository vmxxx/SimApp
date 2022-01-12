using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Linq;

//using static System.Math;
[System.Serializable]
public class RunSimulation : MonoBehaviour
{
    public static RunSimulation instance;
    private int totalAmountOfIndividuals = 0;
    public float speed = 50f;
    private Dictionary<(int, int), float> payoffResults = new Dictionary<(int, int), float>();
    private SortedSet<agent> Doves = new SortedSet<agent>(new agentComparer());
    public int startingNumberOfDoves = 10;
    private SortedSet<agent> Hawks = new SortedSet<agent>(new agentComparer());
    public int startingNumberOfHawks = 10;

    public GameObject simulationVariables;
    public GameObject variableSetting;
    private int varItr = 0;

    public float V = 0f;
    public float C = 0f;

    public bool paused = false;
    private bool initialized = false;

    public GameObject startPauseButton;
    public GameObject restartButton;
    public GameObject accelerateButton;
    public GameObject deccelerateButton;
    public GameObject speedText;

    private int itr = 0;


    void Update()
    {
        if (RunSimulation.instance == null)
        {
            instance = this;
        }
    }

    public void pauseUnpauseSimulation()
    {
        GameObject buttonCLicked = EventSystem.current.currentSelectedGameObject;
        Text buttonName = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>();
        if (buttonName.text == "Start") { buttonName.text = "Pause"; paused = false; }
        else if (buttonName.text == "Pause") { buttonName.text = "Unpause"; paused = true; }
        else if (buttonName.text == "Unpause") { buttonName.text = "Pause"; paused = false; }
    }

    public void restart(GameObject pauseButton)
    {
        pauseButton.transform.GetChild(0).GetComponent<Text>().text = "Start";
        WindoGraph.instance.clearGraph();
        initialized = false;
        paused = true;
        itr = 0;
    }

    public void accelerate(GameObject speedText)
    {
        speed = speed / 2;
        float speedVal = float.Parse(speedText.GetComponent<Text>().text.Substring(7, speedText.GetComponent<Text>().text.Length - 7));
        speedVal = speedVal * 2;
        speedText.GetComponent<Text>().text = "Speed: " + speedVal;
    }

    public void deccelerate(GameObject speedText)
    {
        Notification.instance.showNotification("The speed cannot be smaller than 0.125");
        if (speed * 2 > 8) { StartCoroutine(Notification.instance.showNotification("The speed cannot be smaller than 0.125")); }
        else
        {
            speed = speed * 2;
            float speedVal = float.Parse(speedText.GetComponent<Text>().text.Substring(7, speedText.GetComponent<Text>().text.Length - 7));
            speedVal = speedVal / 2;
            speedText.GetComponent<Text>().text = "Speed: " + speedVal;
        }
    }

    private float setVariable(GameObject inputField)
    {
        float value = float.Parse(inputField.GetComponent<InputField>().text);
        return value;
    }

    private void instantiateFormulas()
    {
        payoffResults.Add((9, 9), 0);
        payoffResults.Add((9, 10), 0);
        payoffResults.Add((10, 9), 0);
        payoffResults.Add((10, 10), 0);

    }

    private void recalculateFormulas()
    {
        payoffResults[(10, 10)] = (V / 2) - (C / 2);
        payoffResults[(10, 9)] = V;
        payoffResults[(9, 10)] = 0;
        payoffResults[(9, 9)] = V / 2;
    }

    private void Start()
    {

        float val;

        GameObject newVariableSetting;

        Debug.Log("works4");
        newVariableSetting = Instantiate(variableSetting);
        newVariableSetting.SetActive(true);
        newVariableSetting.name = "$VSetting";
        newVariableSetting.transform.Find("CurrentValue").GetComponent<Text>().text = "$V: 0";
        newVariableSetting.transform.Find("InputField").GetComponent<InputField>().text = "0";
        newVariableSetting.transform.Find("Button").GetChild(0).GetComponent<Text>().text = "Set $V";
        newVariableSetting.transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate
        {
            Debug.Log("works5");
            val = setVariable(simulationVariables.transform.Find("$VSetting").Find("InputField").gameObject);
            V = val;
            simulationVariables.transform.Find("$VSetting").Find("CurrentValue").GetComponent<Text>().text = "$V: " + val;
        });
        newVariableSetting.transform.SetParent(simulationVariables.transform);
        newVariableSetting.transform.position = variableSetting.transform.position - new Vector3(0f, (60 * varItr), 0);
        varItr++;


        Debug.Log("works6");
        newVariableSetting = Instantiate(variableSetting);
        newVariableSetting.SetActive(true);
        newVariableSetting.name = "$CSetting";
        newVariableSetting.transform.Find("CurrentValue").GetComponent<Text>().text = "$C: 0";
        newVariableSetting.transform.Find("InputField").GetComponent<InputField>().text = "0";
        newVariableSetting.transform.Find("Button").GetChild(0).GetComponent<Text>().text = "Set $C";
        newVariableSetting.transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate
        {
            Debug.Log("works7");
            val = setVariable(simulationVariables.transform.Find("$CSetting").Find("InputField").gameObject);
            C = val;
            simulationVariables.transform.Find("$CSetting").Find("CurrentValue").GetComponent<Text>().text = "$C: " + val;
        });
        newVariableSetting.transform.SetParent(simulationVariables.transform);
        newVariableSetting.transform.position = variableSetting.transform.position - new Vector3(0f, (60 * varItr), 0);
        varItr++;
    }

    private void initialize()
    {
        Debug.Log("BEFORE Doves.Count: " + Doves.Count);
        totalAmountOfIndividuals = 0;
        Doves = new SortedSet<agent>(new agentComparer());
        for (int i = 0; i < startingNumberOfDoves; i++)
        {
            agent newAgent = new agent();
            newAgent.ID = 9;
            newAgent.icon = "icon";
            newAgent.agentName = "Dove";
            newAgent.agentDescription = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500";
            newAgent.authorID = 1;
            newAgent.fitness = 0;
            while (Doves.Contains(newAgent)) { newAgent.key++; }

            Doves.Add(newAgent);
            totalAmountOfIndividuals = totalAmountOfIndividuals + 1;
        }
        Debug.Log("AFTER Doves.Count: " + Doves.Count);

        Debug.Log("BEFORE Hawks.Count: " + Hawks.Count);
        Hawks = new SortedSet<agent>(new agentComparer());
        for (int i = 0; i < startingNumberOfHawks; i++)
        {
            agent newAgent = new agent();
            newAgent.ID = 10;
            newAgent.icon = "icon";
            newAgent.agentName = "Hawk";
            newAgent.agentDescription = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500";
            newAgent.authorID = 1;
            newAgent.fitness = 0;
            while (Hawks.Contains(newAgent)) { newAgent.key++; }

            Hawks.Add(newAgent);
            totalAmountOfIndividuals = totalAmountOfIndividuals + 1;
        }
        Debug.Log("AFTER Hawks.Count: " + Hawks.Count);


        //WindoGraph.instance.showGraphWithInitialValues();
    }

    private void FixedUpdate()
    {
        Debug.Log("(paused == false && itr % speed == 0): " + (paused == false && itr % speed == 0));
        if (paused == false && itr % speed == 0) { playOneRound(); }
        itr++;
    }

    public void playOneRound()
    {
        Debug.Log("V: " + V + ", C: " + C);
        if (initialized == false)
        {
            initialize();
            //initialize(doves, startingNumberOfDoves);
            initialized = true;
            WindoGraph.instance.addInitialValue(startingNumberOfDoves, "Doves", new Color(1f, 1f, 1f));
            WindoGraph.instance.addInitialValue(startingNumberOfHawks, "Hawks", new Color(1f, 1f, 0f));
            WindoGraph.instance.yMaximum = (Doves.Count > WindoGraph.instance.yMaximum) ? Doves.Count : WindoGraph.instance.yMaximum;
            WindoGraph.instance.yMaximum = (Hawks.Count > WindoGraph.instance.yMaximum) ? Hawks.Count : WindoGraph.instance.yMaximum;


            WindoGraph.instance.realignObjects("Doves");
            WindoGraph.instance.realignObjects("Hawks");


            WindoGraph.instance.realignLabels();
            WindoGraph.instance.oldYMaximum = WindoGraph.instance.yMaximum;
        }
        else
        {
            //1st phase

            killOrDuplicateEachIndividual(Hawks);
            killOrDuplicateEachIndividual(Doves);
            //2nd phase
            recalculateFormulas();
            assignIndividualsInPairwiseContestsAndCalculateTheirFitness();

            //3rd phase
            WindoGraph.instance.newValue = Doves.Count;
            WindoGraph.instance.addNewValue("Doves", new Color(1f, 1f, 1f));
            WindoGraph.instance.newValue = Hawks.Count;
            WindoGraph.instance.addNewValue("Hawks", new Color(1f, 1f, 0f));
            WindoGraph.instance.yMaximum = (Doves.Count > WindoGraph.instance.yMaximum) ? Doves.Count : WindoGraph.instance.yMaximum;
            WindoGraph.instance.yMaximum = (Hawks.Count > WindoGraph.instance.yMaximum) ? Hawks.Count : WindoGraph.instance.yMaximum;
            WindoGraph.instance.realignObjects("Doves");
            WindoGraph.instance.realignObjects("Hawks");
            printAmountOfIndividuals();
            //4th phase


            WindoGraph.instance.daysPassed++;
            WindoGraph.instance.realignLabels();
            WindoGraph.instance.oldYMaximum = WindoGraph.instance.yMaximum;
        }
    }

    private void printAmountOfIndividuals()
    {
        Debug.Log("Hawks / Doves: " + Hawks.Count + " / " + Doves.Count + "   hawksFrequency: " + Hawks.Count + " / " + (Hawks.Count + Doves.Count));
    }

    private void killOrDuplicateEachIndividual(SortedSet<agent> agents)
    {
        int iii = 0;
        int ii = 0;
        int totalDuplicatesToMake = 0;
        Stack removableAgents = new Stack();

        foreach (agent individual in agents)
        {
            if (individual.fitness <= -1)
            {
                removableAgents.Push(individual);
                totalAmountOfIndividuals--;
            }
            else if (individual.fitness >= 1)
            {
                int duplicatesToMake = (int)System.Math.Floor(individual.fitness);
                individual.fitness = individual.fitness - duplicatesToMake;
                totalDuplicatesToMake = totalDuplicatesToMake + duplicatesToMake;

                totalAmountOfIndividuals = totalAmountOfIndividuals + duplicatesToMake;
            }
        }

        foreach (agent individual in removableAgents)
        {
            agents.Remove(individual);
        }
        for (int i = 0; i < totalDuplicatesToMake; i++)
        {

            agent newAgent = new agent();
            newAgent.ID = agents.First().ID;
            newAgent.icon = agents.First().icon;
            newAgent.agentName = agents.First().agentName;
            newAgent.agentDescription = agents.First().agentDescription;
            newAgent.authorID = agents.First().authorID;
            newAgent.fitness = 0;
            newAgent.key = agents.Count;
            while (agents.Contains(newAgent)) { newAgent.key++; }

            agents.Add(newAgent);
        }
    }

    private void assignIndividualsInPairwiseContestsAndCalculateTheirFitness()
    {

        SortedSet<agent> alreadyAssignedDoves = new SortedSet<agent>(new agentComparer());
        SortedSet<agent> alreadyAssignedHawks = new SortedSet<agent>(new agentComparer());
        System.Random rand = new System.Random();
        int randomIndex;
        agent agent1 = new agent();
        agent agent2 = new agent();

        if (totalAmountOfIndividuals % 2 == 1)
        {
            randomIndex = rand.Next(Doves.Count + Hawks.Count);
            if (randomIndex < Doves.Count)
            {
                agent1 = Doves.ElementAt(randomIndex);
                alreadyAssignedDoves.Add(agent1);
                Doves.Remove(agent1);
            }
            else if (randomIndex < Doves.Count + Hawks.Count)
            {
                agent1 = Hawks.ElementAt(randomIndex - (Doves.Count));
                alreadyAssignedHawks.Add(agent1);
                Hawks.Remove(agent1);
            }

        }
        for (int i = 0; i < totalAmountOfIndividuals / 2; i++)
        {
            randomIndex = rand.Next(Doves.Count + Hawks.Count);
            if (randomIndex < Doves.Count)
            {
                agent1 = Doves.ElementAt(randomIndex);
                alreadyAssignedDoves.Add(agent1);
                Doves.Remove(agent1);
            }
            else if (randomIndex < Doves.Count + Hawks.Count)
            {
                agent1 = Hawks.ElementAt(randomIndex - (Doves.Count));
                alreadyAssignedHawks.Add(agent1);
                Hawks.Remove(agent1);
            }
            randomIndex = rand.Next(Doves.Count + Hawks.Count);
            if (randomIndex < Doves.Count)
            {
                agent2 = Doves.ElementAt(randomIndex);
                alreadyAssignedDoves.Add(agent2);
                Doves.Remove(agent2);
            }
            else if (randomIndex < Doves.Count + Hawks.Count)
            {
                agent2 = Hawks.ElementAt(randomIndex - (Doves.Count));
                alreadyAssignedHawks.Add(agent2);
                Hawks.Remove(agent2);
            }

            agent1.fitness = agent1.fitness + payoffResults[(agent1.ID, agent2.ID)];
            agent2.fitness = agent2.fitness + payoffResults[(agent2.ID, agent1.ID)];
        }
        Doves = alreadyAssignedDoves;
        Hawks = alreadyAssignedHawks;
    }
}

public class agent
{
    public int ID;
    public string icon;
    public string agentName;
    public string agentDescription;
    public int authorID;

    public float fitness;
    public int key;
}

public class agentComparer : IComparer<agent>
{
    public int Compare(agent x, agent y)
    {
        // TODO: Handle x or y being null, or them not having names
        return x.key.CompareTo(y.key);
    }
}
