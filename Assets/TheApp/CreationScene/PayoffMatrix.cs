using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class Row
{
    public GameObject[] cell = new GameObject[9];
}

[System.Serializable]
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
    public Row[] tableCells = new Row[9];
    public GameObject[] tableColumns = new GameObject[9];
    public GameObject canvas;
    private RectTransform canvasRectTransform;

    private float[] prevResolution;
    private float[] currResolution;

    public void setText(string formula, int j, int k)
    {
        tableCells[j].cell[k].transform.Find("Formula").Find("Text").GetComponent<Text>().text = formula;
        tableCells[j].cell[k].transform.Find("Formula").Find("Text").name = formula;
    }

    public void remove(GameObject cell)
    {
        GameObject temp;
        int agentCount = (int)Round(Sqrt(childCount));
        if (cell.name[10] == '0')
        {
            int Index = (int)(cell.name[12] - '0');
            for (int i = 0; i < agentCount; i++) { Destroy(tableCells[i].cell[Index]); }
            for (int i = 0; i < agentCount; i++) { Destroy(tableCells[Index].cell[i]); }
            Destroy(tableColumns[Index]);
            childCount = (agentCount - 1) * (agentCount - 1);
            realignCells(Index, agentCount);
        }
        else if (cell.name[12] == '0')
        {
            int Index = (int)(cell.name[10] - '0');
            for (int i = 0; i < agentCount; i++) { Destroy(tableCells[i].cell[Index]); }
            for (int i = 0; i < agentCount; i++) { Destroy(tableCells[Index].cell[i]); }
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

        for (i = 0; i < agentCount; i++)
        {
            for (j = Index; j < agentCount - 1; j++)
            {
                tableCells[i].cell[j] = tableCells[i].cell[j + 1];
                tableCells[i].cell[j].name = "TableCell_" + i + "_" + j;
                tableCells[i].cell[j].transform.SetParent(tableColumns[j].transform);
            }
        }
        for (i = 0; i < agentCount - 1; i++)
        {
            for (j = Index; j < agentCount - 1; j++)
            {
                tableCells[j].cell[i] = tableCells[j + 1].cell[i];
                tableCells[j].cell[i].name = "TableCell_" + j + "_" + i;
                tableCells[j].cell[i].transform.SetParent(tableColumns[i].transform);

            }
        }
        tableColumns[i] = new GameObject();


        alignCells(true);

    }

    public GameObject tableColumn;

    public void addExtra()
    {
        if (childCount != 81)
        {
            int agentCount = (int)Round(Sqrt(childCount)) + 1;
            int oldLastIndex = agentCount - 2;
            int newIndex = agentCount - 1;
            this.childCount = agentCount * agentCount;



            //Destroy the '+' on the 0th horizontal column
            //Replace it with a new 'drag & drop' cell
            Destroy(tableCells[0].cell[oldLastIndex]);
            tableCells[0].cell[oldLastIndex] = Instantiate(dragAndDropCell);
            tableCells[0].cell[oldLastIndex].name = "TableCell_0_" + (oldLastIndex);
            tableCells[0].cell[oldLastIndex].transform.SetParent(tableColumns[oldLastIndex].transform);

            //Destroy the '+' on the 0th vertical column
            //Replace it with a new 'drag & drop' cell
            Destroy(tableCells[oldLastIndex].cell[0]);
            tableCells[oldLastIndex].cell[0] = Instantiate(dragAndDropCell);
            tableCells[oldLastIndex].cell[0].name = "TableCell_" + (oldLastIndex) + "_0";
            tableCells[oldLastIndex].cell[0].transform.SetParent(tableColumns[0].transform);

            tableColumns[newIndex] = Instantiate(tableColumn);
            tableColumns[newIndex].name = "TableColumn_" + newIndex;
            tableColumns[newIndex].transform.SetParent(payoffMatrixPanel.transform);
            tableColumns[newIndex].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, -50);

            //Create a new '+' cell on the 0th vertical column
            Destroy(tableCells[0].cell[newIndex]);
            tableCells[0].cell[newIndex] = Instantiate(addExtraCell);
            tableCells[0].cell[newIndex].name = "TableCell_0_" + newIndex;
            tableCells[0].cell[newIndex].transform.SetParent(tableColumns[newIndex].transform);

            //Create a new '+' cell on the 0th horizontal column
            Destroy(tableCells[newIndex].cell[0]);
            tableCells[newIndex].cell[0] = Instantiate(addExtraCell);
            tableCells[newIndex].cell[0].name = "TableCell_" + newIndex + "_0";
            tableCells[newIndex].cell[0].transform.SetParent(tableColumns[0].transform);

            (int, int) formulaID;



            //Add new empty cells beneath the old '+'
            for (int i = 1; i <= oldLastIndex; i++)
            {
                Destroy(tableCells[i].cell[oldLastIndex]);
                tableCells[i].cell[oldLastIndex] = Instantiate(emptyCell) as GameObject;
                tableCells[i].cell[oldLastIndex].name = "TableCell_" + i + "_" + oldLastIndex;
                tableCells[i].cell[oldLastIndex].transform.SetParent(tableColumns[oldLastIndex].transform);
                tableCells[i].cell[oldLastIndex].SetActiveRecursively(true);

                Destroy(tableCells[oldLastIndex].cell[i]);
                tableCells[oldLastIndex].cell[i] = Instantiate(emptyCell) as GameObject;
                tableCells[oldLastIndex].cell[i].name = "TableCell_" + oldLastIndex + "_" + i;
                tableCells[oldLastIndex].cell[i].transform.SetParent(tableColumns[i].transform);
                tableCells[oldLastIndex].cell[i].SetActiveRecursively(true);
            }

            tableCells[newIndex].cell[newIndex] = Instantiate(emptyCell) as GameObject;
            tableCells[newIndex].cell[newIndex].name = "TableCell_" + newIndex + "_" + newIndex;
            tableCells[newIndex].cell[newIndex].transform.SetParent(tableColumns[newIndex].transform);
            tableCells[newIndex].cell[newIndex].SetActiveRecursively(true);


            //Add new unavaible cells beneath the old '+'
            for (int i = 1; i <= newIndex; i++)
            {

                Destroy(tableCells[i].cell[newIndex]);
                tableCells[i].cell[newIndex] = Instantiate(unavaibleCell) as GameObject;
                tableCells[i].cell[newIndex].name = "TableCell_" + i + "_" + newIndex;
                tableCells[i].cell[newIndex].transform.SetParent(tableColumns[newIndex].transform);
                tableCells[i].cell[newIndex].SetActiveRecursively(true);

                Destroy(tableCells[newIndex].cell[i]);
                tableCells[newIndex].cell[i] = Instantiate(unavaibleCell) as GameObject;
                tableCells[newIndex].cell[i].name = "TableCell_" + newIndex + "_" + i;
                tableCells[newIndex].cell[i].transform.SetParent(tableColumns[i].transform);
                tableCells[newIndex].cell[i].SetActiveRecursively(true);
            }
        }
        alignCells(true);
    }

    public GameObject payoffMatrix;


    public void alignCells(bool onCollapse = true, int columnIndex = 0, float offset = 0f)
    {
        bool alternateView;

        RectTransform rectTransform;
        int columnLength = (int)Round(Sqrt(childCount));
        float fullPanelWidth = payoffMatrix.GetComponent<RectTransform>().rect.width;
        float fullPanelHeight = payoffMatrix.GetComponent<RectTransform>().rect.height;
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


                    tableCells[i].cell[j].SetActive(true);
                    rectTransform = tableCells[i].cell[j].GetComponent<RectTransform>();
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
                        tableCells[j].cell[i].SetActive(false);
                    }
                    else
                    {
                        GameObject showDetails = tableCells[j].cell[i].transform.GetChild(0).gameObject;
                        showDetails.SetActive(true);
                        showDetails.GetComponent<RectTransform>().sizeDelta = new Vector2(fullPanelWidth * 0.30f, 90f);
                        showDetails.GetComponent<RectTransform>().anchoredPosition = new Vector2(-fullPanelWidth * 0.15f, (-45f));
                    }

                    tableCells[j].cell[i].GetComponent<RectTransform>().sizeDelta = new Vector2(fullPanelWidth * 0.70f, 90f);
                    tableColumns[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, (45f - (i * 90f)));
                    tableCells[j].cell[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(fullPanelWidth * 0.65f, (-45f - (j * 90f)));
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
        alignCells(false, columnIndex, offset);
    }

    public void Update()
    {

        if (PayoffMatrix.instance == null)
        {
            instance = this;
        }

        currResolution = new float[2] { canvasRectTransform.rect.width, canvasRectTransform.rect.height };

        if (prevResolution[0] != currResolution[0] || prevResolution[1] != currResolution[1])
        {
            prevResolution = new float[2] { currResolution[0], currResolution[1] };
            alignCells(true);
        }
    }

    void Start()
    {
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        prevResolution = new float[] { canvasRectTransform.rect.width, canvasRectTransform.rect.height };

        instance = this;
        tableCells[0].cell[0] = payoffMatrixPanel.transform.Find("TableColumn_0").Find("TableCell_0_0").gameObject;
        tableCells[1].cell[0] = payoffMatrixPanel.transform.Find("TableColumn_0").Find("TableCell_1_0").gameObject;
        tableCells[2].cell[0] = payoffMatrixPanel.transform.Find("TableColumn_0").Find("TableCell_2_0").gameObject;

        tableCells[0].cell[1] = payoffMatrixPanel.transform.Find("TableColumn_1").Find("TableCell_0_1").gameObject;
        tableCells[1].cell[1] = payoffMatrixPanel.transform.Find("TableColumn_1").Find("TableCell_1_1").gameObject;
        tableCells[2].cell[1] = payoffMatrixPanel.transform.Find("TableColumn_1").Find("TableCell_2_1").gameObject;

        tableCells[0].cell[2] = payoffMatrixPanel.transform.Find("TableColumn_2").Find("TableCell_0_2").gameObject;
        tableCells[1].cell[2] = payoffMatrixPanel.transform.Find("TableColumn_2").Find("TableCell_1_2").gameObject;
        tableCells[2].cell[2] = payoffMatrixPanel.transform.Find("TableColumn_2").Find("TableCell_2_2").gameObject;

        alignCells(true);
    }
}
