using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static System.Math;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine.SceneManagement;

using System.IO;
using System.Linq;

public class ClearGeneratableScriptFiles : MonoBehaviour
{
    public static ClearGeneratableScriptFiles instance;
    public GameObject loadingFilter;
    public bool scriptsRecompiled;
    public int compilationIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (ClearGeneratableScriptFiles.instance == null)
        {
            compilationIndex = 0; instance = this;
        }
        StartCoroutine(clearGeneratableScriptFiles());
    }

    public void Update()
    {
        
        if (scriptsRecompiled == true || Buffer.instance.formulaTesterCompilationIndex == 0)
        {
            loadingFilter.SetActive(false);
            scriptsRecompiled = false;
        }
        if (ClearGeneratableScriptFiles.instance == null)
        { 
compilationIndex = 4; scriptsRecompiled = true;
            instance = this; 
        }
    }

    IEnumerator clearGeneratableScriptFiles()
    {
        List<string> txtLines = File.ReadAllLines("Assets/TheApp/SimulationScene/RunSimulation.txt").ToList();
        File.WriteAllLines("Assets/TheApp/SimulationScene/RunSimulation.cs", txtLines);
        txtLines = File.ReadAllLines("Assets/TheApp/CreationScene/formulaTester.txt").ToList();
        File.WriteAllLines("Assets/TheApp/CreationScene/FormulaTester.cs", txtLines);


        Buffer.instance.formulaTesterCompilationIndex++;
        txtLines = File.ReadAllLines("Assets/TheApp/ClearGeneratableScriptFiles.cs").ToList();
        txtLines[41] = "compilationIndex = " + Buffer.instance.formulaTesterCompilationIndex + "; scriptsRecompiled = true;";
        File.WriteAllLines("Assets/TheApp/ClearGeneratableScriptFiles.cs", txtLines);
        AssetDatabase.Refresh();
        yield return new RecompileScripts();
    }
}
