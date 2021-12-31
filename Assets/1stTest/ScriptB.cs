using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptB : ScriptA
{

    //FindObjectsOfType is not allowed to be called from a MonoBehaviour constructor (or instance field initializer),
    //call it in Awake or Start instead. Called from MonoBehaviour 'ScriptB' on game object 'GameObjectB'.
    //
    //public ScriptA scriptA = FindObjectOfType<ScriptA>();

    // Start is called before the first frame update
    void Start()
    {
        ScriptA scriptA = FindObjectOfType<ScriptA>();
        Debug.Log("Debug from Sript B");
        this.gameObject.AddComponent<ScriptC>();
        scriptA.MethodFromA();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
