using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowDetailsOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject detailsOnHover;
    public GameObject agentID;
    public GameObject agentName;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = detailsOnHover.GetComponent<RectTransform>();
        canvasGroup = detailsOnHover.GetComponent<CanvasGroup>();
    }

    public void OnMouseOver(PointerEventData eventData)
    {
        Debug.Log("OnMouseOver");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter");
        detailsOnHover.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit");
        detailsOnHover.SetActive(false);
    }
}
