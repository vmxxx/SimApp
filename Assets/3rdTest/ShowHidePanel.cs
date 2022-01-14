using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHidePanel : MonoBehaviour
{

    public GameObject Panel;
    private bool isHidden = true;

    void ShowHide()
    {
        if (isHidden)
        {
            Panel.GetComponent<Animation>().Play("SlideLeftToRight");
            isHidden = false;
        }
        else
        {
            Panel.GetComponent<Animation>().Play("SlideRightToLeft");
            isHidden = true;
        }
    }
}
