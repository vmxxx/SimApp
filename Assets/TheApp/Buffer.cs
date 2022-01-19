using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

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
    public int formulaTesterCompilationIndex = -1;

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
            this.gameObject.name = "Buffer";
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
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