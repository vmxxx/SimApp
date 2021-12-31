using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;


public class AlignTableCells : MonoBehaviour
{
    public int childCount = 9;
    public GameObject payoffMatrixPanel;
    public GameObject[,] tableCells = new GameObject[10, 10];

    void AlignCells()
    {
        RectTransform rectTransform;
        int columnLength = (int)Round(Sqrt(childCount));
        float fullPanelWidth = payoffMatrixPanel.GetComponent<RectTransform>().rect.width;
        float squareLength = fullPanelWidth / columnLength;
        float zerothSquarePosition = fullPanelWidth / (float)(columnLength) / 2f;
        Debug.Log(fullPanelWidth);
        Debug.Log((float)(columnLength) / 2f);
        Debug.Log(zerothSquarePosition);
        for(int i = 0; i < columnLength; i++)
        {
            for(int j = 0; j < columnLength; j++)
            {
                rectTransform = tableCells[i, j].GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(squareLength, squareLength);
                rectTransform.anchoredPosition = new Vector2( (fullPanelWidth) * (((float)j*columnLength) /childCount) + zerothSquarePosition, (-fullPanelWidth) * (((float)i*columnLength)/childCount) - zerothSquarePosition);
                Debug.Log("i: " + i + " j: " + j);
                Debug.Log(rectTransform.anchoredPosition);

                /*
                rectTransform.anchoredPosition = new Vector2( fullPanelWidth * (columnLength*ii/2 / (columnLength * columnLength)) , ( fullPanelWidth * (columnLength * jj / 2 / (columnLength * columnLength)) ));
                rectTransform.sizeDelta = new Vector2(squareLength, squareLength);
                Debug.Log(rectTransform.anchoredPosition);
                */
            }
        }
    }

    void Awake()
    {
        tableCells[0, 0] = payoffMatrixPanel.transform.GetChild(0).gameObject;
        tableCells[0, 1] = payoffMatrixPanel.transform.GetChild(1).gameObject;
        tableCells[0, 2] = payoffMatrixPanel.transform.GetChild(2).gameObject;
        tableCells[1, 0] = payoffMatrixPanel.transform.GetChild(3).gameObject;
        tableCells[1, 1] = payoffMatrixPanel.transform.GetChild(4).gameObject;
        tableCells[1, 2] = payoffMatrixPanel.transform.GetChild(5).gameObject;
        tableCells[2, 0] = payoffMatrixPanel.transform.GetChild(6).gameObject;
        tableCells[2, 1] = payoffMatrixPanel.transform.GetChild(7).gameObject;
        tableCells[2, 2] = payoffMatrixPanel.transform.GetChild(8).gameObject;
        AlignCells();
    }
}
