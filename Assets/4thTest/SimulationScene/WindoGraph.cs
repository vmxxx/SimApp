using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityEngine.UI;

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
    public  float yMaximum = 1;
    public  float oldYMaximum = 1;
    private float xSize;
    private int   valueCount;
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
        newValue = float.Parse(newValInputBox.transform.GetChild(2).GetComponent<Text>().text);
        addNewValue("Hawks");
        realignObjects("Doves");
    }

    [SerializeField] private Sprite circleSprite;
    public RectTransform graphContainer;
    public GameObject emptyContainer;


    private void Awake()
    {
        if (WindoGraph.instance == null)
        {
            instance = this;
        }


        //addInitialValue(10, "Hawks");
        //addInitialValue(10, "Doves");
        //ShowGraphWithInitialValueList(initialValueList, "Hawks");
        //ShowGraphWithInitialValueList(initialValueList, "Doves");
    }

    private GameObject CreateCircle(Vector2 anchoredPosition, GameObject agentContainer)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(agentContainer.transform, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(3f, 3f);

        //keep everything on the lower left corner
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        return gameObject;

    }

    public void addInitialValue(float val, string container)
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
        GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition), agentContainer);
        lastCircleGameObject = circleGameObject;

        //lastXPosition = xPosition;
        //lastYPosition = yPosition;

        //oldYMaximum = yMaximum;
    }

    /*
    private void ShowGraphWithInitialValueList(List<float> valueList, string container)
    {
        GameObject agentContainer = Instantiate(emptyContainer);
        agentContainer.transform.SetParent(graphContainer.transform);
        agentContainer.name = container;

        valueCount = valueList.Count;
        graphWidth = graphContainer.sizeDelta.x;
        graphHeight = graphContainer.sizeDelta.y;
        yMaximum = FindHighestValueInAList(valueList);
        xSize = graphWidth / (valueCount - 1);

        triangleDirection = "";
        squareHeight = 0;
        triangleHeight = 0;
        lastXPosition = 0f;
        lastYPosition = 0f;
        lastCircleGameObject = null;
        if (valueCount != 1)
        {
            for (int i = 0; i < valueCount; i++)
            {
                float xPosition = (i * xSize);
                float yPosition = (valueList[i] / yMaximum) * graphHeight;
                GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition), agentContainer);
                if (lastCircleGameObject != null)
                {
                    CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, agentContainer);
                }
                lastCircleGameObject = circleGameObject;
                lastXPosition = xPosition;
                lastYPosition = yPosition;
            }
        }
        else
        {
            float xPosition = (0 * graphWidth);
            float yPosition = (valueList[0] / yMaximum) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition), agentContainer);
            lastCircleGameObject = circleGameObject;

            lastXPosition = xPosition;
            lastYPosition = yPosition;
        }
        oldYMaximum = yMaximum;
    }
        /**/

    public void addNewValue(string container)
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

        GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition), agentContainer);
        if(valueCount != 2)
        {
            CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, agentContainer);
        }
        else
        {
            CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, agentContainer);
        }

        lastCircleGameObject = circleGameObject;
        lastXPosition = xPosition;
        lastYPosition = yPosition;

    }

    public void realignLabels()
    {
        
        Debug.Log("yMaximum: " + yMaximum);

        Transform label;
        RectTransform rectTransform;
        float Y;
         //Y axis
        for (int i = YAxis.transform.childCount - 2; i >= 3; i--)
        {
            label = YAxis.transform.GetChild(i);
            rectTransform = label.GetComponent<RectTransform>();
            Y = rectTransform.anchoredPosition.y;
            rectTransform.anchoredPosition = new Vector2(0, ((Y / (yMaximum / oldYMaximum))));
            label.Find("Text").GetComponent<Text>().text = ((Y / graphHeight) * yMaximum).ToString();
            Y = rectTransform.anchoredPosition.y;

            if (Y < graphHeight - 30) { label.gameObject.SetActive(true); }
        }

        label = YAxis.transform.GetChild(11);
        rectTransform = label.GetComponent<RectTransform>();
        Y = rectTransform.anchoredPosition.y;
        rectTransform.anchoredPosition = new Vector2(0, ((Y / (yMaximum / oldYMaximum))));
        label.Find("Text").GetComponent<Text>().text = ((Y / graphHeight) * yMaximum).ToString();

        if (Y < graphHeight - 30)
        {
            for (int i = YAxis.transform.childCount - 1; i >= 3; i--)
            {
                int a = i - 2;
                label = YAxis.transform.GetChild(i);
                rectTransform = label.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, (((float)a / 7f) * graphHeight));
                label.Find("Text").GetComponent<Text>().text = (((float)a / 7f) * yMaximum).ToString();
                Y = rectTransform.anchoredPosition.y;

                if (Y >= graphHeight - 30) { label.gameObject.SetActive(false); }
            }
        }


        //X axis
        float X;
        for (int i = XAxis.transform.childCount - 2; i >= 3; i--)
        {
            int a = i - 2;
            label = XAxis.transform.GetChild(i);
            rectTransform = label.GetComponent<RectTransform>();
            X = rectTransform.anchoredPosition.x;
            if (daysPassed > 1) { X = (X / ((float)daysPassed / (float)(daysPassed - 1))); }
            rectTransform.anchoredPosition = new Vector2(X, 0);
            label.Find("Text").GetComponent<Text>().text = (((float)a / 7f) * daysPassed).ToString();

            if (X < graphWidth - 30) { label.gameObject.SetActive(true); }
        }

        label = XAxis.transform.GetChild(11);
        rectTransform = label.GetComponent<RectTransform>();
        X = rectTransform.anchoredPosition.x;
        if (daysPassed > 1) { X = (X / ((float)daysPassed / (float)(daysPassed - 1))); }
        rectTransform.anchoredPosition = new Vector2(X, 0);
        label.Find("Text").GetComponent<Text>().text = ((9f / 7f) * daysPassed).ToString();

        if (X < graphWidth - 30)
        {
            for (int i = XAxis.transform.childCount - 1; i >= 3; i--)
            {
                int a = i - 2;
                label = XAxis.transform.GetChild(i);
                rectTransform = label.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2((((float)a / 7f) * graphWidth), 0);
                label.Find("Text").GetComponent<Text>().text = (((float)a / 7f) * daysPassed).ToString();
                X = rectTransform.anchoredPosition.x;

                if (X >= graphWidth - 30) { label.gameObject.SetActive(false); }
            }
        }




        /*
        
        for (int i = 3; i < XAxis.transform.childCount; i++)
        {
            Transform label = XAxis.transform.GetChild(i);
            RectTransform rectTransform = label.GetComponent<RectTransform>();
            float X = rectTransform.anchoredPosition.x;
            if (daysPassed > 1) {

                X = (X / ((float)daysPassed / (float)(daysPassed - 1))); 
            
            }
            rectTransform.anchoredPosition = new Vector2(X, 0);
            rectTransform.transform.GetChild(0).GetComponent<Text>().text = (daysPassed / 7f * (float)(i + 1)).ToString();
        }

        if (XLabels[8].GetComponent<RectTransform>().anchoredPosition.x < (graphWidth))
        {
            XLabels[8].SetActive(false);
            XLabels[7].SetActive(false);
            XLabels[6].SetActive(false);
            
            for (int i = 0; i < 9; i++)
            {
                RectTransform rectTransform = XLabels[i].GetComponent<RectTransform>();
                float X = (graphWidth / 7 * (i + 1));
                rectTransform.anchoredPosition = new Vector2(X, 0);

                //Debug.Log("(daysPassed / 7f * (float)(i + 1)): " + (daysPassed / 7f * (float)(i + 1)));
                XLabels[i].transform.GetChild(0).GetComponent<Text>().text = (daysPassed / 7f * (float)(i + 1)).ToString();
            }
        }
        else if (XLabels[7].GetComponent<RectTransform>().anchoredPosition.x < (graphWidth))
        {
            XLabels[7].SetActive(true);
        }
        else if (XLabels[6].GetComponent<RectTransform>().anchoredPosition.x < (graphWidth))
        {
            XLabels[6].SetActive(true);
        }



        /**/


    }

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

        lastCircleGameObject = agentContainer.transform.GetChild(0).gameObject;
        float Y = lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition.y;
        float xPosition = (0 * xSize);
        float yPosition = (oldYMaximum != 0) ? ((Y / (yMaximum / oldYMaximum))) : Y;
        float lastYPosition = yPosition;
        Debug.Log("yMaximum: " + yMaximum);
        Debug.Log("oldYMaximum: " + oldYMaximum);
        Debug.Log(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition + " / " + new Vector2(xPosition, yPosition));
        lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosition, yPosition);
        for (int i = 1; i < valueCount; i++)
        {
            int a = i - 1;
            circleGameObject = agentContainer.transform.GetChild(1 + (2 * a) + 0).gameObject;
            GameObject dotConnection = agentContainer.transform.GetChild(1 + (2 * a) + 1).gameObject;

            Y = circleGameObject.GetComponent<RectTransform>().anchoredPosition.y;
            xPosition = (i * xSize);
            yPosition = ((Y / (yMaximum / oldYMaximum)));


            //Debug.Log("yPosition = ((Y / (yMaximum / oldYMaximum)) * graphHeight) = " + yPosition + "= ((" + Y + "/" + "(" + yMaximum + "/" + oldYMaximum + ")) *" + ")) *);");
            //Debug.Log("yPosition = ((Y / (yMaximum / oldYMaximum)) = " + yPosition + "= ((" + Y + "/" + "(" + yMaximum + "/" + oldYMaximum + ")) *);");

            circleGameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosition, yPosition);
            realignDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, dotConnection);

            lastCircleGameObject = circleGameObject;
            lastYPosition = yPosition;
        }

        /*
        for (int i = 3; i < YAxis.transform.childCount; i++)
        {
            Transform label = YAxis.transform.GetChild(i);
            RectTransform rectTransform = label.GetComponent<RectTransform>();
            Y = rectTransform.anchoredPosition.y;
            rectTransform.anchoredPosition = new Vector2(0, ((Y / (yMaximum / oldYMaximum))));
        }
        for (int i = 3; i < XAxis.transform.childCount; i++)
        {
            Transform label = XAxis.transform.GetChild(i);
            RectTransform rectTransform = label.GetComponent<RectTransform>();
            float X = rectTransform.anchoredPosition.x;
            if (daysPassed > 1) {

                X = (X / ((float)daysPassed / (float)(daysPassed - 1))); 
            
            }
            rectTransform.anchoredPosition = new Vector2(X, 0);
            rectTransform.transform.GetChild(0).GetComponent<Text>().text = (daysPassed / 7f * (float)(i + 1)).ToString();
        }


        if (YLabels[8].GetComponent<RectTransform>().anchoredPosition.y < (graphHeight ))
        {
            YLabels[8].SetActive(false);
            YLabels[7].SetActive(false);
            YLabels[6].SetActive(false);
            for (int i = 0; i < 9; i++)
            {
                RectTransform rectTransform = YLabels[i].GetComponent<RectTransform>();
                Y = (graphHeight / 7 * (i + 1));
                rectTransform.anchoredPosition = new Vector2(0, Y);

                YLabels[i].transform.GetChild(0).GetComponent<Text>().text = (yMaximum / 7 * (i + 1)).ToString();
            }
        }
        else if (YLabels[7].GetComponent<RectTransform>().anchoredPosition.y < (graphHeight ))
        {
            YLabels[7].SetActive(true);
        }
        else if (YLabels[6].GetComponent<RectTransform>().anchoredPosition.y < (graphHeight ))
        {
            YLabels[6].SetActive(true);
        }

        
        //Debug.Log("Days passed: " + daysPassed);
        if (XLabels[8].GetComponent<RectTransform>().anchoredPosition.x < (graphWidth))
        {
            XLabels[8].SetActive(false);
            XLabels[7].SetActive(false);
            XLabels[6].SetActive(false);
            
            for (int i = 0; i < 9; i++)
            {
                RectTransform rectTransform = XLabels[i].GetComponent<RectTransform>();
                float X = (graphWidth / 7 * (i + 1));
                rectTransform.anchoredPosition = new Vector2(X, 0);

                //Debug.Log("(daysPassed / 7f * (float)(i + 1)): " + (daysPassed / 7f * (float)(i + 1)));
                XLabels[i].transform.GetChild(0).GetComponent<Text>().text = (daysPassed / 7f * (float)(i + 1)).ToString();
            }
        }
        else if (XLabels[7].GetComponent<RectTransform>().anchoredPosition.x < (graphWidth))
        {
            XLabels[7].SetActive(true);
        }
        else if (XLabels[6].GetComponent<RectTransform>().anchoredPosition.x < (graphWidth))
        {
            XLabels[6].SetActive(true);
        }
        /**/
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

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, GameObject agentContainer)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(agentContainer.transform, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
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
