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
	
public float V = 0f;
public float C = 0f;
	//The starting totalAmountOfIndividuals = 0
    private int totalAmountOfIndividuals = 0;
    private Dictionary<(int, int), float> payoffResults = new Dictionary<(int, int), float>();
	
	//The code generator has created 4 new lines which initializes the agent sets and their starting amount
	
private SortedSet<agent> Doves = new SortedSet<agent>(new agentComparer());
public int startingNumberOfDoves = 200;
private SortedSet<agent> Hawks = new SortedSet<agent>(new agentComparer());
public int startingNumberOfHawks = 200;
    private int varItr = 0;
	
    private bool initialized = false; public GameObject pauseButton;


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
    public void restart(GameObject pauseBtn)
    {
		pauseButton = pauseBtn; if(pauseButton.transform.GetChild(0).GetComponent<Text>().text != "Start")
		{
			pauseButton.transform.GetChild(0).GetComponent<Text>().text = "Start";
			WindoGraph.instance.clearGraph();
			initialized = false; //If we want to initialize the simulation from the start we have to forget that we have initialized once already
			paused = true;
			itr = 0;
		}
		else { StartCoroutine(Notification.instance.showNotification("You can only restart, if the simulation is started.")); }
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
payoffResults.Add((9, 9), 0 );
payoffResults.Add((9, 10), 0 );
payoffResults.Add((10, 9), 0 );
payoffResults.Add((10, 10), 0 );

    }

	//We have to recalculate the formulas, because when the user changes the $V and $C values,
	//so do their results change
    private void recalculateFormulas()
    {
		//The code generator creates these 4 lines
		//(dove.ID = 9, hawk.ID = 10)
payoffResults[(9, 9)] = V / 2;
payoffResults[(9, 10)] = 0;
payoffResults[(10, 9)] = V;
payoffResults[(10, 10)] = (V - C) / 2;
    }

	//Start() gets called before the first frame update
    private void Start()
    {

        float val;

		//All the lines further below in this function are created by the code generator!

        GameObject newVariableSetting;

//We have to create a "Set$V" button
newVariableSetting = Instantiate(variableSetting);
newVariableSetting.SetActive(true);
newVariableSetting.name = "$VSetting";
newVariableSetting.transform.Find("CurrentValue").GetComponent<Text>().text = "$V: 0";
newVariableSetting.transform.Find("InputField").GetComponent<InputField>().text = "0";
newVariableSetting.transform.Find("Button").GetChild(0).GetComponent<Text>().text = "Set $V";
newVariableSetting.transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate
{
if(pauseButton.transform.GetChild(0).GetComponent<Text>().text == "Start")
{
val = setVariable(simulationVariables.transform.Find("$VSetting").Find("InputField").gameObject);
V = val;
simulationVariables.transform.Find("$VSetting").Find("CurrentValue").GetComponent<Text>().text = "$V: " + val;
}
else { StartCoroutine(Notification.instance.showNotification("You may only change this value, if the simulation isn't (re)started yet.")); }
});
newVariableSetting.transform.SetParent(simulationVariables.transform);
newVariableSetting.transform.position = variableSetting.transform.position - new Vector3(0f, (60 * varItr), 0);
varItr++;

