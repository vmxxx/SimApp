using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;
using UnityEngine.UI;


public class PayoffMatrix : MonoBehaviour
{
    [SerializeField] private Text text;

    public GameObject emptyCellText;

    public static PayoffMatrix instance;

    public GameObject addExtraCell;
    public GameObject emptyCell;
    public GameObject unavaibleCell;
    public GameObject dragAndDropCell;

    public int childCount = 9;
    public GameObject payoffMatrixPanel;
    public GameObject[,] tableCells = new GameObject[10, 10];

    public void setText(string formula, int j, int k)
    {
        tableCells[j, k].transform.Find("Formula").Find("Text").GetComponent<Text>().text = formula;
        Debug.Log(tableCells[j, k].transform.Find("Formula").Find("Text").GetComponent<Text>().text);
        tableCells[j, k].transform.Find("Formula").Find("Text").name = formula;
    }

    public void remove(GameObject cell)
    {
        GameObject temp;
        int agentCount = (int)Round(Sqrt(childCount));
        if(cell.name[10] == '0')
        {
            int Index = (int)(cell.name[12] - '0');
            for (int i = 0; i < agentCount; i++) { Debug.Log("XIndex: " + Index + " i: " + i); Destroy(tableCells[i, Index]); }
            for (int i = 0; i < agentCount; i++) { Debug.Log("i: " + i + " YIndex: " + Index); Destroy(tableCells[Index, i]); }
            childCount = (agentCount - 1) * (agentCount - 1);
            realignCells(Index, agentCount);
        }
        else if (cell.name[12] == '0')
        {
            int Index = (int)(cell.name[10] - '0');
            for (int i = 0; i < agentCount; i++) { Debug.Log("XIndex: " + Index + " i: " + i); Destroy(tableCells[i, Index]); }
            for (int i = 0; i < agentCount; i++) { Debug.Log("i: " + i + " YIndex: " + Index); Destroy(tableCells[Index, i]); }
            childCount = (agentCount - 1) * (agentCount - 1);
            realignCells(Index, agentCount);
        }


    }

    public void realignCells(int Index, int agentCount)
    {
        int i, j;

        
        for(i = 0; i < agentCount; i++)
        {
            for (j = Index; j < agentCount - 1; j++)
            {
                tableCells[j, i] = tableCells[j + 1, i];
                tableCells[j, i].name = "TableCell_" + j + "_" + i;
            }
            tableCells[j, i] = new GameObject();
        }        
        //tableCells[Index, i] = new GameObject();
        
        for (i = 0; i < agentCount; i++)
        {
            for (j = Index; j < agentCount - 1; j++)
            {
                tableCells[i, j] = tableCells[i, j + 1];
                tableCells[i, j].name = "TableCell_" + i + "_" + j;
            }
            tableCells[i, j] = new GameObject();
        }
        //tableCells[i, Index] = new GameObject();

        alignCells();

    }

