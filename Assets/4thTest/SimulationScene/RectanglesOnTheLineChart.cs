/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class RectanglesOnTheLineChart : MonoBehaviour
{

    public int XaxisGranularity = 10;
    public int YaxisGranularity = 10;
    public GameObject lineChart;

    private void createDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.parent = lineChart.transform;
        gameObject.GetComponent<Image>.color = new Color(1, 1, 1, 0.5f);

        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.distance(dotPositionA, dotPositionB);

        RectTransform rt3 = gameObject.GetComponent<RectTransform>();
        rt3.sizeDelta = new Vector2(100, 3f);
        rt3.anchoredPosition = dotPositionA;
    }

    void Start()
    {

        createDotConnection(new Vector2(0, 0), new Vector2(50, 50));

        RectTransform rt = (RectTransform)lineChart.transform;
        float lineChartWidth = rt.rect.width;
        float lineChartHeight = rt.rect.height;

        GameObject[,] squares = new GameObject[XaxisGranularity, YaxisGranularity];
        for (int x = 1; x <= XaxisGranularity; x++)
        {
            for (int y = 1; y <= YaxisGranularity; y++)
            {
                squares[x-1, y-1] = new GameObject("Square", typeof(Image));
                squares[x-1, y-1].transform.parent = lineChart.transform;

                RectTransform rt2 = squares[x-1, y-1].GetComponent<RectTransform>();
                rt2.sizeDelta = new Vector2(50, 50);

                squares[x-1, y-1].transform.position = new Vector3((lineChartWidth / XaxisGranularity) * x, (lineChartHeight / YaxisGranularity) * y, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
*/