using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using System.IO;
using System.Linq;
//using System.Line;

public class ScriptO : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Debug from Sript O");


        string path = "Assets/2ndTest/ScriptX.cs";
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("using System.Collections; using System.Collections.Generic; using UnityEngine; using UnityEditor; using System.IO; public class ScriptX : MonoBehaviour { void Start() { Debug.Log(\"Debug from Sript X 1111111111111111111111\"); } void Update() { Debug.Log(\"Debug from Sript X\"); } } ");
        Debug.Log("Line written");
        writer.Close();
        AssetDatabase.Refresh();
        this.PleaseWork();


        //this.gameObject.AddComponent<ScriptX>();
    }

    // Update is called once per frame
    void Update() 
    {
        //Put here
    }

    public void PleaseWork() //npcName = "item1"
    {
        string filename = "Assets/2ndTest/ScriptO.cs";
        var endTag = "        //Put here";
        var lineToAdd = "this.gameObject.AddComponent<ScriptX>();";

        var txtLines = File.ReadAllLines(filename).ToList();   //Fill a list with the lines from the txt file.
        Debug.Log("Fill a list with the lines from the txt file.");

        txtLines.Insert(txtLines.IndexOf(endTag), lineToAdd);  //Insert the line you want to add last under the tag 'item1'.
        Debug.Log("Insert the line you want to add last under the tag 'item1'.");

        File.WriteAllLines(filename, txtLines);                //Add the lines including the new one.
        Debug.Log("Add the lines including the new one.");

        AssetDatabase.Refresh();
    }
}
