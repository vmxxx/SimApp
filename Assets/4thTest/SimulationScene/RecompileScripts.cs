using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static System.Math;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;

using System.IO;
using System.Linq;


public class RecompileScripts : MonoBehaviour
{
    private List<string> txtLines;
    public static RecompileScripts instance;
public bool scriptsRecompiled = false;

    public void recompile()
    {
        //StartCoroutine(recompileScripts());
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
    }

    public void Update()
    {
        /*
        if (Buffer.instance.formulaTesterCompilationIndex != FormulaTester.instance.compilationIndex)
        {
            Debug.Log("THE PAYOFF FORMULAS HAVE ERRORS !@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$#%!@$#%!@$#%$!@#%$!@#%$@!#%$");
            Debug.Log("Buffer.instance.formulaTesterCompilationIndex: " + Buffer.instance.formulaTesterCompilationIndex);
            Debug.Log("FormulaTester.instance.compilationIndex: " + FormulaTester.instance.compilationIndex);
        }
        else
        {
            Debug.Log("!@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$#%!@$#%!@$#%$!@#%$!@#%$@!#%$!@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$");
        }
        /**/
        if(RecompileScripts.instance == null)
        {
            instance = this;
scriptsRecompiled = true;
        }


        if(Notification.instance != null && FormulaTester.instance != null && Buffer.instance != null && scriptsRecompiled == true)
        {
            if (Buffer.instance.formulaTesterCompilationIndex != FormulaTester.instance.compilationIndex)
            {
                Debug.Log("THE PAYOFF FORMULAS HAVE ERRORS !@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$#%!@$#%!@$#%$!@#%$!@#%$@!#%$");
                Debug.Log("Buffer.instance.formulaTesterCompilationIndex: " + Buffer.instance.formulaTesterCompilationIndex);
                Debug.Log("FormulaTester.instance.compilationIndex: " + FormulaTester.instance.compilationIndex);
                StartCoroutine(Notification.instance.showNotification("The payoff formulas DO have errors"));
            }
            else
            {
                Debug.Log("!@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$#%!@$#%!@$#%$!@#%$!@#%$@!#%$!@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$");
                Debug.Log("Buffer.instance.formulaTesterCompilationIndex: " + Buffer.instance.formulaTesterCompilationIndex);
                Debug.Log("FormulaTester.instance.compilationIndex: " + FormulaTester.instance.compilationIndex);
                StartCoroutine(Notification.instance.showNotification("The payoff formulas dont have errors"));
            }
            scriptsRecompiled = false;
        }
    }

    public void recompileAndTestFormulas()
    {
        /*
        Buffer.instance.formulaTesterCompilationIndex++;
        txtLines = File.ReadAllLines("Assets/4thTest/CreationScene/formulaTester_test.txt").ToList();
        txtLines.Insert(33, "compilationIndex = " + Buffer.instance.formulaTesterCompilationIndex + ";");
        File.WriteAllLines("Assets/4thTest/CreationScene/FormulaTester.cs", txtLines);
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();

        /**/

        
        Buffer.instance.formulaTesterCompilationIndex++;
        txtLines = File.ReadAllLines("Assets/4thTest/CreationScene/formulaTester.txt").ToList();
        txtLines.Insert(24, "compilationIndex = " + Buffer.instance.formulaTesterCompilationIndex + ";");
        txtLines.Insert(47, "compilationIndex = " + Buffer.instance.formulaTesterCompilationIndex + ";");
        File.WriteAllLines("Assets/4thTest/CreationScene/FormulaTester.cs", txtLines);

        txtLines = File.ReadAllLines("Assets/4thTest/SimulationScene/RecompileScripts.cs").ToList();
        txtLines[43] = "scriptsRecompiled = true;";
        File.WriteAllLines("Assets/4thTest/SimulationScene/RecompileScripts.cs", txtLines);
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();

        /*
        Debug.Log("instanteneous: ");
        if (Buffer.instance.formulaTesterCompilationIndex != FormulaTester.instance.compilationIndex)
        {
            Debug.Log("THE PAYOFF FORMULAS HAVE ERRORS !@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$#%!@$#%!@$#%$!@#%$!@#%$@!#%$");
            Debug.Log("Buffer.instance.formulaTesterCompilationIndex: " + Buffer.instance.formulaTesterCompilationIndex);
            Debug.Log("FormulaTester.instance.compilationIndex: " + FormulaTester.instance.compilationIndex);
        }
        else
        {
            Debug.Log("!@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$#%!@$#%!@$#%$!@#%$!@#%$@!#%$!@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$");
        }
        /**/
        //StartCoroutine( RecompileAndTestFormulas() );
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        /*
        Debug.Log("OnScriptsReloaded(): ");
        if (Buffer.instance.formulaTesterCompilationIndex != FormulaTester.instance.compilationIndex)
        {
            Debug.Log("THE PAYOFF FORMULAS HAVE ERRORS !@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$#%!@$#%!@$#%$!@#%$!@#%$@!#%$");
            Debug.Log("Buffer.instance.formulaTesterCompilationIndex: " + Buffer.instance.formulaTesterCompilationIndex);
            Debug.Log("FormulaTester.instance.compilationIndex: " + FormulaTester.instance.compilationIndex);
        }
        else
        {
            Debug.Log("!@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$#%!@$#%!@$#%$!@#%$!@#%$@!#%$!@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$");
            Debug.Log("Buffer.instance.formulaTesterCompilationIndex: " + Buffer.instance.formulaTesterCompilationIndex);
            Debug.Log("FormulaTester.instance.compilationIndex: " + FormulaTester.instance.compilationIndex);
        }
        /**/
    }

