using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;
using UnityEngine.UI;
using System;

public class SaveSimulation : MonoBehaviour
{
    public GameObject payoffMatrixPanel;
    public GameObject nameInputField;
    public GameObject imageInputField;
    public GameObject descriptionInputField;

    private int childCount;
    private int agentCount;
    private int formulaCount;

    public void save()
    {

        agentCount = payoffMatrixPanel.transform.childCount - 2;
        childCount = payoffMatrixPanel.transform.childCount * payoffMatrixPanel.transform.childCount;
        formulaCount = agentCount * agentCount;

        StartCoroutine(saveSimulation( (Buffer.instance.currentSimulation.ID == 0) ? true : false) );
        Debug.Log("Buffer.instance.currentSimulation.ID: " + Buffer.instance.currentSimulation.ID);
        //StartCoroutine(saveItsPayoffFormulas( (Buffer.instance.currentSimulation.ID == 0) ? true : false) );
    }

    IEnumerator saveSimulation(bool newSimulation)
    {
        Debug.Log(newSimulation);
        WWWForm form = new WWWForm();
        form.AddField("class", "SimulationsController\\simulations");
        form.AddField("function", newSimulation ? "create" : "update");
        //form.AddField("function", "create");
        form.AddField("ID", Buffer.instance.currentSimulation.ID);
        form.AddField("name", nameInputField.GetComponent<InputField>().text);
        form.AddField("image", imageInputField.GetComponent<InputField>().text);
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
