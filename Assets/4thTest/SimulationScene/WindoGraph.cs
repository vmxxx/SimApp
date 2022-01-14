using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityEngine.UI;

[System.Serializable]
public class WindoGraph : MonoBehaviour
{
    public static WindoGraph instance;
    public Texture squareTexture;
    public Texture leftTriangleTexture;
    public Texture rightTriangleTexture;
    public GameObject newValInputBox;
    public List<float> initialValueList = new List<float>() { 1 };
    public float newValue;



    private float graphWidth;
    private float graphHeight;
    public float yMaximum = 1;
    public float oldYMaximum = 1;
    private float xSize;
    private int valueCount;
    private bool initiallyThereWasOneValue = true;


    private GameObject lastCircleGameObject = null;
    private string triangleDirection = "";
    private float squareHeight;
    private float triangleHeight;
    private float lastXPosition = 0f;
    private float lastYPosition = 0f;

    public GameObject XAxis;
    public GameObject YAxis;
    public GameObject[] XLabels = new GameObject[9];
    public GameObject[] YLabels = new GameObject[9];

    public float daysPassed = 0f;

    public void OnSubmit()
    {
        /*
        newValue = float.Parse(newValInputBox.transform.GetChild(2).GetComponent<Text>().text);
        addNewValue("Hawks");
        realignObjects("Doves");
        /**/
    }

    [SerializeField] private Sprite circleSprite;
    public RectTransform graphContainer;
    public GameObject emptyContainer;

    public void clearGraph()
    {
        /*
        for(int i = graphContainer.transform.childCount - 1; i > 3 ; i++)
        {
            Destroy(graphContainer.transform.GetChild(i).gameObject);
        }
        /**/
        newValue = 0;
        yMaximum = 10;
        oldYMaximum = 10;
        daysPassed = 1;
        graphHeight = graphContainer.sizeDelta.y;
        graphWidth = graphContainer.sizeDelta.x;

        for(int i = 2; i < YAxis.transform.childCount; i++)
        {
            YAxis.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, graphHeight / ((i - 1) * 7));
            YAxis.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = (10 / ((i - 1) * 7)).ToString();
        }

