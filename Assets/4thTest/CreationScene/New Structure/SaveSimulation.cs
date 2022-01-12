using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static System.Math;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using UnityEditor;

[System.Serializable]
public class SaveSimulation : MonoBehaviour
{
    public static SaveSimulation instance;
    public GameObject payoffMatrixPanel;
    public GameObject nameInputField;
    public GameObject imageInputField;
    public GameObject descriptionInputField;

    private int childCount;
    private int agentCount;
    private int formulaCount;
    /*----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private List<string> txtLines;

    public List<string> payoffVariables = new List<string>();
    public string newPayoffVariable = "";
    public bool scriptsRecompiled = false;

    
    public IEnumerator testFormulas()
    {
        //Remember the formulas
        Buffer.instance.payoffFormulas = new Dictionary<(int, int), PayoffFormula>();
        for (int i = 1; i <= agentCount; i++)
        {
            for (int j = 1; j <= agentCount; j++)
            {

                GameObject agent1Cell = payoffMatrixPanel.transform.Find("TableColumn_0").Find("TableCell_" + i + "_0").gameObject;
                GameObject agent2Cell = payoffMatrixPanel.transform.Find("TableColumn_" + j).Find("TableCell_0_" + j).gameObject;
                GameObject tableCell = payoffMatrixPanel.transform.Find("TableColumn_" + j).Find("TableCell_" + i + "_" + j).gameObject;

                string payoffFormula = tableCell.transform.GetChild(0).GetComponent<InputField>().text;
                Debug.Log("i: " + i);
                int agent1 = Int32.Parse(agent1Cell.transform.GetChild(1).GetComponent<Text>().text);
                Debug.Log("j: " + j);
                int agent2 = Int32.Parse(agent2Cell.transform.GetChild(1).GetComponent<Text>().text);

                Buffer.instance.newFormula = new PayoffFormula();
                Buffer.instance.newFormula.agent1 = agent1;
                Buffer.instance.newFormula.agent2 = agent2;
                Buffer.instance.newFormula.payoffFormula = payoffFormula;

                Buffer.instance.payoffFormulas.Add((Buffer.instance.newFormula.agent1, Buffer.instance.newFormula.agent2), Buffer.instance.newFormula);
            }
        }

        //Detect payoffVariables
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
                    if (!((j + 1 < formula.Length) && ((formula[j + 1] >= 'a' && formula[j + 1] <= 'z') || (formula[j + 1] >= 'A' && formula[j + 1] <= 'Z') || (formula[j + 1] >= '0' && formula[j + 1] <= '9'))))
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

        txtLines = File.ReadAllLines("Assets/4thTest/CreationScene/formulaTester.txt").ToList();


        int a = 0;
        Debug.Log("payoffVariables.Count: " + payoffVariables.Count);
        foreach (string entry in payoffVariables)
        {
            txtLines.Insert(a + 13, "float " + entry.Substring(1) + " = 1f;"); a = a + 1;
        }
        Debug.Log("Buffer.instance.payoffFormulas.Count: " + Buffer.instance.payoffFormulas.Count);
        foreach (KeyValuePair<(int, int), PayoffFormula> entry in Buffer.instance.payoffFormulas)
        {
            entry.Value.payoffFormula = entry.Value.payoffFormula.Replace("$", "");

            txtLines.Insert(a + 18, "formulas.Add(" + entry.Key + ", " + entry.Value.payoffFormula + ");"); a = a + 1;
        }

        Buffer.instance.formulaTesterCompilationIndex++;
        txtLines.Insert(a + 33, "compilationIndex = " + Buffer.instance.formulaTesterCompilationIndex + "; scriptsRecompiled = true;"); a = a + 1;

        //Write them to the file
        File.WriteAllLines("Assets/4thTest/CreationScene/formulaTester.cs", txtLines);

        Debug.Log("FormulaTester.instance: " + FormulaTester.instance);
        Debug.Log("SaveSimulation.instance: " + SaveSimulation.instance);
        Debug.Log("Notification.instance: " + Notification.instance);
        Debug.Log("Buffer.instance: " + Buffer.instance);

        AssetDatabase.Refresh();
        yield return new RecompileScripts();
        yield return new WaitForSeconds(2);
        StartCoroutine(Notification.instance.showNotification("Recompiled"));
        messageDisplayed = false;
        //instance = null;
        scriptsRecompiled = true;
        //UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
    }

    public bool messageDisplayed = false;
    public bool secondIteration = false;

    public void Start()
    {
        if (SaveSimulation.instance == null)
        {
            scriptsRecompiled = false;
            instance = this;
        }
    }

    public int X = 0;

    public void Update()
    {
        if (/*SaveSimulation.instance != null && */messageDisplayed == false)
        {
            Debug.Log(SaveSimulation.instance);
            StartCoroutine(Notification.instance.showNotification("PLEASSE SHOW YOURSELF"));
            
            if (Buffer.instance.formulaTesterCompilationIndex == FormulaTester.instance.compilationIndex)
            {
                StartCoroutine(saveSimulation((Buffer.instance.currentSimulation.ID == 0) ? true : false));
                StartCoroutine(Notification.instance.showNotification("The payoff formulas dont have errors"));
                X = 2;
                scriptsRecompiled = false;
                messageDisplayed = true;
                secondIteration = false;
                FormulaTester.instance.scriptsRecompiled = false;
            }
            else
            {
                StartCoroutine(Notification.instance.showNotification("The payoff formulas DO have errors"));
                X = 1;
                scriptsRecompiled = false;
                messageDisplayed = true;
                secondIteration = false;
            }
            /**/
            messageDisplayed = true;
        }
        if (SaveSimulation.instance == null)
        {
            Debug.Log("ScriptsRecompiled: " + scriptsRecompiled);
            Debug.Log("messageDisplayed: " + messageDisplayed);
            scriptsRecompiled = true;
            instance = this;
            messageDisplayed = false;


        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public void save()
    {

        agentCount = payoffMatrixPanel.transform.childCount - 2;
        childCount = payoffMatrixPanel.transform.childCount;
        formulaCount = agentCount * agentCount;

        Debug.Log("agentCount: " + agentCount);
        Debug.Log("childCount: " + childCount);
        Debug.Log("formulaCount: " + formulaCount);

        StartCoroutine(testFormulas());
        
        //StartCoroutine(saveSimulation( (Buffer.instance.currentSimulation.ID == 0) ? true : false) );
        /*
        Debug.Log("Buffer.instance.currentSimulation.ID: " + Buffer.instance.currentSimulation.ID);
        //StartCoroutine(saveItsPayoffFormulas( (Buffer.instance.currentSimulation.ID == 0) ? true : false) );
        /**/
    }

    IEnumerator saveSimulation(bool newSimulation)
    {
        Debug.Log("SAVING SIMULATION");
        Debug.Log(newSimulation);
        WWWForm form = new WWWForm();
        form.AddField("class", "SimulationsController\\simulations");
        form.AddField("function", newSimulation ? "create" : "update");
        //form.AddField("function", "create");
        form.AddField("ID", Buffer.instance.currentSimulation.ID);
        form.AddField("name", nameInputField.GetComponent<InputField>().text);
        form.AddField("image", Convert.ToBase64String( ImageConversion.EncodeToPNG( (Texture2D)imageInputField.GetComponent<RawImage>().texture )) );
        form.AddField("description", descriptionInputField.GetComponent<InputField>().text);
        form.AddField("authorID", Buffer.instance.authenticatedUser.ID);
        //form.AddField("authorID", 5);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {
            Debug.Log("0; Update succesful;" + www.text);
            Debug.Log(www.text.Substring(2, www.text.Length - 2));
            Debug.Log(www.text.Substring(2, www.text.Length - 3));
            Buffer.instance.currentSimulation.ID = Int32.Parse(www.text.Substring(2, www.text.Length-2));
            Debug.Log("Buffer.instance.currentSimulation.ID: " + Buffer.instance.currentSimulation.ID);
        }
        else
        {
            Debug.Log("Loading agents failed. Error #" + www.text);
        }

        
        form = new WWWForm();
        form.AddField("class", "PayoffFormulasController\\payoffFormulas");
        form.AddField("function", newSimulation ? "create" : "update");
        //form.AddField("function", "create");
        Debug.Log("agentCount: " + agentCount);
        form.AddField("agentCount", agentCount);

        for (int i = 1; i <= agentCount; i++)
        {
            for (int j = 1; j <= agentCount; j++)
            {

                GameObject agent1Cell = payoffMatrixPanel.transform.Find("TableColumn_0").Find("TableCell_" + i + "_0").gameObject;
                GameObject agent2Cell = payoffMatrixPanel.transform.Find("TableColumn_" + j).Find("TableCell_0_" + j).gameObject;
                GameObject tableCell = payoffMatrixPanel.transform.Find("TableColumn_" + j).Find("TableCell_" + i + "_" + j).gameObject;

                string payoffFormula = tableCell.transform.Find("Formula").GetComponent<InputField>().text;
                int agent1 = Int32.Parse(agent1Cell.transform.Find("AgentID").GetComponent<Text>().text);
                int agent2 = Int32.Parse(agent2Cell.transform.Find("AgentID").GetComponent<Text>().text);

                form.AddField(i + "_" + j + "_payoffFormula_agent1", agent1);
                form.AddField(i + "_" + j + "_payoffFormula_agent2", agent2);
                form.AddField(i + "_" + j + "_payoffFormula_payoffFormula", payoffFormula);
                Debug.Log("Buffer.instance.currentSimulation.ID: " + Buffer.instance.currentSimulation.ID);
                form.AddField(i + "_" + j + "_payoffFormula_simulationID", Buffer.instance.currentSimulation.ID);
                //form.AddField(i + "_" + j + "_payoffFormula_simulationID", 2);
            }
        }

        www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {
            Debug.Log("0; Update succesful;" + www.text);
        }
        else
        {
            Debug.Log("Loading agents failed. Error #" + www.text);
        }
        /**/
    }

    IEnumerator saveItsPayoffFormulas(bool newSimulation)
    {
        WWWForm form = new WWWForm();
        form.AddField("class", "PayoffFormulasController\\payoffFormulas");
        //form.AddField("function", newSimulation ? "create" : "update");
        form.AddField("function", "create");
        form.AddField("agentCount", agentCount);

        for (int i = 1; i <= agentCount; i++)
        {
            for (int j = 1; j <= agentCount; j++)
            {

                GameObject agent1Cell = payoffMatrixPanel.transform.Find("TableCell_" + i + "_0").gameObject;
                GameObject agent2Cell = payoffMatrixPanel.transform.Find("TableCell_0_" + j).gameObject;
                GameObject tableCell = payoffMatrixPanel.transform.Find("TableCell_" + i + "_" + j).gameObject;

                string payoffFormula = tableCell.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().text;
                int agent1 = Int32.Parse(agent1Cell.transform.GetChild(0).GetComponent<Text>().text);
                int agent2 = Int32.Parse(agent2Cell.transform.GetChild(0).GetComponent<Text>().text);

                form.AddField(i + "_" + j + "_payoffFormula_agent1", agent1);
                form.AddField(i + "_" + j + "_payoffFormula_agent2", agent2);
                form.AddField(i + "_" + j + "_payoffFormula_payoffFormula", payoffFormula);
                Debug.Log("Buffer.instance.currentSimulation.ID: " + Buffer.instance.currentSimulation.ID);
                form.AddField(i + "_" + j + "_payoffFormula_simulationID", Buffer.instance.currentSimulation.ID);
                //form.AddField(i + "_" + j + "_payoffFormula_simulationID", 2);
            }
        }

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {
            Debug.Log("0; Update succesful;" + www.text);
        }
        else
        {
            Debug.Log("Loading agents failed. Error #" + www.text);
        }
    }
}
