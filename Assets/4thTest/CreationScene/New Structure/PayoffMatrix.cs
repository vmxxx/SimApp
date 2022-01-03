using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;
using UnityEngine.UI;
using UnityEngine.EventSystems;


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
    public GameObject[,] tableCells = new GameObject[9, 9];
    public GameObject[] tableColumns = new GameObject[9];
    public GameObject canvas;
    private RectTransform canvasRectTransform;

    private float[] prevResolution;
    private float[] currResolution;

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
            for (int i = 0; i < agentCount; i++) { Debug.Log("DESTROYED: XIndex: " + Index + " i: " + i + tableCells[i, Index].name); Destroy(tableCells[i, Index]); }
            for (int i = 0; i < agentCount; i++) { Debug.Log("DESTROYED: i: " + i + " YIndex: " + Index + tableCells[Index, i].name); Destroy(tableCells[Index, i]); }
            Destroy(tableColumns[Index]);
            childCount = (agentCount - 1) * (agentCount - 1);
            realignCells(Index, agentCount);
        }
        else if (cell.name[12] == '0')
        {
            int Index = (int)(cell.name[10] - '0'); 
            for (int i = 0; i < agentCount; i++) { Debug.Log("XIndex: " + Index + " i: " + i); Destroy(tableCells[i, Index]); }
            for (int i = 0; i < agentCount; i++) { Debug.Log("i: " + i + " YIndex: " + Index); Destroy(tableCells[Index, i]); }
            Destroy(tableColumns[Index]);
            childCount = (agentCount - 1) * (agentCount - 1);
            realignCells(Index, agentCount);
        }
    }

    public void realignCells(int Index, int agentCount)
    {
        int i = 0, j = 0;

        for (i = Index; i < agentCount - 1; i++)
        {
            tableColumns[i] = tableColumns[i + 1];
            tableColumns[i].name = "TableColumn_" + i;
        }
        Debug.Log(tableColumns[i].name);
        //tableColumns[i] = new GameObject();

        for (i = 0; i < agentCount; i++)
        {
            for (j = Index; j < agentCount - 1; j++)
            {
                //Debug.Log(tableCells[i, j].name);
                //Debug.Log(tableCells[i, j + 1].name);
                tableCells[i, j] = tableCells[i, j + 1];
                tableCells[i, j].name = "TableCell_" + i + "_" + j;
                tableCells[i, j].transform.SetParent(tableColumns[j].transform);

                /**/
            }
        }
        //Debug.Log(tableCells[i - 1, j]);
        //tableCells[i, j] = new GameObject();
        
        Debug.Log("agentCount - 1: " + (agentCount - 1));
        for (i = 0; i < agentCount - 1; i++)
        {
            Debug.Log("i: " + i);
            for (j = Index; j < agentCount - 1; j++)
            {
                //Debug.Log(tableCells[j, i].name);
                Debug.Log("TableCell_" + j + "_" + i);
                Debug.Log(tableCells[j + 1, i].name);
                tableCells[j, i] = tableCells[j + 1, i];
                tableCells[j, i].name = "TableCell_" + j + "_" + i;
                Debug.Log(tableCells[j, i].name);
                tableCells[j, i].transform.SetParent(tableColumns[i].transform);

            }
        }
        tableColumns[i] = new GameObject();

        /*
        Debug.Log("AI: " + i);


        Debug.Log("TABLE COLUMNS:");
        for (i = 0; i < 9; i++)
        {
            if (tableColumns[i] != null) Debug.Log(tableColumns[i].name);
        }
        Debug.Log("TABLE CELLS:");
        for (i = 0; i < 9; i++)
        {
            string nineCells = "";
            for (j = 0; j < 9; j++)
            {
                if(tableCells[i, j] != null) nineCells = nineCells + tableCells[i, j].name + " / ";
            }
            Debug.Log(nineCells);
        }
        /**/


        alignCells(true);

    }

    public GameObject tableColumn;

    public void addExtra()
    {

        //int newIndex = i + 1;
        //int agentCount = newIndex;
        if (childCount != 81)
        {
            int agentCount = (int)Round(Sqrt(childCount)) + 1;
            int oldLastIndex = agentCount - 2;
            Debug.Log("oldLastIndex: " + oldLastIndex);
            int newIndex = agentCount - 1;
            this.childCount = agentCount * agentCount;
            /*
            int agentCount = (int)Round(Sqrt(childCount)) + 1;
            //int oldLastIndex = agentCount - 2;
            this.childCount = agentCount * agentCount;
            /**/



            //Destroy the '+' on the 0th horizontal column
            //Replace it with a new 'drag & drop' cell
            Destroy(tableCells[0, oldLastIndex]);
            tableCells[0, oldLastIndex] = Instantiate(dragAndDropCell);
            tableCells[0, oldLastIndex].name = "TableCell_0_" + (oldLastIndex);
            tableCells[0, oldLastIndex].transform.SetParent(tableColumns[oldLastIndex].transform);

            //Destroy the '+' on the 0th vertical column
            //Replace it with a new 'drag & drop' cell
            Destroy(tableCells[oldLastIndex, 0]);
            tableCells[oldLastIndex, 0] = Instantiate(dragAndDropCell);
            tableCells[oldLastIndex, 0].name = "TableCell_" + (oldLastIndex) + "_0";
            tableCells[oldLastIndex, 0].transform.SetParent(tableColumns[0].transform);
            /**/

            tableColumns[newIndex] = Instantiate(tableColumn);
            tableColumns[newIndex].name = "TableColumn_" + newIndex;
            tableColumns[newIndex].transform.SetParent(payoffMatrixPanel.transform);
            tableColumns[newIndex].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, -50);

            //Create a new '+' cell on the 0th vertical column
            Destroy(tableCells[0, newIndex]);
            tableCells[0, newIndex] = Instantiate(addExtraCell);
            tableCells[0, newIndex].name = "TableCell_0_" + newIndex;
            tableCells[0, newIndex].transform.SetParent(tableColumns[newIndex].transform);
            //tableCells[0, newIndex].transform.Find("AgentID").GetComponent<Text>().text = Buffer.instance.agents[i].agentID.ToString();
            //tableCells[0, newIndex].transform.Find("Button").GetChild(0).GetComponent<Text>().text = Buffer.instance.agents[i].agentName;
            //tableCells[0, newIndex].transform.Find("Button").GetChild(0).GetComponent<Text>().color = Buffer.instance.agents[i].color;

            //Create a new '+' cell on the 0th horizontal column
            Destroy(tableCells[newIndex, 0]);
            tableCells[newIndex, 0] = Instantiate(addExtraCell);
            tableCells[newIndex, 0].name = "TableCell_" + newIndex + "_0";
            tableCells[newIndex, 0].transform.SetParent(tableColumns[0].transform);
            //tableCells[newIndex, 0].transform.Find("AgentID").GetComponent<Text>().text = Buffer.instance.agents[i].agentID.ToString();
            //tableCells[newIndex, 0].transform.Find("Button").GetChild(0).GetComponent<Text>().text = Buffer.instance.agents[i].agentName;
            //tableCells[newIndex, 0].transform.Find("Button").GetChild(0).GetComponent<Text>().color = Buffer.instance.agents[i].color;

            (int, int) formulaID;



            //Add new empty cells beneath the old '+'
            for (int i = 1; i <= oldLastIndex; i++)
            {
                //Debug.Log("Instantiate(emptyCell).transform.Find(\"Formula\").Find(\"Text\").GetComponent<Text>(): " + Instantiate(emptyCell).transform.Find("Formula").Find("Text").GetComponent<Text>());
                //Debug.Log("Instantiate(emptyCell).GetComponentInChildren<TextMesh>().text = \"ASDASFWEADFSDFAS\": " + Instantiate(emptyCell).transform.Find("Formula").Find("Text").GetComponent<TextMesh>().text = "ASDASFWEADFSDFAS");
                Destroy(tableCells[i, oldLastIndex]);
                tableCells[i, oldLastIndex] = Instantiate(emptyCell) as GameObject;
                tableCells[i, oldLastIndex].name = "TableCell_" + i + "_" + oldLastIndex;
                tableCells[i, oldLastIndex].transform.SetParent(tableColumns[oldLastIndex].transform);
                tableCells[i, oldLastIndex].SetActiveRecursively(true);
                Debug.Log(tableCells[i, oldLastIndex].transform.Find("Formula").Find("Text").GetComponent<Text>());


                //tableCells[i, oldLastIndex].GetComponentInChildren<TextMesh>().text = "ASDASFWEADFSDFAS";



                Destroy(tableCells[oldLastIndex, i]);
                tableCells[oldLastIndex, i] = Instantiate(emptyCell) as GameObject;
                tableCells[oldLastIndex, i].name = "TableCell_" + oldLastIndex + "_" + i;
                tableCells[oldLastIndex, i].transform.SetParent(tableColumns[i].transform);
                tableCells[oldLastIndex, i].SetActiveRecursively(true);
            }


            //formulaID = (Buffer.instance.agents[i].agentID, Buffer.instance.agents[i].agentID);

            tableCells[newIndex, newIndex] = Instantiate(emptyCell) as GameObject;
            tableCells[newIndex, newIndex].name = "TableCell_" + newIndex + "_" + newIndex;
            tableCells[newIndex, newIndex].transform.SetParent(tableColumns[newIndex].transform);
            //tableCells[newIndex, newIndex].transform.Find("Formula").GetComponent<InputField>().text = Buffer.instance.payoffFormulas[formulaID].payoffFormula;
            tableCells[newIndex, newIndex].SetActiveRecursively(true);

            
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
                tableCells[i, newIndex].transform.SetParent(tableColumns[newIndex].transform);
                tableCells[i, newIndex].SetActiveRecursively(true);
                /*
                tableCells[i, newIndex].transform.Find("Formula").Find("Text").GetComponent<Text>().text = "AnotherText";
                tableCells[i, newIndex].transform.Find("Formula").Find("Text").name = "AnotherText";
                /**/
                Destroy(tableCells[newIndex, i]);
                tableCells[newIndex, i] = Instantiate(unavaibleCell) as GameObject;
                tableCells[newIndex, i].name = "TableCell_" + newIndex + "_" + i;
                tableCells[newIndex, i].transform.SetParent(tableColumns[i].transform);
                tableCells[newIndex, i].SetActiveRecursively(true);
                Debug.Log(tableCells[newIndex, i].transform.Find("Formula"));
                /*
                tableCells[newIndex, i].transform.Find("Formula").Find("Text").GetComponent<Text>().text = "AnotherText";
                tableCells[newIndex, i].transform.Find("Formula").Find("Text").name = "AnotherText";
                /**/
            }
        }
        alignCells(true);
    }


    public void alignCells(bool onCollapse = true, int columnIndex = 0, float offset = 0f)
    {
        bool alternateView;

        RectTransform rectTransform;
        int columnLength = (int)Round(Sqrt(childCount));
        float fullPanelWidth = payoffMatrixPanel.GetComponent<RectTransform>().rect.width;
        float fullPanelHeight = payoffMatrixPanel.GetComponent<RectTransform>().rect.height;
        float squareWidth = fullPanelWidth / columnLength;
        float squareHeight = fullPanelHeight / columnLength;
        float zerothSquarePositionX = fullPanelWidth / (float)(columnLength) / 2f;
        float zerothSquarePositionY = fullPanelHeight / (float)(columnLength) / 2f;

        if (squareWidth < 140f) alternateView = true;
        else alternateView = false;


        if (alternateView == false)
        {
            tableColumns[0].SetActive(true);

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
                        if (showDetails.name == "ShowDetails") showDetails.SetActive(false);
                    }


                    tableCells[i, j].SetActive(true);
                    rectTransform = tableCells[i, j].GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(squareWidth, squareHeight);
                    rectTransform.anchoredPosition = new Vector2((fullPanelWidth) * (((float)j * columnLength) / childCount) + zerothSquarePositionX, (-fullPanelHeight) * (((float)i * columnLength) / childCount) - zerothSquarePositionY);
                }
            }
        }
        else if (onCollapse == true)
        {
            tableColumns[0].SetActive(false);

            for (int i = 1; i < columnLength; i++)
            {
                for (int j = 0; j < columnLength; j++)
                {
                    if (j != 0)
                    {
                        tableCells[j, i].SetActive(false);
                    }
                    else
                    {
                        GameObject showDetails = tableCells[j, i].transform.GetChild(0).gameObject;
                        showDetails.SetActive(true);
                        showDetails.GetComponent<RectTransform>().sizeDelta = new Vector2(fullPanelWidth * 0.30f, 90f);
                        showDetails.GetComponent<RectTransform>().anchoredPosition = new Vector2(-fullPanelWidth * 0.15f, (-45f));
                    }


                    Debug.Log(tableCells[j, i]);
                    tableCells[j, i].GetComponent<RectTransform>().sizeDelta = new Vector2(fullPanelWidth * 0.70f, 90f);
                    tableColumns[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, (45f - (i * 90f)));
                    tableCells[j, i].GetComponent<RectTransform>().anchoredPosition = new Vector2(fullPanelWidth * 0.65f, (-45f - (j * 90f)));
                }
            }
        }
        else
        {
            
            for (int i = 1; i < columnLength; i++)
            {

                float X = tableColumns[i].transform.GetComponent<RectTransform>().anchoredPosition.x;
                float Y = tableColumns[i].transform.GetComponent<RectTransform>().anchoredPosition.y;
                if (i > columnIndex) tableColumns[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(X, (Y - offset));
            }
        }
        RectTransform addButton = tableColumns[columnLength - 1].transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        addButton.offsetMin = new Vector2(10, 10);
        addButton.offsetMax = new Vector2(-10, -10);

        Debug.Log("ALIGN CELSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
    }

    public void resetHideShowButtons()
    {

    }

    public void showDetails(GameObject hideButton)
    {
        GameObject showButton = EventSystem.current.currentSelectedGameObject;
        Transform appropriateTableColumn = showButton.transform.parent.parent.parent;
        int columnIndex = appropriateTableColumn.GetSiblingIndex();
        float offset = 0f;

        hideButton.SetActive(true);
        showButton.SetActive(false);

        for (int i = 1; i < appropriateTableColumn.childCount - 1; i++)
        {
            appropriateTableColumn.GetChild(i).gameObject.SetActive(true);
            offset = offset + 90f;
        }
        Debug.Log("offset: " + offset);
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

        for (int i = 1; i < appropriateTableColumn.childCount - 1; i++)
        {
            appropriateTableColumn.GetChild(i).gameObject.SetActive(false);
            offset = offset - 90f;
        }
        Debug.Log("offset: " + offset);
        alignCells(false, columnIndex, offset);
    }

    /*
    public void Start()
    {
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        prevResolution = new float[] { canvasRectTransform.rect.width, canvasRectTransform.rect.height };
    }
    /**/

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

    void Start()
    {
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        prevResolution = new float[] { canvasRectTransform.rect.width, canvasRectTransform.rect.height };

        //text.text = "Brand new text";
        instance = this;
        tableCells[0, 0] = payoffMatrixPanel.transform.Find("TableColumn_0").Find("TableCell_0_0").gameObject;
        tableCells[1, 0] = payoffMatrixPanel.transform.Find("TableColumn_0").Find("TableCell_1_0").gameObject;
        tableCells[2, 0] = payoffMatrixPanel.transform.Find("TableColumn_0").Find("TableCell_2_0").gameObject;

        tableCells[0, 1] = payoffMatrixPanel.transform.Find("TableColumn_1").Find("TableCell_0_1").gameObject;
        tableCells[1, 1] = payoffMatrixPanel.transform.Find("TableColumn_1").Find("TableCell_1_1").gameObject;
        tableCells[2, 1] = payoffMatrixPanel.transform.Find("TableColumn_1").Find("TableCell_2_1").gameObject;

        tableCells[0, 2] = payoffMatrixPanel.transform.Find("TableColumn_2").Find("TableCell_0_2").gameObject;
        tableCells[1, 2] = payoffMatrixPanel.transform.Find("TableColumn_2").Find("TableCell_1_2").gameObject;
        tableCells[2, 2] = payoffMatrixPanel.transform.Find("TableColumn_2").Find("TableCell_2_2").gameObject;
        /**/

        /*
        tableCells[0, 0] = payoffMatrixPanel.transform.GetChild(0).gameObject;
        tableCells[0, 1] = payoffMatrixPanel.transform.GetChild(1).gameObject;
        tableCells[0, 2] = payoffMatrixPanel.transform.GetChild(2).gameObject;

        /*
        tableCells[1, 0] = payoffMatrixPanel.transform.GetChild(3).gameObject;
        tableCells[1, 1] = payoffMatrixPanel.transform.GetChild(4).gameObject;
        tableCells[1, 2] = payoffMatrixPanel.transform.GetChild(5).gameObject;

        tableCells[2, 0] = payoffMatrixPanel.transform.GetChild(6).gameObject;
        tableCells[2, 1] = payoffMatrixPanel.transform.GetChild(7).gameObject;
        tableCells[2, 2] = payoffMatrixPanel.transform.GetChild(8).gameObject;
        /**/

        Debug.Log("tableCells nameSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS:");
        Debug.Log(tableCells[0, 0].name);
        Debug.Log(tableCells[0, 1].name);
        Debug.Log(tableCells[0, 2].name);
        Debug.Log(tableCells[1, 0].name);
        Debug.Log(tableCells[1, 1].name);
        Debug.Log(tableCells[1, 2].name);
        Debug.Log(tableCells[2, 0].name);
        Debug.Log(tableCells[2, 1].name);
        Debug.Log(tableCells[2, 2].name);
        alignCells(true);
    }
}
