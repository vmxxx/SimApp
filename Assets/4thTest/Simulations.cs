using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static System.Math;
using System.Text.RegularExpressions;

public class Simulations : MonoBehaviour
{

    public GameObject payoffMatrixPanel;
    public GameObject popularSimulationsPanel;
    public GameObject userSimulationsPanel;
    public GameObject entry;

    public void Awake()
    {
        load();
    }
    
    public void save()
    {
        StartCoroutine(saveSimulation());
    }

    IEnumerator saveSimulation()
    {
        int childCount = payoffMatrixPanel.transform.childCount;
        int agentCount = (int)Round(Sqrt(childCount)) - 2;
        int formulaCount = agentCount * agentCount;

        WWWForm form = new WWWForm();
        form.AddField("class", "SimulationsController\\simulations");
        form.AddField("function", "create");
        //form.AddField("name", iconSetting.text);
        //form.AddField("image", nameSetting.text);
        //form.AddField("description", descriptionSetting.text);
        form.AddField("name", "name");
        form.AddField("image", "image");
        form.AddField("description", "description");
        //form.AddField("authorID", Buffer.instance.authenticatedUser.ID);
        form.AddField("authorID", 5);
        form.AddField("agentCount", agentCount);

        
        for (int i=1; i <= agentCount; i++)
        {
            for (int j=1; j <= agentCount; j++)
            {

                GameObject agent1Cell = payoffMatrixPanel.transform.Find("TableCell_" + i + "_0").gameObject;
                GameObject agent2Cell = payoffMatrixPanel.transform.Find("TableCell_0_" + j).gameObject;
                GameObject tableCell = payoffMatrixPanel.transform.Find("TableCell_" + i + "_" + j).gameObject;

                string payoffFormula = tableCell.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().text;
                int agent1 = Int32.Parse(agent1Cell.transform.GetChild(0).GetComponent<Text>().text);
                int agent2 = Int32.Parse(agent2Cell.transform.GetChild(0).GetComponent<Text>().text);

                form.AddField(i + "_" + j + "_payoffFormula_agent1", agent1);
                form.AddField(i + "_" + j + "_payoffFormula_agent2", agent2 );
                form.AddField(i + "_" + j + "_payoffFormula_payoffFormula", payoffFormula);
                //form.AddField(i + "_" + j + "_payoffFormula_authorID", simulationIDSetting.text);
                form.AddField(i + "_" + j + "_payoffFormula_authorID", 2);
            }
        }
        /**/

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

    public void load()
    {
        StartCoroutine(loadSimulations());
    }

    IEnumerator loadSimulations()
    {
        WWWForm form = new WWWForm();
        form.AddField("class", "SimulationsController\\simulations");
        form.AddField("function", "read");
        //form.AddField("authorID", Buffer.instance.authenticatedUser.ID);
        form.AddField("authorID", 5);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {
            int i = 0;
            int popularSimulationsCount = 0;
            int userrSimulationsCount = 0;
            foreach (Match match in Regex.Matches(www.text, @"0;(.*?){(.*?)}0;"))
            {
                if (i == 0)
                {
                    MatchCollection entries = Regex.Matches(match.Value, @"{{1}(.*?)}{1}");
                    Debug.Log(entries.Count);
                    Buffer.instance.newUserSimulationsArray(entries.Count);
                    Debug.Log(match);
                    int j = 0;
                    foreach (Match entry in entries)
                    {
                        string seperate_entry = entry.ToString();
                        Debug.Log("seperate_entry: " + seperate_entry);
                        string ID = Regex.Match(seperate_entry, @"ID:(.*?),").Value;
                        string name = Regex.Match(seperate_entry, @"name:(.*?),").Value;
                        string image = Regex.Match(seperate_entry, @"image:(.*?),").Value;
                        string description = Regex.Match(seperate_entry, @"description:(.*?),").Value;
                        string likesCount = Regex.Match(seperate_entry, @"likesCount:(.*?),").Value;
                        string dislikesCount = Regex.Match(seperate_entry, @"dislikesCount:(.*?),").Value;
                        string authorID = Regex.Match(seperate_entry, @"authorID:(.*?)}").Value;
                        Debug.Log("authorID" + authorID);


                        if (ID != "")
                        {
                            Buffer.instance.userSimulations[j].ID = Int32.Parse(ID.Substring(3, ID.Length - 4));
                            Buffer.instance.userSimulations[j].name = name.Substring(6, name.Length - 8);
                            Buffer.instance.userSimulations[j].image = image.Substring(7, image.Length - 9);
                            Buffer.instance.userSimulations[j].description = description.Substring(13, description.Length - 15);
                            Buffer.instance.userSimulations[j].likesCount = Int32.Parse(likesCount.Substring(11, likesCount.Length - 12));
                            Buffer.instance.userSimulations[j].dislikesCount = Int32.Parse(dislikesCount.Substring(14, dislikesCount.Length - 15));
                            Buffer.instance.userSimulations[j].authorID = Int32.Parse(authorID.Substring(9, authorID.Length - 10));
                        }
                        /**/
                    }
                    j++;
                }
                else if (i == 1)
                {
                    /*
                    int j = 0;
                    MatchCollection entries = Regex.Matches(match.Value, @"{{1}(.*?)}{1}");
                    Buffer.instance.newPopularSimulationsArray(entries.Count);
                    foreach (Match entry in entries)
                    {
                        string seperate_entry = entry.ToString();
                        Debug.Log("seperate_entry: " + seperate_entry);
                        string ID = Regex.Match(seperate_entry, @"ID:(.*?),").Value;
                        string name = Regex.Match(seperate_entry, @"name:(.*?),").Value;
                        string image = Regex.Match(seperate_entry, @"image:(.*?),").Value;
                        string description = Regex.Match(seperate_entry, @"description:(.*?),").Value;
                        string likesCount = Regex.Match(seperate_entry, @"likesCount:(.*?),").Value;
                        string dislikesCount = Regex.Match(seperate_entry, @"dislikesCount:(.*?),").Value;
                        string authorID = Regex.Match(seperate_entry, @"authorID:(.*?)}").Value;
                        if (ID != "")
                        {
                            Buffer.instance.popularSimulations[j].ID = Int32.Parse(ID.Substring(3, ID.Length - 4));
                            Buffer.instance.popularSimulations[j].name = name.Substring(6, name.Length - 8);
                            Buffer.instance.popularSimulations[j].image = image.Substring(7, image.Length - 9);
                            Buffer.instance.popularSimulations[j].description = description.Substring(13, description.Length - 15);
                            Buffer.instance.popularSimulations[j].likesCount = Int32.Parse(likesCount.Substring(11, likesCount.Length - 12));
                            Buffer.instance.popularSimulations[j].dislikesCount = Int32.Parse(dislikesCount.Substring(14, dislikesCount.Length - 15));
                            Buffer.instance.popularSimulations[j].authorID = Int32.Parse(authorID.Substring(9, authorID.Length - 10));
                        }
                    }
                    j++;
                    /***/
                }
                i++;
            }
            displaySimulations();
            Buffer.instance.printPopularSimulationsArray();
            Debug.Log("0; Load succesful;" + www.text);
        }
        else
        {
            Debug.Log("Loading simulations failed. Error #" + www.text);
        }
    }
    
    public void displaySimulations()
    {
        Debug.Log(Buffer.instance.userSimulations.Length);
        for(int i = 0; i < Buffer.instance.userSimulations.Length; i++)
        {
            GameObject newEntry = Instantiate(entry);
            newEntry.SetActive(true);
            newEntry.name = "Entry_" + i;
            newEntry.transform.GetChild(0).GetComponent<Text>().text = Buffer.instance.userSimulations[i].name;
            newEntry.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = "likes/dislikes = " + Buffer.instance.userSimulations[i].likesCount + "/" + Buffer.instance.userSimulations[i].dislikesCount;
            newEntry.transform.GetChild(4).GetChild(1).GetComponent<Text>().text = Buffer.instance.userSimulations[i].description;
            newEntry.transform.SetParent(userSimulationsPanel.transform);
        }
        /*
        for(int i = 0; i < Buffer.instance.popularSimulations.Length; i++)
        {
            GameObject newEntry = Instantiate(entry);
            newEntry.SetActive(true);
            newEntry.name = "Entry_" + i;
            newEntry.transform.GetChild(0).GetComponent<Text>().text = Buffer.instance.popularSimulations[i].name;
            newEntry.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = "likes/dislikes = " + Buffer.instance.popularSimulations[i].likesCount + "/" + Buffer.instance.popularSimulations[i].dislikesCount;
            newEntry.transform.GetChild(4).GetChild(1).GetComponent<Text>().text = Buffer.instance.popularSimulations[i].description;
            newEntry.transform.SetParent(popularSimulationsPanel.transform);
        }
        /**/
    }
}
