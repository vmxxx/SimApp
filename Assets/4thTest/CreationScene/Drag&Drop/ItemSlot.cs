using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UI;
using static System.Math;

public class ItemSlot : MonoBehaviour, IDropHandler
{

    public GameObject payoffMatrixPanel;
    public GameObject agentOnDrag;
    public GameObject cellToDragOn;

    private GameObject oppositeCell;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if (eventData.pointerDrag != null)
        {
            int agentID = Int32.Parse(agentOnDrag.transform.GetChild(0).GetComponent<Text>().text);
            Debug.Log("agentID" + agentID);
            string agentName = agentOnDrag.transform.GetChild(1).GetComponent<Text>().text;
            gameObject.transform.GetChild(0).GetComponent<Text>().text = agentID.ToString();
            gameObject.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = agentName;
            int indexX = (int)(gameObject.name[12] - '0');
            int indexY = (int)(gameObject.name[10] - '0');
            if (indexX == 0)
            {
                oppositeCell = payoffMatrixPanel.transform.Find("TableCell_0_" + indexY).gameObject;
            }
            else
            {
                oppositeCell = payoffMatrixPanel.transform.Find("TableCell_" + indexX + "_0").gameObject;
            }
            Debug.Log(indexY + ", " + indexX);
            Debug.Log(oppositeCell.name);
            oppositeCell.transform.GetChild(0).GetComponent<Text>().text = agentID.ToString();
            oppositeCell.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = agentName;
        }
    }
}
