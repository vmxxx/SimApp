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
    private float yMaximum;
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

    private float daysPassed = 0f;

    public void OnSubmit()
    {
        newValue = float.Parse(newValInputBox.transform.GetChild(2).GetComponent<Text>().text);
        addNewValue();
        realignObjects();
    }

    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;


    private void Awake()
    {
        if (WindoGraph.instance == null)
        {
            instance = this;
        }

        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();

        ShowGraphWithInitialValueList(initialValueList);
    }

    private void CreateSquareAndTriangle(Vector2 squarePosition, Vector2 trianglePosition, float width, float squareHeight, float triangleHeight, string triangleDirection)
    {
        GameObject gameObject = new GameObject("square", typeof(RawImage));
        gameObject.transform.SetParent(graphContainer, false);
        RawImage rawImage = gameObject.GetComponent<RawImage>();
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rawImage.texture = squareTexture;
        rectTransform.anchoredPosition = squarePosition;
        rectTransform.sizeDelta = new Vector2(width, squareHeight);

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);



        gameObject = new GameObject("triangle", typeof(RawImage));
        gameObject.transform.SetParent(graphContainer, false);
        rawImage = gameObject.GetComponent<RawImage>();
        rectTransform = gameObject.GetComponent<RectTransform>();
        //Debug.Log("triangle direction: " + triangleDirection);
        if (triangleDirection == "left") { rawImage.texture = leftTriangleTexture; }
        else { rawImage.texture = rightTriangleTexture; }
        rectTransform.anchoredPosition = trianglePosition;
        rectTransform.sizeDelta = new Vector2(width, triangleHeight);

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

    }

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(3f, 3f);

        //keep everything on the lower left corner
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        return gameObject;

    }

    private void ShowGraphWithInitialValueList(List<float> valueList)
    {
        
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
                GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
                if (lastCircleGameObject != null)
                {
                    CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                    /*
                    if (lastYPosition < yPosition) { squareHeight = lastYPosition; triangleDirection = "right"; triangleHeight = yPosition - lastYPosition; }
                    else { squareHeight = yPosition; triangleDirection = "left"; triangleHeight = lastYPosition - yPosition; }
                    CreateSquareAndTriangle(new Vector2(xPosition - (xSize / 2), squareHeight / 2), new Vector2(xPosition - (xSize / 2), squareHeight + (triangleHeight / 2)), xSize, squareHeight, triangleHeight, triangleDirection);
                    /**/
                }
                lastCircleGameObject = circleGameObject;
                lastXPosition = xPosition;
                lastYPosition = yPosition;
            }
        }
        else
        {
            float xPosition = (1 * graphWidth);
            float yPosition = (valueList[0] / yMaximum) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            lastCircleGameObject = circleGameObject;

            lastXPosition = xPosition;
            lastYPosition = yPosition;

            //append the single square
            /*
            CreateSquareAndTriangle(new Vector2(graphWidth / 2, graphHeight / 2), new Vector2(0, 0), graphWidth, graphHeight, 0, "left");
            /**/
        }
        /**/
    }


    private void showGraphWithInitialValues(float[] initialValues)
    {
        /*
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
                GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
                if (lastCircleGameObject != null)
                {
                    CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                    if (lastYPosition < yPosition) { squareHeight = lastYPosition; triangleDirection = "right"; triangleHeight = yPosition - lastYPosition; }
                    else { squareHeight = yPosition; triangleDirection = "left"; triangleHeight = lastYPosition - yPosition; }
                    CreateSquareAndTriangle(new Vector2(xPosition - (xSize / 2), squareHeight / 2), new Vector2(xPosition - (xSize / 2), squareHeight + (triangleHeight / 2)), xSize, squareHeight, triangleHeight, triangleDirection);
                }
                lastCircleGameObject = circleGameObject;
                lastXPosition = xPosition;
                lastYPosition = yPosition;
            }
        }
        else
        {
            float xPosition = (1 * graphWidth);
            float yPosition = (valueList[0] / yMaximum) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            lastCircleGameObject = circleGameObject;

            lastXPosition = xPosition;
            lastYPosition = yPosition;

            //append the single square
            CreateSquareAndTriangle(new Vector2(graphWidth / 2, graphHeight / 2), new Vector2(0, 0), graphWidth, graphHeight, 0, "left");
        }
        /**/
    }

    public void addNewValue()
    {

        //newValue = Int32.Parse(newValInputBox.transform.GetChild(2).GetComponent<Text>().text);
        valueCount = valueCount + 1;
        //graphWidth = graphContainer.sizeDelta.x;
        //graphHeight = graphContainer.sizeDelta.y;
        //yMaximum = (newValue > yMaximum) ? newValue : yMaximum;
        //xSize = graphWidth / (valueCount - 1);

        int i = valueCount - 1;

        float xPosition = (i * xSize);
        float yPosition = (newValue / yMaximum) * graphHeight;
        GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
        if(valueCount != 2)
        {
            CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            /*
            if (lastYPosition < yPosition) { squareHeight = lastYPosition; triangleDirection = "right"; triangleHeight = yPosition - lastYPosition; }
            else { squareHeight = yPosition; triangleDirection = "left"; triangleHeight = lastYPosition - yPosition; }
            CreateSquareAndTriangle(new Vector2(xPosition - (xSize / 2), squareHeight / 2), new Vector2(xPosition - (xSize / 2), squareHeight + (triangleHeight / 2)), xSize, squareHeight, triangleHeight, triangleDirection);
            /**/
        }
        else
        {

            CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            /*
            if (lastYPosition < yPosition) { squareHeight = lastYPosition; triangleDirection = "right"; triangleHeight = yPosition - lastYPosition; }
            else { squareHeight = yPosition; triangleDirection = "left"; triangleHeight = lastYPosition - yPosition; }
            CreateSquareAndTriangle(new Vector2(xPosition - (xSize / 2), squareHeight / 2), new Vector2(xPosition - (xSize / 2), squareHeight + (triangleHeight / 2)), xSize, squareHeight, triangleHeight, triangleDirection);
            /**/


            //Destroy(graphContainer.transform.GetChild(2).gameObject);
            //Destroy(graphContainer.transform.GetChild(3).gameObject);
        }

        lastCircleGameObject = circleGameObject;
        lastXPosition = xPosition;
        lastYPosition = yPosition;

    }

    public void realignObjects()
    {
        //int b = (valueCount == 2) ? 4 : 2;
        int b = (valueCount == 2) ? 2 : 2;
        graphWidth = graphContainer.sizeDelta.x;
        graphHeight = graphContainer.sizeDelta.y;
        float oldYMaximum = yMaximum;
        yMaximum = (newValue > yMaximum) ? newValue : yMaximum;
        xSize = graphWidth / (valueCount - 1);


        GameObject circleGameObject = graphContainer.transform.GetChild(1).gameObject;
        float Y = circleGameObject.GetComponent<RectTransform>().anchoredPosition.y;
        float xPosition = (0 * xSize);
        float yPosition = ((Y / (yMaximum / oldYMaximum)));
        float lastYPosition = yPosition;
        circleGameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosition, yPosition);
        lastCircleGameObject = circleGameObject;

        for (int i = 1; i < valueCount; i++)
        {
            int a = i - 1;
            circleGameObject = graphContainer.transform.GetChild(b + (2 * a) + 0).gameObject;
            GameObject dotConnection = graphContainer.transform.GetChild(b + (2 * a) + 1).gameObject;
            /*
            circleGameObject = graphContainer.transform.GetChild(b + (4 * a) + 0).gameObject;
            GameObject dotConnection = graphContainer.transform.GetChild(b + (4 * a) + 1).gameObject;
            GameObject square = graphContainer.transform.GetChild(b + (4 * a) + 2).gameObject;
            GameObject triangle = graphContainer.transform.GetChild(b + (4 * a) + 3).gameObject;
            /**/

            Y = circleGameObject.GetComponent<RectTransform>().anchoredPosition.y;
            xPosition = (i * xSize);
            yPosition = ((Y / (yMaximum / oldYMaximum)));


            //Debug.Log("yPosition = ((Y / (yMaximum / oldYMaximum)) * graphHeight) = " + yPosition + "= ((" + Y + "/" + "(" + yMaximum + "/" + oldYMaximum + ")) *" + ")) *);");
            //Debug.Log("yPosition = ((Y / (yMaximum / oldYMaximum)) = " + yPosition + "= ((" + Y + "/" + "(" + yMaximum + "/" + oldYMaximum + ")) *);");

            circleGameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosition, yPosition);
            realignDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, dotConnection);

            /*
            if (lastYPosition < yPosition) { squareHeight = lastYPosition; triangleDirection = "right"; triangleHeight = yPosition - lastYPosition; }
            else { squareHeight = yPosition; triangleDirection = "left"; triangleHeight = lastYPosition - yPosition; }
            realignSquareAndTriangle(new Vector2(xPosition - (xSize / 2), squareHeight / 2), new Vector2(xPosition - (xSize / 2), squareHeight + (triangleHeight / 2)), xSize, squareHeight, triangleHeight, triangleDirection, square, triangle);
            /**/

            lastCircleGameObject = circleGameObject;
            lastYPosition = yPosition;
        }

        daysPassed++;

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


        if (YLabels[8].GetComponent<RectTransform>().anchoredPosition.y < (graphHeight - 30))
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
        else if (YLabels[7].GetComponent<RectTransform>().anchoredPosition.y < (graphHeight - 30))
        {
            YLabels[7].SetActive(true);
        }
        else if (YLabels[6].GetComponent<RectTransform>().anchoredPosition.y < (graphHeight - 30))
        {
            YLabels[6].SetActive(true);
        }

        
        Debug.Log("Days passed: " + daysPassed);
        if (XLabels[8].GetComponent<RectTransform>().anchoredPosition.x < (graphWidth - 30))
        {
            XLabels[8].SetActive(false);
            XLabels[7].SetActive(false);
            XLabels[6].SetActive(false);
            
            for (int i = 0; i < 9; i++)
            {
                RectTransform rectTransform = XLabels[i].GetComponent<RectTransform>();
                float X = (graphWidth / 7 * (i + 1));
                rectTransform.anchoredPosition = new Vector2(X, 0);

                Debug.Log("(daysPassed / 7f * (float)(i + 1)): " + (daysPassed / 7f * (float)(i + 1)));
                XLabels[i].transform.GetChild(0).GetComponent<Text>().text = (daysPassed / 7f * (float)(i + 1)).ToString();
            }
            /**/
        }
        else if (XLabels[7].GetComponent<RectTransform>().anchoredPosition.x < (graphWidth - 30))
        {
            XLabels[7].SetActive(true);
        }
        else if (XLabels[6].GetComponent<RectTransform>().anchoredPosition.x < (graphWidth - 30))
        {
            XLabels[6].SetActive(true);
        }
        /**/
    }

    private void realignSquareAndTriangle(Vector2 squarePosition, Vector2 trianglePosition, float width, float squareHeight, float triangleHeight, string triangleDirection, GameObject square, GameObject triangle)
    {
        RectTransform rectTransform = square.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = squarePosition;
        rectTransform.sizeDelta = new Vector2(width, squareHeight);

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);



        rectTransform = triangle.GetComponent<RectTransform>();
        //Debug.Log("triangle direction: " + triangleDirection);
        rectTransform.anchoredPosition = trianglePosition;
        rectTransform.sizeDelta = new Vector2(width, triangleHeight);

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

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

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
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