        for (int i = 2; i < XAxis.transform.childCount; i++)
        {
            XAxis.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 4; i < graphContainer.transform.childCount; i++)
        {
            Destroy(graphContainer.transform.GetChild(i).gameObject);
        }
        /**/
    }



    private GameObject CreateCircle(Vector2 anchoredPosition, GameObject agentContainer, Color circleColor)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(agentContainer.transform, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        gameObject.GetComponent<Image>().color = circleColor;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(3f, 3f);

        //keep everything on the lower left corner
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        return gameObject;

    }

    public void addInitialValue(float val, string container, Color agentColor)
    {
        GameObject agentContainer = Instantiate(emptyContainer);
        agentContainer.transform.SetParent(graphContainer.transform);
        agentContainer.name = container;

        valueCount = 1;
        graphWidth = graphContainer.sizeDelta.x;
        graphHeight = graphContainer.sizeDelta.y;
        //yMaximum = (val > yMaximum) ? val : yMaximum;
        //oldYMaximum = yMaximum;
        //xSize = graphWidth / (valueCount - 1);

        lastXPosition = 0f;
        lastYPosition = 0f;
        lastCircleGameObject = null;


        float xPosition = (0 * graphWidth);
        float yPosition = (val / yMaximum) * graphHeight;
        GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition), agentContainer, agentColor);
        lastCircleGameObject = circleGameObject;

        //lastXPosition = xPosition;
        //lastYPosition = yPosition;

        //oldYMaximum = yMaximum;
    }

    public void addNewValue(string container, Color agentColor)
    {
        GameObject agentContainer = graphContainer.transform.Find(container).gameObject;
        //newValue = Int32.Parse(newValInputBox.transform.GetChild(2).GetComponent<Text>().text);
        //valueCount = valueCount + 1;
        valueCount = (agentContainer.transform.childCount / 2) + 1 + 1;
        //graphWidth = graphContainer.sizeDelta.x;
        //graphHeight = graphContainer.sizeDelta.y;
        //yMaximum = (newValue > yMaximum) ? newValue : yMaximum;
        xSize = graphWidth / (valueCount - 1);
        int i = valueCount;

        float xPosition = (i * xSize);
        float yPosition = (newValue / yMaximum) * graphHeight;

        GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition), agentContainer, agentColor);
        if (valueCount != 2)
        {
            CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, agentContainer, agentColor);
        }
        else
        {
            CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, agentContainer, agentColor);
        }

        lastCircleGameObject = circleGameObject;
        lastXPosition = xPosition;
        lastYPosition = yPosition;

    }

    public void realignLabels(bool onScreenResize = false)
    {
        newProportion = graphHeight / oldGraphHeight;
        float newWidthProportion = graphWidth / oldGraphWidth;

        GameObject yAxisArrowHead = YAxis.transform.GetChild(1).gameObject;
        yAxisArrowHead.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, graphHeight - 5);

        Transform label;
        RectTransform rectTransform;
        float Y;
        float yPosition;
        float YlabelValue;
        //Y axis
        for (int i = YAxis.transform.childCount - 2; i >= 2; i--)
        {
            label = YAxis.transform.GetChild(i);
            rectTransform = label.GetComponent<RectTransform>();
            Y = rectTransform.anchoredPosition.y;

            yPosition = ((Y / (yMaximum / oldYMaximum)));
            YlabelValue = (Y / graphHeight) * yMaximum;

            yPosition = yPosition * newProportion;
            YlabelValue = YlabelValue * newProportion;

            rectTransform.anchoredPosition = new Vector2(0, yPosition);
            label.Find("Text").GetComponent<Text>().text = (YlabelValue).ToString();
            Y = rectTransform.anchoredPosition.y;

            if (Y < graphHeight - 30) { label.gameObject.SetActive(true); }
        }

        label = YAxis.transform.GetChild(10);
        rectTransform = label.GetComponent<RectTransform>();
        Y = rectTransform.anchoredPosition.y;

        yPosition = ((Y / (yMaximum / oldYMaximum)));
        YlabelValue = (Y / graphHeight) * yMaximum;

        yPosition = yPosition * newProportion;
        YlabelValue = YlabelValue * newProportion;


        rectTransform.anchoredPosition = new Vector2(0, yPosition);
        label.Find("Text").GetComponent<Text>().text = (YlabelValue).ToString();

        if (Y < graphHeight - 30)
        {
            for (int i = YAxis.transform.childCount - 1; i >= 2; i--)
            {
                int a = i - 2;
                label = YAxis.transform.GetChild(i);
                rectTransform = label.GetComponent<RectTransform>();

                yPosition = (((float)a / 7f) * graphHeight);
                YlabelValue = ((float)a / 7f) * yMaximum;

                yPosition = yPosition * newProportion;
                YlabelValue = YlabelValue * newProportion;

                rectTransform.anchoredPosition = new Vector2(0, yPosition);
                label.Find("Text").GetComponent<Text>().text = (YlabelValue).ToString();
                Y = rectTransform.anchoredPosition.y;

                if (Y >= graphHeight - 30) { label.gameObject.SetActive(false); }
            }
        }


        //X axis
        Debug.Log("graphWidth: " + graphWidth);

        GameObject xAxisArrowHead = XAxis.transform.GetChild(1).gameObject;
        xAxisArrowHead.GetComponent<RectTransform>().anchoredPosition = new Vector2(graphWidth - 5, 0);

        float X;
        float xPosition;
        float XlabelValue;
        
        for (int i = XAxis.transform.childCount - 2; i >= 2; i--)
        {
            int a = i - 1;


            label = XAxis.transform.GetChild(i);
            rectTransform = label.GetComponent<RectTransform>();
            xPosition = rectTransform.anchoredPosition.x;

            if(onScreenResize == false)
            {
                if (daysPassed > 1) { xPosition = xPosition / ((float)daysPassed / (float)(daysPassed - 1)); }
                else { xPosition = ((float)a / 7f) * graphWidth * (1 / (1f / 7f)); }
            }
            else
            {
                xPosition = xPosition * newWidthProportion;
            }
            //Debug.Log("xPosition: " + xPosition + ", xPosition * newWidthProportion: " + (xPosition * newWidthProportion));
            //xPosition = xPosition * newWidthProportion;


            XlabelValue = (xPosition / graphWidth) * daysPassed;



            rectTransform.anchoredPosition = new Vector2(xPosition, 0);
            label.Find("Text").GetComponent<Text>().text = (XlabelValue).ToString();

            if (onScreenResize == false) { if (xPosition < graphWidth - 30) { label.gameObject.SetActive(true); Debug.Log("label.gameObject.SetActive(true);"); } }
        }
        /**/

        
        label = XAxis.transform.GetChild(10);
        rectTransform = label.GetComponent<RectTransform>();
        xPosition = rectTransform.anchoredPosition.x;

        if(onScreenResize == false)
        {
            if (daysPassed > 1) { xPosition = (xPosition / ((float)daysPassed / (float)(daysPassed - 1))); }
            else { xPosition = (9f / 7f) * graphWidth * (1 / (1f / 7f)); }
        }
        else
        {
            xPosition = xPosition * newWidthProportion;
        }
        XlabelValue = (xPosition / graphWidth) * daysPassed;

        rectTransform.anchoredPosition = new Vector2(xPosition, 0);
        label.Find("Text").GetComponent<Text>().text = (XlabelValue).ToString();

        //Debug.Log("(xPosition < graphWidth - 30): " + (xPosition < graphWidth - 30) + ", xPosition: " + xPosition + ", graphWidth: " + graphWidth);
        
        if(onScreenResize == false)
        {
        if (xPosition < graphWidth - 30)
        {
            for (int i = XAxis.transform.childCount - 1; i >= 2; i--)
            {
                int a = i - 1;
                label = XAxis.transform.GetChild(i);
                rectTransform = label.GetComponent<RectTransform>();

                if (onScreenResize == false)
                {
                    xPosition = (((float)a / 7f) * graphWidth);
                    XlabelValue = (xPosition / graphWidth) * daysPassed;
                }
                else
                {
                    xPosition = xPosition * newWidthProportion;
                }
                Debug.Log(i + ". XlabelValue = (xPosition / graphWidth) * daysPassed: " + XlabelValue + " = (" + xPosition + " / " + graphWidth + ") * " + daysPassed);



                rectTransform.anchoredPosition = new Vector2(xPosition, 0);
                label.Find("Text").GetComponent<Text>().text = (XlabelValue).ToString();

                if (xPosition >= graphWidth - 30) { label.gameObject.SetActive(false); Debug.Log("label.gameObject.SetActive(false);"); }
            }
        }
        }
    }


    /* The NEW STUFF ---------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public RectTransform windoGraph;
    public float oldGraphHeight;
    public float oldGraphWidth;
    public float newProportion;

    public void Update()
    {
        graphHeight = graphContainer.sizeDelta.y;
        graphWidth = graphContainer.sizeDelta.x;
        graphContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(windoGraph.rect.width - 20, windoGraph.rect.height - 20);
        graphContainer.GetComponent<RectTransform>().anchoredPosition = new Vector2(windoGraph.rect.width / 2, windoGraph.rect.height / 2);

        for(int i = 4; i < graphContainer.transform.childCount; i++)
        {
            realignObjects( graphContainer.transform.GetChild(i).name );
        }
        realignLabels(true);

        oldGraphHeight = graphHeight;
        oldGraphWidth = graphWidth;


        if (WindoGraph.instance == null)
        {
            instance = this;
        }
    }

    private void Awake()
    {
        if (WindoGraph.instance == null)
        {
            instance = this;
        }

        graphHeight = graphContainer.sizeDelta.y;
        graphWidth = graphContainer.sizeDelta.x;
        graphContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(windoGraph.rect.width - 20, windoGraph.rect.height - 20);
        graphContainer.GetComponent<RectTransform>().anchoredPosition = new Vector2(windoGraph.rect.width / 2, windoGraph.rect.height / 2);
        oldGraphHeight = graphHeight;
        oldGraphWidth = graphWidth;

    }
    /* The NEW STUFF ---------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public void realignObjects(string container)
    {

        GameObject agentContainer = graphContainer.transform.Find(container).gameObject;
        //int b = (valueCount == 2) ? 4 : 2;
        //int b = (valueCount == 2) ? 2 : 2;
        //int b = (valueCount == 1) ? 1 : 1;
        valueCount = (agentContainer.transform.childCount / 2) + 1;
        graphWidth = graphContainer.sizeDelta.x;
        graphHeight = graphContainer.sizeDelta.y;
        //float oldYMaximum = yMaximum;
        //yMaximum = (newValue > yMaximum) ? newValue : yMaximum;
        xSize = (valueCount - 1 != 0) ? (graphWidth / (valueCount - 1)) : (0);

        GameObject circleGameObject;
        /*
        GameObject circleGameObject = agentContainer.transform.GetChild(1).gameObject;
        float Y = circleGameObject.GetComponent<RectTransform>().anchoredPosition.y;
        float xPosition = (0 * xSize);
        float yPosition = ((Y / (yMaximum / oldYMaximum)));
        float lastYPosition = yPosition;
        circleGameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosition, yPosition);
        /**/

        newProportion = graphHeight / oldGraphHeight;

        lastCircleGameObject = agentContainer.transform.GetChild(0).gameObject;
        float Y = lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition.y;
        float xPosition = (0 * xSize);
        float yPosition = (oldYMaximum != 0) ? ((Y / (yMaximum / oldYMaximum))) : Y;
        yPosition = yPosition * newProportion;
        float lastYPosition = yPosition;
        lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosition, yPosition);
        for (int i = 1; i < valueCount; i++)
        {
            int a = i - 1;
            circleGameObject = agentContainer.transform.GetChild(1 + (2 * a) + 0).gameObject;
            GameObject dotConnection = agentContainer.transform.GetChild(1 + (2 * a) + 1).gameObject;

            Y = circleGameObject.GetComponent<RectTransform>().anchoredPosition.y;
            xPosition = (i * xSize);
            yPosition = ((Y / (yMaximum / oldYMaximum)));
            yPosition = yPosition * newProportion;

            circleGameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosition, yPosition);
            realignDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, dotConnection);

            lastCircleGameObject = circleGameObject;
            lastYPosition = yPosition;
        }
    }

    private void realignDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, GameObject dotConnection)
    {
        RectTransform rectTransform = dotConnection.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, GameObject agentContainer, Color connectionColor)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(agentContainer.transform, false);
        gameObject.GetComponent<Image>().color = connectionColor;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
        //return gameObject;
    }

    private static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public float FindHighestValueInAList(List<float> list)
    {
        if (list.Count == 0)
        {
            return 1;
        }
        float maxAge = float.MinValue;
        float itr = 0;
        foreach (float val in list)
        {
            itr = itr + 1;
            if (val > maxAge)
            {
                maxAge = val;
            }
        }
        return maxAge;
    }
}