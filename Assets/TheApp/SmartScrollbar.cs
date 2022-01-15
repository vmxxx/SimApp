using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartScrollbar : MonoBehaviour
{
    public RectTransform entryList;
    public RectTransform scrollArea;
    public GameObject scrollBar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        showHideScrollBar();
    }

    void showHideScrollBar()
    {
        if (entryList.rect.height > scrollArea.rect.height) { scrollBar.SetActive(true); Debug.Log("scrollBar.SetActive(true)"); }
        else { scrollBar.SetActive(false); Debug.Log("scrollBar.SetActive(false)"); } 
    }
}
