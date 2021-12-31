using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.IO;

public class ScriptC : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Debug from Sript C");
       

        string path = "Assets/ScriptX.cs";
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("using System.Collections; using System.Collections.Generic; using UnityEngine; using UnityEditor; using System.IO; public class ScriptX : MonoBehaviour { void Start() { Debug.Log(\"Debug from Sript X 1111111111111111111111\"); } void Update() { Debug.Log(\"Debug from Sript X\"); } } "); 
        Debug.Log("Line written");
        writer.Close();
        AssetDatabase.Refresh();
        //this.gameObject.AddComponent<ScriptX>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void SomeMethodFromC()
    {
        if (1 == 2)
        {
        //    this.gameObject.AddComponent<ScriptX>();
        }
    }
}
