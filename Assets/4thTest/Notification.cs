using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{

    public static Notification instance;

    public void Awake()
    {
        if(Notification.instance == null)
        {
            this.gameObject.name = "Notification";
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else { Destroy(this.gameObject); }
    }

    public IEnumerator showNotification(string notification)
    {
        Debug.Log("SHOWING NOTIFICATION!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        gameObject.GetComponent<Text>().text = notification;
        yield return new WaitForSeconds(2);
        gameObject.GetComponent<Text>().text = "";
    }

    public void Update()
    {
        Debug.Log("NOTFIFCATION.INSTANCE == SOMETHING");
        if(Notification.instance == null)
        {
            instance = this;
        }
    }
}
