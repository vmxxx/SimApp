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
    /*This is the section where the formulas are tested (the save() function also gets called from in here, since we may only do that if there are no errors in the formulas)-----------------------------------------------*/

    private List<string> txtLines;

    public List<string> payoffVariables = new List<string>();
    public string newPayoffVariable = "";
    public bool scriptsRecompiled = false;

    public bool messageDisplayed = true;
    public bool secondIteration = false;
    public GameObject loadingFilter;

    public IEnumerator testFormulas()
    {
        if (readFormulasFromTheMatrix() == "containsErrors") { StartCoroutine(Notification.instance.showNotification("There cannot be duplicate agents!")); yield break; }

        detectPayoffVariables(); //For example "$V - $C" contains 2 variables: "$V" and "$C"



            //Generate a formula tester script
            //If it gets recompiled then there are no errors in the formulas
            //If it doesnt then we cant save the simulation

            //Copy the script template into memory
            txtLines = File.ReadAllLines("Assets/TheApp/CreationScene/formulaTester.txt").ToList();

            int a = 0;
            //Insert "float payoffVariable = 1f;" into the template copy (for each payoff variable)
            foreach (string entry in payoffVariables)
            {
                txtLines.Insert(a + 21, "float " + entry.Substring(1) + " = 1f;"); a = a + 1;
            }
            //Insert "formulas.Add((9, 10), (some + formmula / payoffVariable));" into the template copy (for each formula)
            foreach (KeyValuePair<(int, int), PayoffFormula> entry in Buffer.instance.payoffFormulas)
            {
                entry.Value.payoffFormula = entry.Value.payoffFormula.Replace("$", "");

                txtLines.Insert(a + 26, "formulas.Add(" + entry.Key + ", " + entry.Value.payoffFormula + ");"); a = a + 1;
            }

            Buffer.instance.formulaTesterCompilationIndex++;
            //txtLines.Insert(a + 44, "compilationIndex = " + Buffer.instance.formulaTesterCompilationIndex + "; scriptsRecompiled = true;"); a = a + 1;

            //Write them to the file
            File.WriteAllLines("Assets/TheApp/CreationScene/formulaTester.cs", txtLines);

            /*------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
            txtLines = File.ReadAllLines("Assets/TheApp/ClearGeneratableScriptFiles.cs").ToList();

            txtLines[41] = "compilationIndex = " + Buffer.instance.formulaTesterCompilationIndex + "; scriptsRecompiled = true;";

            File.WriteAllLines("Assets/TheApp/ClearGeneratableScriptFiles.cs", txtLines);
        /*------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

        //After we've generated a formula tester script:



        //We recompile the scripts to see if it contains errors
        AssetDatabase.Refresh();
        yield return new RecompileScripts();
        yield return new WaitForSeconds(2);
        messageDisplayed = false;
        //scriptsRecompiled = true;
    }

    public void Start()
    {
        //When the scene gets reloaded we have (re)set the script instance to this script
        if (SaveSimulation.instance == null)
        {
            scriptsRecompiled = false;
            instance = this;
        }
    }

    public void Update()
    {
        //If we haven't displayed a success/error message SINCE the last time we pressed "SAVE"
        if (ClearGeneratableScriptFiles.instance != null && messageDisplayed == false && messageDisplayed == false )
        {
            if(true)
            {
                //If the formulas DONT contain errors
                if (Buffer.instance.formulaTesterCompilationIndex == ClearGeneratableScriptFiles.instance.compilationIndex)
                {
                    if (nameInputField.GetComponent<InputField>().text != "" && imageInputField.GetComponent<RawImage>().texture != null && descriptionInputField.GetComponent<InputField>().text != "")
                    { StartCoroutine(saveSimulation((Buffer.instance.currentSimulation.ID == 0) ? true : false)); }
                    else StartCoroutine(Notification.instance.showNotification("The simulation name, image and description cannot be NULL."));

                    scriptsRecompiled = false;
                    FormulaTester.instance.scriptsRecompiled = false;
                }
                else //If the formulas DO contain errors
                {
                    StartCoroutine(Notification.instance.showNotification("The payoff formulas have errors"));
                }
            }
            scriptsRecompiled = false;
            messageDisplayed = true; //Now we take note that we shouldn't display it again
            loadingFilter.SetActive(false);
        }
        /**/
        //In case the scripts gets recompiled (because then the instance variable is forgotten)
        //we reset the script instance to this script
        if (SaveSimulation.instance == null)
        {
            instance = this;
            messageDisplayed = false;
            scriptsRecompiled = true;
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public void save()
    {
        //agentCount = matrix column count - last_column - 0_column
        agentCount = payoffMatrixPanel.transform.childCount - 2;
        //total column count of the matrix
        childCount = payoffMatrixPanel.transform.childCount;
        formulaCount = agentCount * agentCount;

        loadingFilter.SetActive(true);

        for (int i = 1; i <= agentCount; i++)
        {
            GameObject agent1Cell = payoffMatrixPanel.transform.Find("TableColumn_0").Find("TableCell_" + i + "_0").gameObject;
            string agent1 = agent1Cell.transform.Find("AgentID").GetComponent<Text>().text;
            if (agent1 == "X") { StartCoroutine(Notification.instance.showNotification("Agent cells are empty.")); return; }
        }

        //First we test the formulas, if there are no errors then the saveSimulation() will be called (line 110)
        if (imageInputField.GetComponent<RawImage>().texture != null) { StartCoroutine(testFormulas()); }
        else StartCoroutine(Notification.instance.showNotification("Simulation image cannot be NULL!"));
    }

    IEnumerator saveSimulation(bool newSimulation)
    {
        //Create an HTML form with the data
        WWWForm form = new WWWForm();
        form.AddField("class", "SimulationsController\\simulations");
        form.AddField("function", newSimulation ? "create" : "update");
        form.AddField("ID", Buffer.instance.currentSimulation.ID);
        form.AddField("name", nameInputField.GetComponent<InputField>().text);
        form.AddField("image", Convert.ToBase64String( ImageConversion.EncodeToPNG( (Texture2D)imageInputField.GetComponent<RawImage>().texture )) );
        form.AddField("description", descriptionInputField.GetComponent<InputField>().text);
        form.AddField("authorID", Buffer.instance.authenticatedUser.ID);
        form.AddField("onApproval", "false");

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        if (www.text != "" && www.text[0] == '0')
        {
            //Incase we created a new simulation (NOT updated it) we have to set the current simulation ID to the new entry's ID,
            //since next time we press "SAVE" we are going to update it
            Buffer.instance.currentSimulation.ID = Int32.Parse(www.text.Substring(2, www.text.Length-2));
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }


        //Create an HTML form with the data
        form = new WWWForm();
        form.AddField("class", "PayoffFormulasController\\payoffFormulas");
        form.AddField("function", newSimulation ? "create" : "update");
        form.AddField("agentCount", agentCount);
        for (int i = 1; i <= agentCount; i++)
        {
            for (int j = 1; j <= agentCount; j++)
            {

                GameObject agent1Cell = payoffMatrixPanel.transform.Find("TableColumn_0").Find("TableCell_" + i + "_0").gameObject;
                GameObject agent2Cell = payoffMatrixPanel.transform.Find("TableColumn_" + j).Find("TableCell_0_" + j).gameObject;
                GameObject tableCell = payoffMatrixPanel.transform.Find("TableColumn_" + j).Find("TableCell_" + i + "_" + j).gameObject;

                string payoffFormula = tableCell.transform.GetChild(0).GetComponent<InputField>().text;
                int agent1 = Int32.Parse(agent1Cell.transform.Find("AgentID").GetComponent<Text>().text);
                int agent2 = Int32.Parse(agent2Cell.transform.Find("AgentID").GetComponent<Text>().text);

                form.AddField(i + "_" + j + "_payoffFormula_agent1", agent1);
                form.AddField(i + "_" + j + "_payoffFormula_agent2", agent2);
                form.AddField(i + "_" + j + "_payoffFormula_payoffFormula", payoffFormula);
                form.AddField(i + "_" + j + "_payoffFormula_simulationID", Buffer.instance.currentSimulation.ID);
            }
        }

        www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        if (www.text != "" && www.text[0] == '0')
        {
            //Display the notification
            StartCoroutine(Notification.instance.showNotification("Simulation saved successfully!"));
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }
    }

    public string readFormulasFromTheMatrix()
    {
        Buffer.instance.payoffFormulas = new Dictionary<(int, int), PayoffFormula>();
        for (int i = 1; i <= agentCount; i++)
        {
            for (int j = 1; j <= agentCount; j++)
            {

                GameObject agent1Cell = payoffMatrixPanel.transform.Find("TableColumn_0").Find("TableCell_" + i + "_0").gameObject;
                GameObject agent2Cell = payoffMatrixPanel.transform.Find("TableColumn_" + j).Find("TableCell_0_" + j).gameObject;
                GameObject tableCell = payoffMatrixPanel.transform.Find("TableColumn_" + j).Find("TableCell_" + i + "_" + j).gameObject;

                string payoffFormula = tableCell.transform.GetChild(0).GetComponent<InputField>().text;
                int agent1 = Int32.Parse(agent1Cell.transform.GetChild(1).GetComponent<Text>().text);
                int agent2 = Int32.Parse(agent2Cell.transform.GetChild(1).GetComponent<Text>().text);

                Buffer.instance.newFormula = new PayoffFormula();
                Buffer.instance.newFormula.agent1 = agent1;
                Buffer.instance.newFormula.agent2 = agent2;
                Buffer.instance.newFormula.payoffFormula = payoffFormula;

                if (!Buffer.instance.payoffFormulas.ContainsKey((agent1, agent2))) Buffer.instance.payoffFormulas.Add((agent1, agent2), Buffer.instance.newFormula);
                else return "containsErrors";
            }
        }
        return "ok";
    }

    public void detectPayoffVariables()
    {
        bool variableFound = false;
        foreach (KeyValuePair<(int, int), PayoffFormula> entry in Buffer.instance.payoffFormulas)
        {
            string formula = entry.Value.payoffFormula;
            for (int j = 0; j < formula.Length; j++)
            {
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
    }

}
