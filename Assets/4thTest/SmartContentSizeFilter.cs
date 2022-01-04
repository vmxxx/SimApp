using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SmartContentSizeFilter : MonoBehaviour
{

    public RectTransform scrollArea;
    public RectTransform scrollAreaContentR;
    public Transform scrollAreaContentT;

    private float minHeight;

    // Start is called before the first frame update
    void Start()
    {
        fitContent();
    }

    // Update is called once per frame
    void Update()
    {
        fitContent();
    }

    void fitContent()
    {
        minHeight = scrollArea.rect.height;
        
        RectTransform lastChild = scrollAreaContentT.GetChild(scrollAreaContentT.childCount - 1).GetComponent<RectTransform>();

        float lastChildBottomSide = -lastChild.anchoredPosition.y + (lastChild.rect.height / 2);
        Debug.Log("lastChildBottomSide = -lastChild.anchoredPosition.y + (lastChild.rect.height / 2): " + lastChildBottomSide + " = " + -lastChild.anchoredPosition.y + " + ( " + lastChild.rect.height + " / " + 2 + ")");
        if(lastChildBottomSide > minHeight) scrollAreaContentR.sizeDelta = new Vector2(scrollArea.rect.width, lastChildBottomSide);
        else scrollAreaContentR.sizeDelta = new Vector2(scrollArea.rect.width, minHeight);

    }
}


public enum Fit
{
    unconstrained,
    preferredSize
};
/**/