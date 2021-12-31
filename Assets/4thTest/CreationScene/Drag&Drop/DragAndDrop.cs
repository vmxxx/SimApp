using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{

    [SerializeField] private Canvas canvas;


    public GameObject agentOnDrag;
    public GameObject agentID;
    public GameObject agentName;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = agentOnDrag.GetComponent<RectTransform>();
        canvasGroup = agentOnDrag.GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        canvasGroup.alpha = 0.6f;
        //agentName.GetComponent<Text>().text = agentOnDrag.transform.GetChild(1).GetComponent<Text>().text;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        rectTransform.anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        //rectTransform.anchoredPosition += eventData.delta; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        agentOnDrag.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        agentOnDrag.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = gameObject.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text;
        agentOnDrag.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = gameObject.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text;
        OnBeginDrag(eventData); OnDrag(eventData);
        agentOnDrag.SetActive(true);
    }
}