    public void addExtra()
    {
        if (childCount != 100)
        {
            int agentCount = (int)Round(Sqrt(childCount)) + 1;
            int oldLastIndex = agentCount - 2;
            int newIndex = agentCount - 1;
            this.childCount = agentCount * agentCount;

            //Destroy the '+' on the 0th horizontal column
            //Replace it with a new 'drag & drop' cell
            Destroy(tableCells[0, oldLastIndex]);
            tableCells[0, oldLastIndex] = Instantiate(dragAndDropCell);
            tableCells[0, oldLastIndex].name = "TableCell_0_" + (oldLastIndex);
            tableCells[0, oldLastIndex].transform.SetParent(payoffMatrixPanel.transform);

            //Destroy the '+' on the 0th vertical column
            //Replace it with a new 'drag & drop' cell
            Destroy(tableCells[oldLastIndex, 0]);
            tableCells[oldLastIndex, 0] = Instantiate(dragAndDropCell);
            tableCells[oldLastIndex, 0].name = "TableCell_" + (oldLastIndex) + "_0";
            tableCells[oldLastIndex, 0].transform.SetParent(payoffMatrixPanel.transform);

            //Create a new '+' cell on the 0th vertical column
            Destroy(tableCells[0, newIndex]);
            tableCells[0, newIndex] = Instantiate(addExtraCell);
            tableCells[0, newIndex].name = "TableCell_0_" + newIndex;
            tableCells[0, newIndex].transform.SetParent(payoffMatrixPanel.transform);

            //Create a new '+' cell on the 0th horizontal column
            Destroy(tableCells[newIndex, 0]);
            tableCells[newIndex, 0] = Instantiate(addExtraCell);
            tableCells[newIndex, 0].name = "TableCell_" + newIndex + "_0";
            tableCells[newIndex, 0].transform.SetParent(payoffMatrixPanel.transform);

            //Add new empty cells beneath the old '+'
            for (int i = 1; i <= oldLastIndex; i++)
            {
                //Debug.Log("Instantiate(emptyCell).transform.Find(\"Formula\").Find(\"Text\").GetComponent<Text>(): " + Instantiate(emptyCell).transform.Find("Formula").Find("Text").GetComponent<Text>());
                //Debug.Log("Instantiate(emptyCell).GetComponentInChildren<TextMesh>().text = \"ASDASFWEADFSDFAS\": " + Instantiate(emptyCell).transform.Find("Formula").Find("Text").GetComponent<TextMesh>().text = "ASDASFWEADFSDFAS");
                Destroy(tableCells[i, oldLastIndex]);
                tableCells[i, oldLastIndex] = Instantiate(emptyCell) as GameObject;
                tableCells[i, oldLastIndex].name = "TableCell_" + i + "_" + oldLastIndex;
                tableCells[i, oldLastIndex].transform.SetParent(payoffMatrixPanel.transform);
                tableCells[i, oldLastIndex].SetActiveRecursively(true);
                Debug.Log(tableCells[i, oldLastIndex].transform.Find("Formula").Find("Text").GetComponent<Text>());

                
                //tableCells[i, oldLastIndex].GetComponentInChildren<TextMesh>().text = "ASDASFWEADFSDFAS";


                /*
                text = tableCells[i, oldLastIndex].transform.Find("AnotherFormulaName").Find("AnotherText").GetComponent<Text>();
                this.text.text = "AnotherText";
                /**/

                Destroy(tableCells[oldLastIndex, i]);
                tableCells[oldLastIndex, i] = Instantiate(emptyCell) as GameObject;
                tableCells[oldLastIndex, i].name = "TableCell_" + oldLastIndex + "_" + i;
                tableCells[oldLastIndex, i].transform.SetParent(payoffMatrixPanel.transform);
                tableCells[oldLastIndex, i].SetActiveRecursively(true);
            }

            //Add new unavaible cells beneath the old '+'
            for (int i = 1; i <= newIndex; i++)
            {
                /*
                Destroy(tableCells[i, newIndex]);
                unavaibleCell.name = "TableCell_" + i + "_" + newIndex;
                payoffMatrixPanel.transform.gameObject.AddChild(unavaibleCell);
                tableCells[i, newIndex] = unavaibleCell;
                Debug.Log(tableCells[i, newIndex].transform.GetChild(0));
                /**/
                
                Destroy(tableCells[i, newIndex]);
                tableCells[i, newIndex] = Instantiate(unavaibleCell) as GameObject;
                tableCells[i, newIndex].name = "TableCell_" + i + "_" + newIndex;
                tableCells[i, newIndex].transform.SetParent(payoffMatrixPanel.transform);
                tableCells[i, newIndex].SetActiveRecursively(true);
                /*
                tableCells[i, newIndex].transform.Find("Formula").Find("Text").GetComponent<Text>().text = "AnotherText";
                tableCells[i, newIndex].transform.Find("Formula").Find("Text").name = "AnotherText";
                /**/

                Destroy(tableCells[newIndex, i]);
                tableCells[newIndex, i] = Instantiate(unavaibleCell) as GameObject;
                tableCells[newIndex, i].name = "TableCell_" + newIndex + "_" + i;
                tableCells[newIndex, i].transform.SetParent(payoffMatrixPanel.transform);
                tableCells[newIndex, i].SetActiveRecursively(true);
                Debug.Log(tableCells[newIndex, i].transform.Find("Formula"));
                /*
                tableCells[newIndex, i].transform.Find("Formula").Find("Text").GetComponent<Text>().text = "AnotherText";
                tableCells[newIndex, i].transform.Find("Formula").Find("Text").name = "AnotherText";
                /**/
            }

            alignCells();
            /**/
        }
    }

