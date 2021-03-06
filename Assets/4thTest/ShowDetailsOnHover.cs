using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowDetailsOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject detailsOnHover;
    public GameObject agentDetails;
    public GameObject agentID;
    public GameObject agentName;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = detailsOnHover.GetComponent<RectTransform>();
        canvasGroup = detailsOnHover.GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        OnPointerOverCustom();
    }

    private void OnPointerOverCustom()
    {
        
        if(detailsOnHover.active == true)
        {
            //detailsOnHover.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;

            //get the Input from Horizontal axis
            float horizontalInput = Input.GetAxis("Horizontal");
            //get the Input from Vertical axis
            float verticalInput = Input.GetAxis("Vertical");

            Debug.Log("horizontalInput: " + horizontalInput + ", verticalInput: " + verticalInput);
            Debug.Log(detailsOnHover.transform.position);


            //update the position
            detailsOnHover.transform.position = Input.mousePosition + new Vector3(85f, -50f, 0f);

        }
        /**/
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter");
        detailsOnHover.SetActive(true);

        detailsOnHover.transform.GetChild(0).GetComponent<Text>().text = agentDetails.transform.GetChild(0).GetComponent<Text>().text;
        detailsOnHover.transform.GetChild(1).GetComponent<Text>().text = agentDetails.transform.GetChild(1).GetComponent<Text>().text;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        Debug.Log("OnPointerExit");
        detailsOnHover.SetActive(false);
        /**/
    }
}
