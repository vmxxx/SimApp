using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;

public class FormulaTester : MonoBehaviour
{
	public static FormulaTester instance; public bool scriptsRecompiled;
	public int compilationIndex = 0;
float C = 1f;
float V = 1f;

	Dictionary<(int, int), float> formulas = new Dictionary<(int, int), float>();

	public void tryToAddFormulas()
	{
formulas.Add((10, 10), C - V);
	}
	
	public void Start()
	{
		if (FormulaTester.instance == null)
		{
			compilationIndex = 0; scriptsRecompiled = false;
			instance = this;
		}
	}
	
	public void Update()
	{
		if(FormulaTester.instance == null)
		{
compilationIndex = 1; scriptsRecompiled = true;
			instance = this;
		}
	}
	
	public bool recompiled = false;
	public bool tested = false;
	public bool initialVersion = false;
}