//We have to create a "Set$C" button
newVariableSetting = Instantiate(variableSetting);
newVariableSetting.SetActive(true);
newVariableSetting.name = "$CSetting";
newVariableSetting.transform.Find("CurrentValue").GetComponent<Text>().text = "$C: 0";
newVariableSetting.transform.Find("InputField").GetComponent<InputField>().text = "0";
newVariableSetting.transform.Find("Button").GetChild(0).GetComponent<Text>().text = "Set $C";
newVariableSetting.transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate
{
if(pauseButton.transform.GetChild(0).GetComponent<Text>().text == "Start")
{
val = setVariable(simulationVariables.transform.Find("$CSetting").Find("InputField").gameObject);
C = val;
simulationVariables.transform.Find("$CSetting").Find("CurrentValue").GetComponent<Text>().text = "$C: " + val;
}
else { StartCoroutine(Notification.instance.showNotification("You may only change this value, if the simulation isn't (re)started yet.")); }
});
newVariableSetting.transform.SetParent(simulationVariables.transform);
newVariableSetting.transform.position = variableSetting.transform.position - new Vector3(0f, (60 * varItr), 0);
varItr++;

    }

    private void initialize()
    {
		
        totalAmountOfIndividuals = 0;
		
//These next 15 lines are created by the code generator
//The simulation starts with 10 Doves (at the start (or at the restart))
Doves = new SortedSet<agent>(new agentComparer());
for (int i = 0; i < startingNumberOfDoves; i++)
{
agent newAgent = new agent();
newAgent.ID = 9;
newAgent.agentName = "Dove";
newAgent.agentDescription = "";
newAgent.authorID = 5;
newAgent.fitness = 0;

while (Doves.Contains(newAgent)) { newAgent.key++; }

Doves.Add(newAgent);
totalAmountOfIndividuals = totalAmountOfIndividuals + 1;
}

//These next 15 lines are created by the code generator
//The simulation starts with 10 Hawks (at the start (or at the restart))
Hawks = new SortedSet<agent>(new agentComparer());
for (int i = 0; i < startingNumberOfHawks; i++)
{
agent newAgent = new agent();
newAgent.ID = 10;
newAgent.agentName = "Hawk";
newAgent.agentDescription = "";
newAgent.authorID = 5;
newAgent.fitness = 0;

while (Hawks.Contains(newAgent)) { newAgent.key++; }

Hawks.Add(newAgent);
totalAmountOfIndividuals = totalAmountOfIndividuals + 1;
}

		
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
WindoGraph.instance.addInitialValue(startingNumberOfDoves, "Doves", new Color(1f, 1f, 1f));
WindoGraph.instance.addInitialValue(startingNumberOfHawks, "Hawks", new Color(1f, 1f, 1f));
WindoGraph.instance.yMaximum = (Doves.Count > WindoGraph.instance.yMaximum) ? Doves.Count : WindoGraph.instance.yMaximum;
WindoGraph.instance.yMaximum = (Hawks.Count > WindoGraph.instance.yMaximum) ? Hawks.Count : WindoGraph.instance.yMaximum;
WindoGraph.instance.realignObjects("Doves");
WindoGraph.instance.realignObjects("Hawks");

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
killOrDuplicateEachIndividual(Doves);
killOrDuplicateEachIndividual(Hawks);
			
            //4th phase
WindoGraph.instance.newValue = Doves.Count;
WindoGraph.instance.addNewValue("Doves", new Color(1f, 1f, 1f));
WindoGraph.instance.newValue = Hawks.Count;
WindoGraph.instance.addNewValue("Hawks", new Color(1f, 1f, 0f));
WindoGraph.instance.yMaximum = (Doves.Count > WindoGraph.instance.yMaximum) ? Doves.Count : WindoGraph.instance.yMaximum;
WindoGraph.instance.yMaximum = (Hawks.Count > WindoGraph.instance.yMaximum) ? Hawks.Count : WindoGraph.instance.yMaximum;
WindoGraph.instance.realignObjects("Doves");
WindoGraph.instance.realignObjects("Hawks");

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
SortedSet<agent> alreadyAssignedDoves = new SortedSet<agent>(new agentComparer());
SortedSet<agent> alreadyAssignedHawks = new SortedSet<agent>(new agentComparer());
		//They are needed so we can't assign some individual to another which is already in a contest with someone else
        
		
		
		System.Random rand = new System.Random();
        int randomIndex;
		
        agent agent1 = new agent();
        agent agent2 = new agent();

		//If there is an uneven number of individuals in the simulation we
		//have to ignore someone so that he isn't later assigned to NULL
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
		
		//For each individual
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

			//Recalculate the first one's fitness
            agent1.fitness = agent1.fitness + payoffResults[(agent1.ID, agent2.ID)];
			//Recalculate the second one's fitness
            agent2.fitness = agent2.fitness + payoffResults[(agent2.ID, agent1.ID)];
        }
		
		//Once we're done we put the agents back in their original sets (with their new fitnesses)
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

public class IndexedSortedSet
{
	public IndexedSortedSet instance;
	public int val;
	public int Count = 0;
	public int currPower = 0;
	public int nextPower = 1;
	public int currPowerOfTwo = 1;
	public int nextPowerOfTwo = 2;
	public int index = -1;
	public List<IndexedSortedSet> pointers = new List<IndexedSortedSet>();
	public int supposedCount;

	public int ElementAt(int idx, int prevPowerOfTwo = -1)
	{
		//Console.WriteLine("0. val: " + val + ", Count: " + Count + ", currPower: " + currPower + ", nextPower: " + nextPower + ", currPowerOfTwo: " + currPowerOfTwo + ", nextPowerOfTwo: " + nextPowerOfTwo + ", index: " + index + ", idx: " + idx + ", pointers.Count: " + ((pointers != null) ? (pointers.Count) : (0)));

		if (index == idx)
		{
			Debug.Log("1. val: " + val + ", Count: " + Count + ", currPower: " + currPower + ", nextPower: " + nextPower + ", currPowerOfTwo: " + currPowerOfTwo + ", nextPowerOfTwo: " + nextPowerOfTwo + ", index: " + index + ", idx: " + idx + ", pointers.Count: " + ((pointers != null) ? (pointers.Count) : (0)));

			return val;
		}
		else
		{
			/*
			int subtractor = idx + 1;
			if (currPowerOfTwo - (idx + 1) < 0) { subtractor = (idx - (index - Count)) + 1; }
			int pointerIdx = -1;
			for(int i = currPowerOfTwo; i > currPowerOfTwo - subtractor; i = i - (i / 2))
			{
				pointerIdx++;
			}
			/**/
			int pointerIdx = 0;
			while (pointers[pointerIdx].index < idx) pointerIdx++;
			Debug.Log("2. val: " + val + ", Count: " + Count + ", currPower: " + currPower + ", nextPower: " + nextPower + ", currPowerOfTwo: " + currPowerOfTwo + ", nextPowerOfTwo: " + nextPowerOfTwo + ", index: " + index + ", idx: " + idx + ", pointers.Count: " + ((pointers != null) ? (pointers.Count) : (0)) + ", pointerIdx: " + pointerIdx);

			return pointers[pointerIdx].ElementAt(idx);
		}
	}