    /*
    public void addExtra()
    {
        if (childCount != 100)
        {
            int agentCount = (int)Round(Sqrt(childCount)) + 1;
            int oldLastIndex = agentCount - 2;
            int newIndex = agentCount - 1;
            this.childCount = agentCount * agentCount;

            //Destroy the '+' on the 0th horizontal column
            //Replace it with a new 'drag & drop' cell
            Destroy(tableCells[0, oldLastIndex]);
            tableCells[0, oldLastIndex] = Instantiate(dragAndDropCell);
            tableCells[0, oldLastIndex].name = "TableCell_0_" + (oldLastIndex);
            tableCells[0, oldLastIndex].transform.SetParent(payoffMatrixPanel.transform);

            //Destroy the '+' on the 0th vertical column
            //Replace it with a new 'drag & drop' cell
            Destroy(tableCells[oldLastIndex, 0]);
            tableCells[oldLastIndex, 0] = Instantiate(dragAndDropCell);
            tableCells[oldLastIndex, 0].name = "TableCell_" + (oldLastIndex) + "_0";
            tableCells[oldLastIndex, 0].transform.SetParent(payoffMatrixPanel.transform);

            //Create a new '+' cell on the 0th vertical column
            Destroy(tableCells[0, newIndex]);
            tableCells[0, newIndex] = Instantiate(addExtraCell);
            tableCells[0, newIndex].name = "TableCell_0_" + newIndex;
            tableCells[0, newIndex].transform.SetParent(payoffMatrixPanel.transform);

            //Create a new '+' cell on the 0th horizontal column
            Destroy(tableCells[newIndex, 0]);
            tableCells[newIndex, 0] = Instantiate(addExtraCell);
            tableCells[newIndex, 0].name = "TableCell_" + newIndex + "_0";
            tableCells[newIndex, 0].transform.SetParent(payoffMatrixPanel.transform);

            //Add new empty cells beneath the old '+'
            for (int i = 1; i <= oldLastIndex; i++)
            {
                Destroy(tableCells[i, oldLastIndex]);
                tableCells[i, oldLastIndex] = Instantiate(emptyCell);
                tableCells[i, oldLastIndex].name = "TableCell_" + i + "_" + oldLastIndex;
                tableCells[i, oldLastIndex].transform.SetParent(payoffMatrixPanel.transform);

                Destroy(tableCells[oldLastIndex, i]);
                tableCells[oldLastIndex, i] = Instantiate(emptyCell);
                tableCells[oldLastIndex, i].name = "TableCell_" + oldLastIndex + "_" + i;
                tableCells[oldLastIndex, i].transform.SetParent(payoffMatrixPanel.transform);
            }

            //Add new unavaible cells beneath the old '+'
            for (int i = 1; i <= newIndex; i++)
            {
                Destroy(tableCells[i, newIndex]);
                tableCells[i, newIndex] = Instantiate(unavaibleCell);
                tableCells[i, newIndex].name = "TableCell_" + i + "_" + newIndex;
                tableCells[i, newIndex].transform.SetParent(payoffMatrixPanel.transform);

                Destroy(tableCells[newIndex, i]);
                tableCells[newIndex, i] = Instantiate(unavaibleCell);
                tableCells[newIndex, i].name = "TableCell_" + newIndex + "_" + i;
                tableCells[newIndex, i].transform.SetParent(payoffMatrixPanel.transform);
            }

            alignCells();
        }
    }
    /**/

    public void alignCells()
    {
        RectTransform rectTransform;
        int columnLength = (int)Round(Sqrt(childCount));
        float fullPanelWidth = payoffMatrixPanel.GetComponent<RectTransform>().rect.width;
        float fullPanelHeight = payoffMatrixPanel.GetComponent<RectTransform>().rect.height;
        float squareWidth = fullPanelWidth / columnLength;
        float squareHeight = fullPanelHeight / columnLength;
        float zerothSquarePositionX = fullPanelWidth / (float)(columnLength) / 2f;
        float zerothSquarePositionY = fullPanelHeight / (float)(columnLength) / 2f;
        for (int i = 0; i < columnLength; i++)
        {
            for (int j = 0; j < columnLength; j++)
            {
                rectTransform = tableCells[i, j].GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(squareWidth, squareHeight);
                rectTransform.anchoredPosition = new Vector2((fullPanelWidth) * (((float)j * columnLength) / childCount) + zerothSquarePositionX, (-fullPanelHeight) * (((float)i * columnLength) / childCount) - zerothSquarePositionY);
            }
        }
    }

    void Awake()
    {
        //text.text = "Brand new text";
        instance = this;
        tableCells[0, 0] = payoffMatrixPanel.transform.GetChild(0).gameObject;
        tableCells[0, 1] = payoffMatrixPanel.transform.GetChild(1).gameObject;
        tableCells[0, 2] = payoffMatrixPanel.transform.GetChild(2).gameObject;
        tableCells[1, 0] = payoffMatrixPanel.transform.GetChild(3).gameObject;
        tableCells[1, 1] = payoffMatrixPanel.transform.GetChild(4).gameObject;
        tableCells[1, 2] = payoffMatrixPanel.transform.GetChild(5).gameObject;
        tableCells[2, 0] = payoffMatrixPanel.transform.GetChild(6).gameObject;
        tableCells[2, 1] = payoffMatrixPanel.transform.GetChild(7).gameObject;
        tableCells[2, 2] = payoffMatrixPanel.transform.GetChild(8).gameObject;
        alignCells();
    }
}
