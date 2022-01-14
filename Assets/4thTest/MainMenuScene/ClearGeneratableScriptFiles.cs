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
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(clearGeneratableScriptFiles());
    }


    IEnumerator clearGeneratableScriptFiles()
    {
        List<string> txtLines = File.ReadAllLines("Assets/4thTest/SimulationScene/RunSimulation.txt").ToList();
        File.WriteAllLines("Assets/4thTest/SimulationScene/RunSimulation.cs", txtLines);
        txtLines = File.ReadAllLines("Assets/4thTest/CreationScene/formulaTester.txt").ToList();
        File.WriteAllLines("Assets/4thTest/CreationScene/FormulaTester.cs", txtLines);
        AssetDatabase.Refresh();
        yield return new RecompileScripts();
    }
}