	public void Add(int v, bool pushOldValDeeper = true, int idx = -1)
	{
		if (idx == -1) idx = Count;
		Count = Count + 1;




		//if amount of children is less than 2^(nodes.Count)
		if (Count < currPowerOfTwo)
		{
			//The node index that this should be put under:
			int subtractor = idx + 1;
			if (currPowerOfTwo - (idx + 1) < 0) { subtractor = (idx - (index - Count)) + 1; }
			int pointerIndex = -1;
			//if (Count == 11) pointerIndex = 0;
			for (int i = currPowerOfTwo; i > currPowerOfTwo - subtractor; i = i - (i / 2))
			{
				pointerIndex = pointerIndex + 1;
			}

			Debug.Log("currPowerOfTwo: " + currPowerOfTwo + ", Count: " + Count + ", pointerIndex: " + pointerIndex);

			//if the pointers index isnt out of bounds
			if ((Count - (currPowerOfTwo / 2)) % 2 == 0)
			{
				Debug.Log("if(pointerIndex < pointers.Count), pointerIndex: " + pointerIndex + ", pointers.Count: " + pointers.Count);
				//pointers[pointerIndex].Add(v, false, idx);
				/*NEW---------------------------------------------------------------------------------------------------------------------*/
				Debug.Log(Count + "el, " + currPowerOfTwo + ", index: " + index + ", idx: " + idx);
				//Copy this subtree
				List<IndexedSortedSet> prevPointers = pointers;
				//Initialize a new empty subtree
				pointers = new List<IndexedSortedSet>();
				for (int i = 0; i < pointerIndex; i++)
				{
					pointers.Add(prevPointers[i]);
				}
				for (int i = 0; i < pointerIndex; i++)
				{
					prevPointers.RemoveAt(0);
				}
				//Add a new node with a new value to it (which also owns the copy of the copied subtree)
				pointers.Add(new IndexedSortedSet((pushOldValDeeper == false) ? val : v, prevPointers[0].Count + 1, prevPointers[0].currPower + 1, prevPointers[0].nextPower + 1, prevPointers[0].currPowerOfTwo * 2, prevPointers[0].nextPowerOfTwo * 2, (pushOldValDeeper == false) ? index : idx, prevPointers));
				if (pushOldValDeeper == false) val = v;
				if (pushOldValDeeper == false) index = idx;
				/*------------------------------------------------------------------------------------------------------------------------*/
			}
			else //else create a new pointer (if the index is out of bounds)
			{
				Debug.Log("createNewPointer");
				pointers.Add(new IndexedSortedSet(v, 0, 0, 1, 1, 2, idx, new List<IndexedSortedSet>()));
			}
		}
		else
		{
			Debug.Log(Count + "else, " + currPowerOfTwo + ", index: " + index + ", idx: " + idx);
			//Copy this subtree
			List<IndexedSortedSet> prevPointers = pointers;
			//Initialize a new empty subtree
			pointers = new List<IndexedSortedSet>();
			//Add a new node with a new value to it (which also owns the copy of the copied subtree)
			pointers.Add(new IndexedSortedSet((pushOldValDeeper == false) ? val : v, Count - 1, currPower, nextPower, currPowerOfTwo, nextPowerOfTwo, (pushOldValDeeper == false) ? index : idx, prevPointers));
			if (pushOldValDeeper == false) val = v;
			if (pushOldValDeeper == false) index = idx;
			currPower++;
			nextPower++;
			currPowerOfTwo = currPowerOfTwo * 2;
			nextPowerOfTwo = nextPowerOfTwo * 2;
		}

	}

	public IndexedSortedSet(int v = 0, int Cnt = 0, int currPow = 0, int nextPow = 1, int currPowOf2 = 1, int nextPowOf2 = 2, int idx = -1, List<IndexedSortedSet> ptrs = null, int suppCnt = 0)
	{
		val = v;
		Count = Cnt;
		currPower = currPow;
		nextPower = nextPow;
		currPowerOfTwo = currPowOf2;
		nextPowerOfTwo = nextPowOf2;
		index = idx;
		pointers = ptrs;
	}
}
