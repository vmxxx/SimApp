using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RectTransformInPercentage : MonoBehaviour
{

    public GameObject canvas;
    private float[] prevResolution;
    private float[] currResolution;

    public AnchorPreset verticalAnchor   = new AnchorPreset();
    public AnchorPreset horizontalAnchor = new AnchorPreset();

    public bool leftInPercentage   = true;
    public bool topInPercentage    = true;
    public bool rightInPercentage  = true;
    public bool bottomInPercentage = true;

    public float left;
    public float top;
    public float right;
    public float bottom;
    private float recalculatedLeft;
    private float recalculatedTop;
    private float recalculatedRight;
    private float recalculatedBottom;

    private void Start()
    {

        recalculateRectTransform();

        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        prevResolution = new float[] { canvasRectTransform.rect.width, canvasRectTransform.rect.height };
    }

    private void Update()
    {

        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        currResolution = new float[2] { canvasRectTransform.rect.width, canvasRectTransform.rect.height };

        if (prevResolution[0] != currResolution[0] || prevResolution[1] != currResolution[1])
        {
            Debug.Log("new resolution");
            prevResolution = new float[2] { currResolution[0], currResolution[1] } ;
            recalculateRectTransform();
        }
    }

    private void recalculateRectTransform()
    {
        RectTransform parentRectTransform = (RectTransform)gameObject.transform.parent;
        RectTransform rectTransform       = gameObject.GetComponent<RectTransform>();
        float parentWidth  = parentRectTransform.rect.width;
        float parentHeight = parentRectTransform.rect.height;

        if (leftInPercentage == true)   { recalculatedLeft   = (  left / 100) * parentWidth;  }
        if (rightInPercentage == true)  { recalculatedRight  = ( right / 100) * parentWidth;  }
        if (topInPercentage == true)    { recalculatedTop    = (   top / 100) * parentHeight; }
        if (bottomInPercentage == true) { recalculatedBottom = (bottom / 100) * parentHeight; }

        rectTransform.offsetMin = new Vector2(recalculatedLeft, recalculatedBottom);
        rectTransform.offsetMax = new Vector2(-recalculatedRight, -recalculatedTop);

        if(PayoffMatrix.instance != null)
        {
            PayoffMatrix.instance.alignCells();
        }
    }
}

public enum AnchorPreset
{
    stretch
};