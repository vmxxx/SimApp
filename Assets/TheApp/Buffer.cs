using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public static class RecompileListener
{
    private static bool loaded;

    // This should ban setting Loaded to false outside the class.
    public static bool Loaded
    {
        get { return loaded; }
        set { if (value != false) loaded = value; }
    }

    static RecompileListener()
    {
        // This is called on code recompile
        loaded = false;
    }
}

[System.Serializable]
public class Buffer : MonoBehaviour
{
    public static Buffer instance;
    public bool assetDatabaseRefreshed = false;
    public int currentSimulationID;

    public User authenticatedUser = new User();
    public Agent[] agents = new Agent[0];
    public Simulation[] popularSimulations = new Simulation[0];
    public Simulation[] userSimulations = new Simulation[0];
    public Simulation currentSimulation = new Simulation();
    public PayoffFormula newFormula = new PayoffFormula();
    public Dictionary<(int, int), PayoffFormula> payoffFormulas = new Dictionary<(int, int), PayoffFormula>();
    public PayoffFormula[] payoffFormula;
    public int formulaTesterCompilationIndex = 0;
    //public (int, int) doubleInt = (10, 4);

    void Awake()
    {
        if(Buffer.instance == null)
        {
            this.gameObject.name = "Buffer";
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else { Destroy(this.gameObject); }
    }

    
    void Update()
    {
        if (Buffer.instance == null)
        {
            Debug.Log("currentSimulationDOTIDBEFORE: " + currentSimulation.ID);
            this.gameObject.name = "Buffer";
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("buffer instance SETTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT!");
            Debug.Log("currentSimulationDOTIDAFTER: " + currentSimulation.ID);
            Debug.Log("currentSimulationID: " + currentSimulationID);
        }
    }
    /**/

    void onReinitialize()
    {
        /*
        authenticatedUser.ID = Buffer.instance.authenticatedUser.ID;
        authenticatedUser.username = Buffer.instance.authenticatedUser.username
        
        currentSimulation.ID = Buffer.instance.currentSimulation.ID;
        /**/
authenticatedUser.ID = 0;
authenticatedUser.username = "";
currentSimulation.ID = 0;
    }

    public void clearPayoffFormulas()
    {
        payoffFormulas = new Dictionary<(int, int), PayoffFormula>();
    }

    public void newPopularSimulationsArray(int n)
    {
        instance.popularSimulations = new Simulation[n];
        for (int i = 0; i < instance.popularSimulations.Length; i++) 
        { 
            instance.popularSimulations[i] = new Simulation(); 
        }
    }

    public void newUserSimulationsArray(int n)
    {
        instance.userSimulations = new Simulation[n];
        for (int i = 0; i < instance.userSimulations.Length; i++) { instance.userSimulations[i] = new Simulation(); }
    }

    public void newAgentsArray(int n)
    {
        instance.agents = new Agent[n];
        for (int i = 0; i < instance.agents.Length; i++) 
        { 
            instance.agents[i] = new Agent();
            instance.agents[i].color = new Color( (i < 4) ? 1 : 0, (i % 4 < 2) ? 1 : 0, (i % 2 == 0) ? 1 : 0 );
        }

        /*
        this.agents = new Agent[n];
        for (int i = 0; i < this.agents.Length; i++) { this.agents[i] = new Agent(); }
        */
    }

    public void printPayoffFormulas()
    {

        Debug.Log("Length: " + payoffFormulas.Count);

        foreach (KeyValuePair<(int, int), PayoffFormula> formula in payoffFormulas)
        {
            Debug.Log("Key: " + formula.Key + ", value: " + formula.Value);
            Debug.Log("ID: " + formula.Value.ID);
            Debug.Log("agent1: " + formula.Value.agent1);
            Debug.Log("agent2: " + formula.Value.agent2);
            Debug.Log("payoffFormula: " + formula.Value.payoffFormula);
            Debug.Log("authorID: " + formula.Value.authorID);
        }
    }

    public void printPopularSimulationsArray()
    {
        if (instance.popularSimulations != null)
        {
            for (int i = 0; i < instance.popularSimulations.Length; i++)
            {
                Debug.Log(i + ". ID: " + instance.popularSimulations[i].ID);
                Debug.Log(i + ". name: " + instance.popularSimulations[i].name);
                Debug.Log(i + ". image: " + instance.popularSimulations[i].image);
                Debug.Log(i + ". description: " + instance.popularSimulations[i].description);
                Debug.Log(i + ". likesCount: " + instance.popularSimulations[i].likesCount);
                Debug.Log(i + ". dislikesCount: " + instance.popularSimulations[i].dislikesCount);
                Debug.Log(i + ". authorID: " + instance.popularSimulations[i].authorID);
            }
        }
        else
        {
            Debug.Log("Buffer.instance.agents == null");
        }
    }

    public void printUserSimulationsArray()
    {
        if (instance.userSimulations != null)
        {
            for (int i = 0; i < instance.userSimulations.Length; i++)
            {
                Debug.Log(i + ". ID: " + instance.userSimulations[i].ID);
                Debug.Log(i + ". name: " + instance.userSimulations[i].name);
                Debug.Log(i + ". image: " + instance.userSimulations[i].image);
                Debug.Log(i + ". description: " + instance.userSimulations[i].description);
                Debug.Log(i + ". likesCount: " + instance.userSimulations[i].likesCount);
                Debug.Log(i + ". dislikesCount: " + instance.userSimulations[i].dislikesCount);
                Debug.Log(i + ". authorID: " + instance.userSimulations[i].authorID);
            }
        }
        else
        {
            Debug.Log("Buffer.instance.agents == null");
        }
    }

    public void printAgents()
    {
        if(instance.agents != null)
        {
            for (int i = 0; i < instance.agents.Length; i++)
            {
                Debug.Log(i + ". agentID: " + instance.agents[i].agentID);
                Debug.Log(i + ". icon: " + instance.agents[i].icon);
                Debug.Log(i + ". agentName: " + instance.agents[i].agentName);
                Debug.Log(i + ". agentDescription: " + instance.agents[i].agentDescription);
                Debug.Log(i + ". authorID: " + instance.agents[i].authorID);
            }
        }
        else
        {
            Debug.Log("Buffer.instance.agents == null"); 
        }
    }
}
[System.Serializable]
public class PayoffFormula
{
    public int ID;
    public int agent1;
    public int agent2;
    public string payoffFormula;
    public int authorID;

    public float result;
}
[System.Serializable]
public class User
{
    public int ID;
    public string username;
    public bool isAdmin;
}
[System.Serializable]
public class Simulation
{
    public int ID;
    public string name;
    public string image;
    public string description;
    public int likesCount;
    public int dislikesCount;
    public int authorID;
    public bool approved;

    private int amountOfCorrespondingFormulas;
    //private int amountOfCorrespondingAgents = sqrt(amountOfCorrespondingFormulas);

    //public PayoffMatrix PayoffFormulas[,] = new PayoffMatrix[amountOfCorrespondingAgents, amountOfCorrespondingAgents];
}
[System.Serializable]
public class Agent
{
    public int agentID;
    public string icon;
    public string agentName;
    public string agentDescription;
    public int authorID;

    public Color color;
}