using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptA : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Debug from Sript A");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MethodFromA()
    {
        Debug.Log("This is extra debug from Sript A, which should be called from script B.");
    }
}
