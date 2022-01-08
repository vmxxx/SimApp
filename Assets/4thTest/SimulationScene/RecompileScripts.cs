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
    public void recompile()
    {
        //StartCoroutine(recompileScripts());
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
    }

    public IEnumerator recompileScripts()
    {
        yield return new WaitForSeconds(2);
        //AssetDatabase.ImportAsset("Assets/4thTest/SimulationScene/Buffer.cs");
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
    }
}
