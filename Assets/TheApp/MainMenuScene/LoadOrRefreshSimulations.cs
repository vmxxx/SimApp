using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static System.Math;
using System.Text.RegularExpressions;

public class LoadOrRefreshSimulations : MonoBehaviour
{
    public static LoadOrRefreshSimulations instance;

    public GameObject popularSimulationsPanel;
    public GameObject userSimulationsPanel;
    public GameObject popularSimulationEntry;
    public GameObject userSimulationEntry;

    private int pN = 1;
    private int uN = 1;

    private void Update()
    {
        if (LoadOrRefreshSimulations.instance == null) instance = this;
    }

    private void Start()
    {
        instance = this;
        loadPopular();
        loadUser();
    }

    public void loadPopular(bool onDelete = false)
    {
        if (onDelete == true) pN = pN - 1;
        StartCoroutine(loadPopularSimulations(pN));
    }

    public void loadUser(bool onDelete = false)
    {
        if (onDelete == true) uN = uN - 1;
        StartCoroutine(loadUserSimulations(uN));
    }

    private IEnumerator loadPopularSimulations(int N)
    {
        WWWForm form = new WWWForm();
        form.AddField("class", "SimulationsController\\simulations");
        form.AddField("function", "read");
        form.AddField("scene", "mainMenu");
        form.AddField("list", "popular");
        form.AddField("onSearch", "false");
        form.AddField("N", N);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {
            int i = 0;
            int popularSimulationsCount = 0;
            int userrSimulationsCount = 0;
            removeSimulations(popularSimulationsPanel, Buffer.instance.popularSimulations);

                    MatchCollection entries = Regex.Matches(www.text, @"{{1}(.*?)}{1}");
                    Buffer.instance.newPopularSimulationsArray(entries.Count);
                    int j = 0;
                    foreach (Match entry in entries)
                    {
                        string seperate_entry = entry.ToString();
                        string ID = Regex.Match(seperate_entry, @"ID:(.*?),").Value;
                        string name = Regex.Match(seperate_entry, @"name:(.*?),").Value;
                        string image = Regex.Match(seperate_entry, @"image:(.*?),").Value;
                        string description = Regex.Match(seperate_entry, @"description:(.*?),").Value;
                        string likesCount = Regex.Match(seperate_entry, @"likesCount:(.*?),").Value;
                        string dislikesCount = Regex.Match(seperate_entry, @"dislikesCount:(.*?),").Value;
                        string approved = Regex.Match(seperate_entry, @"approved:(.*?),").Value;
                        string authorID = Regex.Match(seperate_entry, @"authorID:(.*?)}").Value;

                        if (ID != "")
                        {
                            Buffer.instance.popularSimulations[j].ID = Int32.Parse(ID.Substring(3, ID.Length - 4));
                            Buffer.instance.popularSimulations[j].name = name.Substring(6, name.Length - 8);
                            Buffer.instance.popularSimulations[j].image = image.Substring(7, image.Length - 9);
                            Buffer.instance.popularSimulations[j].description = description.Substring(13, description.Length - 15);
                            Buffer.instance.popularSimulations[j].likesCount = Int32.Parse(likesCount.Substring(11, likesCount.Length - 12));
                            Buffer.instance.popularSimulations[j].dislikesCount = Int32.Parse(dislikesCount.Substring(14, dislikesCount.Length - 15));
                            Buffer.instance.popularSimulations[j].approved = (approved.Substring(9, approved.Length - 10) == "1") ? true : false;
                            Buffer.instance.popularSimulations[j].authorID = Int32.Parse(authorID.Substring(9, authorID.Length - 10));
                        }
                        j++;
                    }
            displaySimulations(popularSimulationEntry, popularSimulationsPanel, Buffer.instance.popularSimulations);
            pN = pN + 1;
        }
        else
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }
    }

    private IEnumerator loadUserSimulations(int N)
    {
        WWWForm form = new WWWForm();
        form.AddField("class", "SimulationsController\\simulations");
        form.AddField("function", "read");
        form.AddField("scene", "mainMenu");
        form.AddField("list", "user");
        form.AddField("onSearch", "false");
        form.AddField("authorID", Buffer.instance.authenticatedUser.ID);
        form.AddField("N", N);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {
            int popularSimulationsCount = 0;
            int userrSimulationsCount = 0;
            removeSimulations(userSimulationsPanel, Buffer.instance.userSimulations);

                    int j = 0;
                    MatchCollection entries = Regex.Matches(www.text, @"{{1}(.*?)}{1}");
                    Buffer.instance.newUserSimulationsArray(entries.Count);
                    foreach (Match entry in entries)
                    {
                        string seperate_entry = entry.ToString();
                        string ID = Regex.Match(seperate_entry, @"ID:(.*?),").Value;
                        string name = Regex.Match(seperate_entry, @"name:(.*?),").Value;
                        string image = Regex.Match(seperate_entry, @"image:(.*?),").Value;
                        string description = Regex.Match(seperate_entry, @"description:(.*?),").Value;
                        string likesCount = Regex.Match(seperate_entry, @"likesCount:(.*?),").Value;
                        string dislikesCount = Regex.Match(seperate_entry, @"dislikesCount:(.*?),").Value;
                        string approved = Regex.Match(seperate_entry, @"approved:(.*?),").Value;
                        string authorID = Regex.Match(seperate_entry, @"authorID:(.*?)}").Value;
                        if (ID != "")
                        {
                            Buffer.instance.userSimulations[j].ID = Int32.Parse(ID.Substring(3, ID.Length - 4));
                            Buffer.instance.userSimulations[j].name = name.Substring(6, name.Length - 8);
                            Buffer.instance.userSimulations[j].image = image.Substring(7, image.Length - 9);
                            Buffer.instance.userSimulations[j].description = description.Substring(13, description.Length - 15);
                            Buffer.instance.userSimulations[j].likesCount = Int32.Parse(likesCount.Substring(11, likesCount.Length - 12));
                            Buffer.instance.userSimulations[j].dislikesCount = Int32.Parse(dislikesCount.Substring(14, dislikesCount.Length - 15));
                            Buffer.instance.userSimulations[j].approved = (approved.Substring(9, approved.Length - 10) == "1") ? true : false;
                            Buffer.instance.userSimulations[j].authorID = Int32.Parse(authorID.Substring(9, authorID.Length - 10));
                        }
                        j++;
                    }

            displaySimulations(userSimulationEntry, userSimulationsPanel, Buffer.instance.userSimulations);
            Buffer.instance.printPopularSimulationsArray();
            uN = uN + 1;
        }
        else
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }
    }

    private void removeSimulations(GameObject panel, Simulation[] simulations)
    {
        foreach (Transform child in panel.transform)
        {
            if(child.name != "ShowMore") Destroy(child.gameObject);
        }
    }

    private void displaySimulations(GameObject entry, GameObject panel, Simulation[] simulations)
    {
        for (int i = 0; i < simulations.Length; i++)
        {
            GameObject newEntry = Instantiate(entry);
            newEntry.SetActive(true);
            newEntry.name = "Entry_" + i;
            newEntry.transform.GetChild(0).GetComponent<Text>().text = simulations[i].ID.ToString();
            newEntry.transform.GetChild(1).GetComponent<Text>().text = simulations[i].name;
            newEntry.transform.GetChild(2).GetComponent<RawImage>().texture = applyBase64StringAsTexture(simulations[i].image);
            newEntry.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = "likes/dislikes = " + simulations[i].likesCount + "/" + simulations[i].dislikesCount;
            newEntry.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<Text>().text = simulations[i].description;


            newEntry.transform.Find("LikesCount").GetComponent<Text>().text = simulations[i].likesCount.ToString();
            newEntry.transform.Find("DislikesCount").GetComponent<Text>().text = simulations[i].dislikesCount.ToString();
            newEntry.transform.Find("Description").GetComponent<Text>().text = simulations[i].description;
            newEntry.transform.SetParent(panel.transform);
            int currIndex = newEntry.transform.GetSiblingIndex();
            newEntry.transform.SetSiblingIndex(currIndex - 1);
        }
    }

    private Texture2D applyBase64StringAsTexture(string textureString)
    {
        Texture2D newTexture = new Texture2D(1, 1);
        newTexture.LoadImage(Convert.FromBase64String(textureString));
        newTexture.Apply();
        return newTexture;
    }
}
