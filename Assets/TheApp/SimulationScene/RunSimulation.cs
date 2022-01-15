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
	
    public float speed = 50f;

    public GameObject startPauseButton;
    public GameObject restartButton;
    public GameObject accelerateButton;
    public GameObject deccelerateButton;
    public GameObject speedText;
	
	//This is the panel where the "Set $V, Set $C" buttons are initialized
    public GameObject simulationVariables;
	//This is a template for a "Set $var"
    public GameObject variableSetting;
	
    public bool paused = false;

	//These are created by the code generator
	
	//The starting totalAmountOfIndividuals = 0
    private int totalAmountOfIndividuals = 0;
    private Dictionary<(int, int), float> payoffResults = new Dictionary<(int, int), float>();
	
	//The code generator has created 4 new lines which initializes the agent sets and their starting amount
	
    private int varItr = 0;
	
    private bool initialized = false;


    private int itr = 0;


    void Update()
    {
        if (RunSimulation.instance == null)
        {
            instance = this; Start();
        }
    }

	//This function gets called when the user clicks on the "pause / unpause" button
    public void pauseUnpauseSimulation()
    {
        GameObject buttonCLicked = EventSystem.current.currentSelectedGameObject;
        Text buttonName = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>();
        if (buttonName.text == "Start") { buttonName.text = "Pause"; paused = false; }
        else if (buttonName.text == "Pause") { buttonName.text = "Unpause"; paused = true; }
        else if (buttonName.text == "Unpause") { buttonName.text = "Pause"; paused = false; }
    }

	//This function gets called when the user clicks on the "restart" button
    public void restart(GameObject pauseButton)
    {
        pauseButton.transform.GetChild(0).GetComponent<Text>().text = "Start";
        WindoGraph.instance.clearGraph();
        initialized = false; //If we want to initialize the simulation from the start we have to forget that we have initialized once already
        paused = true;
        itr = 0;
    }

	//This function gets called when the user clicks on the "accelerate" button
    public void accelerate(GameObject speedText)
    {
        if (speed / 2 < 50f*0.125f) { StartCoroutine(Notification.instance.showNotification("The speed cannot be bigger than 8")); }
        else
        {
			// 50 = 50 / 2
			// We actually make this number smaller because that way (itr % speed == 0) will be true more frequently (which is actually faster)
			speed = speed / 2;
			
			float speedVal = float.Parse(speedText.GetComponent<Text>().text.Substring(7, speedText.GetComponent<Text>().text.Length - 7));
			speedVal = speedVal * 2;
			speedText.GetComponent<Text>().text = "Speed: " + speedVal;
		}
    }

	//This function gets called when the user clicks on the "deccelerate" button
    public void deccelerate(GameObject speedText)
    {
        Notification.instance.showNotification("The speed cannot be smaller than 0.125");
        if (speed * 2 > 50f*8f) { StartCoroutine(Notification.instance.showNotification("The speed cannot be smaller than 0.125")); }
        else
        {
			//50 = 50 * 2
			//We actually make this number smaller because that way (itr % speed == 0) will be true less frequently (which is actually slower)
            speed = speed * 2;
            float speedVal = float.Parse(speedText.GetComponent<Text>().text.Substring(7, speedText.GetComponent<Text>().text.Length - 7));
            speedVal = speedVal / 2;
            speedText.GetComponent<Text>().text = "Speed: " + speedVal;
        }
    }

	//For example "Set $V: 2"
    private float setVariable(GameObject inputField)
    {
        float value = float.Parse(inputField.GetComponent<InputField>().text);
        return value;
    }

    private void instantiateFormulas()
    {
		//Every time 2 individuals their fitness is recalculated accoring to these formulas
		
		//The code generator creates these 4 lines
		//(dove.ID = 9, hawk.ID = 10)

    }

	//We have to recalculate the formulas, because when the user changes the $V and $C values,
	//so do their results change
    private void recalculateFormulas()
    {
		//The code generator creates these 4 lines
		//(dove.ID = 9, hawk.ID = 10)
    }

	//Start() gets called before the first frame update
    private void Start()
    {

        float val;

		//All the lines further below in this function are created by the code generator!

        GameObject newVariableSetting;

    }

    private void initialize()
    {
		
        totalAmountOfIndividuals = 0;
		
		
    }

    private void FixedUpdate()
    {
		//Example: speed = 50
		//If we've iterated through this function 50 times in a row: playOneRound();
        if (paused == false && itr % speed == 0) { playOneRound(); }
        itr++;
    }

    public void playOneRound()
    {
        if (initialized == false)
        {
            initialize();
            initialized = true;
			
			//These next 6 lines are created by the code generator

			//redraw the graph
            WindoGraph.instance.realignLabels();
            WindoGraph.instance.oldYMaximum = WindoGraph.instance.yMaximum;
        }
        else
        {
            //1st phase

            //2nd phase
            recalculateFormulas();
            assignIndividualsInPairwiseContestsAndCalculateTheirFitness();

            //3rd phase
			
            //4th phase

			//Redraw the graph
            WindoGraph.instance.daysPassed++;
            WindoGraph.instance.realignLabels();
            WindoGraph.instance.oldYMaximum = WindoGraph.instance.yMaximum;
        }
    }

    private void killOrDuplicateEachIndividual(SortedSet<agent> agents)
    {
        int iii = 0;
        int ii = 0;
        int totalDuplicatesToMake = 0;
        Stack removableAgents = new Stack();

		//We iterate through each individual
        foreach (agent individual in agents)
        {
			//If they should die we put them in the killable agents stack
            if (individual.fitness <= -1)
            {
                removableAgents.Push(individual);
                totalAmountOfIndividuals--;
            }
			//Else if they should procreate we add the appropriate number in the sum of how many should be created
            else if (individual.fitness >= 1)
            {
                int duplicatesToMake = (int)System.Math.Floor(individual.fitness);
                individual.fitness = individual.fitness - duplicatesToMake;
                totalDuplicatesToMake = totalDuplicatesToMake + duplicatesToMake;

                totalAmountOfIndividuals = totalAmountOfIndividuals + duplicatesToMake;
            }
        }

		//For each agent in killable agents stack we kill them
        foreach (agent individual in removableAgents)
        {
            agents.Remove(individual);
        }
		//According to how big the procreation sum is we initialize new agents
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

		//These 2 lines are created by the code generator
		//They are needed so we can't assign some individual to another which is already in a contest with someone else
        
		
		
		System.Random rand = new System.Random();
        int randomIndex;
		
        agent agent1 = new agent();
        agent agent2 = new agent();

		//If there is an uneven number of individuals in the simulation we
		//have to ignore someone so that he isn't later assigned to NULL
        if (totalAmountOfIndividuals % 2 == 1)
        {

        }
		
		//For each individual
        for (int i = 0; i < totalAmountOfIndividuals / 2; i++)
        {

			//Recalculate the first one's fitness
            agent1.fitness = agent1.fitness + payoffResults[(agent1.ID, agent2.ID)];
			//Recalculate the second one's fitness
            agent2.fitness = agent2.fitness + payoffResults[(agent2.ID, agent1.ID)];
        }
		
		//Once we're done we put the agents back in their original sets (with their new fitnesses)

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