    public IEnumerator RecompileAndTestFormulas()
    {

        Buffer.instance.formulaTesterCompilationIndex++;
        txtLines = File.ReadAllLines("Assets/4thTest/CreationScene/formulaTester.txt").ToList();
        txtLines.Insert(24, "compilationIndex = " + Buffer.instance.formulaTesterCompilationIndex + ";");
        txtLines.Insert(48, "compilationIndex = " + Buffer.instance.formulaTesterCompilationIndex + ";");
        File.WriteAllLines("Assets/4thTest/CreationScene/FormulaTester.cs", txtLines);
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();

        Debug.Log("instanteneous: ");
        if (Buffer.instance.formulaTesterCompilationIndex != FormulaTester.instance.compilationIndex)
        {
            Debug.Log("THE PAYOFF FORMULAS HAVE ERRORS !@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$#%!@$#%!@$#%$!@#%$!@#%$@!#%$");
            Debug.Log("Buffer.instance.formulaTesterCompilationIndex: " + Buffer.instance.formulaTesterCompilationIndex);
            Debug.Log("FormulaTester.instance.compilationIndex: " + FormulaTester.instance.compilationIndex);
        }
        else
        {
            Debug.Log("!@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$#%!@$#%!@$#%$!@#%$!@#%$@!#%$!@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$");
            Debug.Log("Buffer.instance.formulaTesterCompilationIndex: " + Buffer.instance.formulaTesterCompilationIndex);
            Debug.Log("FormulaTester.instance.compilationIndex: " + FormulaTester.instance.compilationIndex);
        }

        AssetDatabase.Refresh();
        yield return new RecompileScripts();
        Debug.Log("on yield return new RecompileScripts(): ");
        if (Buffer.instance.formulaTesterCompilationIndex != FormulaTester.instance.compilationIndex)
        {
            Debug.Log("THE PAYOFF FORMULAS HAVE ERRORS !@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$#%!@$#%!@$#%$!@#%$!@#%$@!#%$");
            Debug.Log("Buffer.instance.formulaTesterCompilationIndex: " + Buffer.instance.formulaTesterCompilationIndex);
            Debug.Log("FormulaTester.instance.compilationIndex: " + FormulaTester.instance.compilationIndex);
        }
        else
        {
            Debug.Log("!@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$#%!@$#%!@$#%$!@#%$!@#%$@!#%$!@#!@$!@#!@#$#!@$#!@$!#@$!#@$!@$");
            Debug.Log("Buffer.instance.formulaTesterCompilationIndex: " + Buffer.instance.formulaTesterCompilationIndex);
            Debug.Log("FormulaTester.instance.compilationIndex: " + FormulaTester.instance.compilationIndex);
        }
    }

    public IEnumerator recompileScripts()
    {
        yield return new WaitForSeconds(2);
        //AssetDatabase.ImportAsset("Assets/4thTest/SimulationScene/Buffer.cs");
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
    }
}
