using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;

//This file gets recompiled every time we want to test if the payoffFormulas contain errors
//When that happens the Buffer.instance.compilationIndex increases by 1 and FormulaTester.instance.compilationIndex is set to the same number (by writing it in this file)
//
//If the formulas contain errors, the scripts wont get recompiled and the compilationIndexes will differ (Buffer.instance.compilationIndex != FormulaTester.instance.compilationIndex)
//If the formulas are work then both of the indexes will be the same, which will allow you to save the simulation
public class FormulaTester : MonoBehaviour
{
	public static FormulaTester instance; public bool scriptsRecompiled;
	public int compilationIndex = 0;

	Dictionary<(int, int), float> formulas = new Dictionary<(int, int), float>();
	//This is where we initialize the formula variables

	//This is where we write the formulas
	//We dont actually use, we just want to see if the script will get recompiled
	public void tryToAddFormulas()
	{
	}
	
	//Every time we reload the scene we reset the compilationIndex to 0 (and set the script instance to this script
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
			//If the scripts got recompiled we have to reset the instance to this script (because it was forgotten)
			//We also write a line here which sets the FormulaTester.compilationIndex to same as Buffer.compilationIndex
			instance = this; 
		}
	}
}
