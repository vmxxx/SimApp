using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public class PayoffMatrix_2 : MonoBehaviour
{
    [SerializeField] private Text text;

    public GameObject emptyCellText;

    public static PayoffMatrix_2 instance;

    public GameObject tableColumn;
    public GameObject addExtraCell;
    public GameObject emptyCell;
    public GameObject unavaibleCell;
    public GameObject dragAndDropCell;

    public int childCount = 1;
    public GameObject payoffMatrixPanel;
    public GameObject[,] tableCells = new GameObject[9, 9];
    public GameObject[] tableColumns = new GameObject[9];
    public bool alternateView = false;
    public int agentCount;

    public void setText(string formula, int j, int k)
    {
        tableCells[j, k].transform.Find("Formula").Find("Text").GetComponent<Text>().text = formula;
        tableCells[j, k].transform.Find("Formula").Find("Text").name = formula;
    }

    public void remove(GameObject cell)
    {
        GameObject temp;
        int agentCount = (int)Round(Sqrt(childCount));
        if (cell.name[10] == '0')
        {
            int Index = (int)(cell.name[12] - '0');
            for (int i = 0; i < agentCount; i++) { Destroy(tableCells[i, Index]); }
            for (int i = 0; i < agentCount; i++) { Destroy(tableCells[Index, i]); }
            childCount = (agentCount - 1) * (agentCount - 1);
            realignCells(Index, agentCount);
        }
        else if (cell.name[12] == '0')
        {
            int Index = (int)(cell.name[10] - '0');
            for (int i = 0; i < agentCount; i++) { Destroy(tableCells[i, Index]); }
            for (int i = 0; i < agentCount; i++) { Destroy(tableCells[Index, i]); }
            childCount = (agentCount - 1) * (agentCount - 1);
            realignCells(Index, agentCount);
        }


    }

    public void realignCells(int Index, int agentCount)
    {
        int i, j;


        for (i = 0; i < agentCount; i++)
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

    public void initialize()
    {

        //text.text = "Brand new text";
        //instance = this;
        tableCells[0, 0] = tableColumns[0].transform.GetChild(0).gameObject;
        for (int i = 0; i < Buffer.instance.agents.Length; i++)
        {
            addExtra(i);
        }
        alignCells();
    }

    public void addExtra(int i)
    {
        int newIndex = i + 1;
        agentCount = newIndex;
        if (childCount != 100)
        {
            /*
            int agentCount = (int)Round(Sqrt(childCount)) + 1;
            //int oldLastIndex = agentCount - 2;
            this.childCount = agentCount * agentCount;
            /**/

            /*
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
            /**/

            tableColumns[newIndex] = Instantiate(tableColumn);
            tableColumns[newIndex].name = "TableColumn_" + newIndex;
            tableColumns[newIndex].transform.SetParent(payoffMatrixPanel.transform);
            tableColumns[newIndex].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, -50);

            //Create a new '+' cell on the 0th vertical column
            Destroy(tableCells[0, newIndex]);
            tableCells[0, newIndex] = Instantiate(dragAndDropCell);
            tableCells[0, newIndex].GetComponent<RawImage>().color = Buffer.instance.agents[i].color;
            tableCells[0, newIndex].name = "TableCell_0_" + newIndex;
            tableCells[0, newIndex].transform.SetParent(tableColumns[newIndex].transform);
            tableCells[0, newIndex].transform.Find("AgentID").GetComponent<Text>().text = Buffer.instance.agents[i].agentID.ToString();


            try
            {
                Texture2D newTexture = new Texture2D(1, 1);
                newTexture.LoadImage(Convert.FromBase64String(Buffer.instance.agents[i].icon));
                newTexture.Apply();
                tableCells[0, newIndex].transform.Find("Button").GetChild(1).GetComponent<RawImage>().texture = newTexture;
            }
            catch { }
            tableCells[0, newIndex].transform.Find("Button").GetChild(1).GetChild(0).Find("Name").GetComponent<Text>().text = Buffer.instance.agents[i].agentName;
            tableCells[0, newIndex].transform.Find("Button").GetChild(1).GetChild(0).Find("Description").GetComponent<Text>().text = Buffer.instance.agents[i].agentDescription;
            tableCells[0, newIndex].transform.Find("Button").GetChild(0).GetComponent<Text>().color = Buffer.instance.agents[i].color;

            //Create a new '+' cell on the 0th horizontal column
            Destroy(tableCells[newIndex, 0]);
            tableCells[newIndex, 0] = Instantiate(dragAndDropCell);
            tableCells[newIndex, 0].GetComponent<RawImage>().color = Buffer.instance.agents[i].color;
            tableCells[newIndex, 0].name = "TableCell_" + newIndex + "_0";
            tableCells[newIndex, 0].transform.SetParent(tableColumns[0].transform);
            tableCells[newIndex, 0].transform.Find("AgentID").GetComponent<Text>().text = Buffer.instance.agents[i].agentID.ToString();
            try
            {
                Texture2D newTexture = new Texture2D(1, 1);
                newTexture.LoadImage(Convert.FromBase64String(Buffer.instance.agents[i].icon));
                newTexture.Apply();
                tableCells[newIndex, 0].transform.Find("Button").GetChild(1).GetComponent<RawImage>().texture = newTexture;
            }
            catch { }
            tableCells[newIndex, 0].transform.Find("Button").GetChild(1).GetChild(0).Find("Name").GetComponent<Text>().text = Buffer.instance.agents[i].agentName;
            tableCells[newIndex, 0].transform.Find("Button").GetChild(1).GetChild(0).Find("Description").GetComponent<Text>().text = Buffer.instance.agents[i].agentDescription;
            tableCells[newIndex, 0].transform.Find("Button").GetChild(0).GetComponent<Text>().color = Buffer.instance.agents[i].color;

            (int, int) formulaID;

            //Add new empty cells beneath the old '+'
            for (int j = 1; j < newIndex; j++)
            {
                formulaID = (Buffer.instance.agents[j - 1].agentID, Buffer.instance.agents[i].agentID);

                tableCells[j, newIndex] = Instantiate(emptyCell) as GameObject;
                tableCells[j, newIndex].name = "TableCell_" + j + "_" + newIndex;
                tableCells[j, newIndex].transform.SetParent(tableColumns[newIndex].transform);
                tableCells[j, newIndex].transform.Find("Formula").GetComponent<InputField>().text = Buffer.instance.payoffFormulas[formulaID].payoffFormula;
                tableCells[j, newIndex].SetActiveRecursively(true);


                //tableCells[i, oldLastIndex].GetComponentInChildren<TextMesh>().text = "ASDASFWEADFSDFAS";


                /*
                text = tableCells[i, oldLastIndex].transform.Find("AnotherFormulaName").Find("AnotherText").GetComponent<Text>();
                this.text.text = "AnotherText";
                /**/

                formulaID = (Buffer.instance.agents[i].agentID, Buffer.instance.agents[j - 1].agentID);

                tableCells[newIndex, j] = Instantiate(emptyCell) as GameObject;
                tableCells[newIndex, j].name = "TableCell_" + newIndex + "_" + j;
                tableCells[newIndex, j].transform.SetParent(tableColumns[j].transform);
                tableCells[newIndex, j].transform.Find("Formula").GetComponent<InputField>().text = Buffer.instance.payoffFormulas[formulaID].payoffFormula;
                tableCells[newIndex, j].SetActiveRecursively(true);
            }


            formulaID = (Buffer.instance.agents[i].agentID, Buffer.instance.agents[i].agentID);

            tableCells[newIndex, newIndex] = Instantiate(emptyCell) as GameObject;
            tableCells[newIndex, newIndex].name = "TableCell_" + newIndex + "_" + newIndex;
            tableCells[newIndex, newIndex].transform.SetParent(tableColumns[newIndex].transform);
            tableCells[newIndex, newIndex].transform.Find("Formula").GetComponent<InputField>().text = Buffer.instance.payoffFormulas[formulaID].payoffFormula;
            tableCells[newIndex, newIndex].SetActiveRecursively(true);
        }
    }


    public GameObject canvas;
    RectTransform canvasRectTransform;

    private float[] prevResolution;
    private float[] currResolution;

    public void Awake()
    {
        instance = this;

    }

    public void Start()
    {
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        prevResolution = new float[] { canvasRectTransform.rect.width, canvasRectTransform.rect.height };
    }

    public void Update()
    {

        currResolution = new float[2] { canvasRectTransform.rect.width, canvasRectTransform.rect.height };

        if (prevResolution[0] != currResolution[0] || prevResolution[1] != currResolution[1])
        {
            Debug.Log("new resolution2");
            prevResolution = new float[2] { currResolution[0], currResolution[1] };
            alignCells(true);
        }
    }

    public void alignCells(bool onCollapse = true, int columnIndex = 0, float offset = 0f)
    {
        RectTransform rectTransform;
        int columnLength = (int)Round(Sqrt(childCount));
        float fullPanelWidth = payoffMatrixPanel.GetComponent<RectTransform>().rect.width;
        float fullPanelHeight = payoffMatrixPanel.GetComponent<RectTransform>().rect.height;
        float squareWidth = fullPanelWidth / columnLength;
        float squareHeight = fullPanelHeight / columnLength;
        float zerothSquarePositionX = fullPanelWidth / (float)(columnLength) / 2f;
        float zerothSquarePositionY = fullPanelHeight / (float)(columnLength) / 2f;

        lastInvisibleColumn.transform.SetSiblingIndex(columnLength);

        if (squareWidth < 140f) alternateView = true;
        else alternateView = false;

        if (alternateView == false)
        {

            for (int j = 0; j < columnLength; j++) tableColumns[0].transform.GetChild(j).gameObject.SetActive(true);

            for (int i = 0; i < columnLength; i++)
            {
                tableColumns[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(50f, -45f);
                for (int j = 0; j < columnLength; j++)
                {



                    if (j != 0)
                    {
                        tableColumns[i].transform.GetChild(j).gameObject.SetActive(true);
                    }
                    else
                    {
                        GameObject showDetails = tableColumns[i].transform.GetChild(j).GetChild(0).gameObject;
                        showDetails.SetActive(false);

                    }



                    rectTransform = tableCells[i, j].GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(squareWidth, squareHeight);
                    rectTransform.anchoredPosition = new Vector2((fullPanelWidth) * (((float)j * columnLength) / childCount) + zerothSquarePositionX, (-fullPanelHeight) * (((float)i * columnLength) / childCount) - zerothSquarePositionY);
                }
            }
        }
        else if(onCollapse == true)
        {
            for (int i = 0; i < columnLength; i++)
            {
                for (int j = 0; j < columnLength; j++)
                {
                    rectTransform = tableCells[i, j].GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(squareWidth, squareHeight);
                    rectTransform.anchoredPosition = new Vector2((fullPanelWidth) * (((float)j * columnLength) / childCount) + zerothSquarePositionX, (-fullPanelHeight) * (((float)i * columnLength) / childCount) - zerothSquarePositionY);
                }
            }

            /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/


            for (int j = 0; j < columnLength; j++) tableColumns[0].transform.GetChild(j).gameObject.SetActive(false);

            for (int i = 1; i < columnLength; i++)
            {
                for (int j = 0; j < columnLength; j++)
                {
                    if (j != 0)
                    {
                        tableColumns[i].transform.GetChild(j).gameObject.SetActive(false);
                    }
                    else
                    {
                        GameObject showDetails = tableColumns[i].transform.GetChild(j).GetChild(0).gameObject;
                        showDetails.SetActive(true);
                        showDetails.GetComponent<RectTransform>().sizeDelta = new Vector2(fullPanelWidth * 0.30f, 90f);
                        showDetails.GetComponent<RectTransform>().anchoredPosition = new Vector2(-fullPanelWidth * 0.15f, (-45f));

                    }


                    tableColumns[i].transform.GetChild(j).GetComponent<RectTransform>().sizeDelta = new Vector2(fullPanelWidth * 0.70f, 90f);
                    tableColumns[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, (45f - (i * 90f)));
                    tableColumns[i].transform.GetChild(j).GetComponent<RectTransform>().anchoredPosition = new Vector2(fullPanelWidth * 0.65f, (-45f - (j * 90f)));
                  
                }
            }
            lastInvisibleColumn.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, (45f - (columnLength * 90f)));
        }
        else
        {

            /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
            float X;
            float Y;
            for (int i = 1; i < columnLength; i++)
            {
                Debug.Log("i: " + i + ", columnIndex: " + columnIndex);
                X = tableColumns[i].transform.GetComponent<RectTransform>().anchoredPosition.x;
                Y = tableColumns[i].transform.GetComponent<RectTransform>().anchoredPosition.y;
                if (i > columnIndex) tableColumns[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, (Y - offset));

            }

            X = lastInvisibleColumn.transform.GetComponent<RectTransform>().anchoredPosition.x;
            Y = lastInvisibleColumn.transform.GetComponent<RectTransform>().anchoredPosition.y;
            lastInvisibleColumn.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, (Y - offset));
        }
    }

    public GameObject lastInvisibleColumn;

    public void showDetails(GameObject hideButton)
    {
        GameObject showButton = EventSystem.current.currentSelectedGameObject;
        Transform appropriateTableColumn = showButton.transform.parent.parent.parent;
        int columnIndex = appropriateTableColumn.GetSiblingIndex();
        float offset = 0f;

        hideButton.SetActive(true);
        showButton.SetActive(false);

        for (int i = 1; i < appropriateTableColumn.childCount; i++)
        {
            appropriateTableColumn.GetChild(i).gameObject.SetActive(true);
            offset = offset + 90f;
        }
        alignCells(false, columnIndex, offset);
    }
    public void hideDetails(GameObject showButton)
    {
        GameObject hideButton = EventSystem.current.currentSelectedGameObject;
        Transform appropriateTableColumn = hideButton.transform.parent.parent.parent;
        int columnIndex = appropriateTableColumn.GetSiblingIndex();
        float offset = 0f;

        showButton.SetActive(true);
        hideButton.SetActive(false);

        for (int i = 1; i < appropriateTableColumn.childCount; i++)
        {
            appropriateTableColumn.GetChild(i).gameObject.SetActive(false);
            offset = offset - 90f;
        }
        alignCells(false, columnIndex, offset);
    }
}

