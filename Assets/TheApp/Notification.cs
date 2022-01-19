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
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else { Destroy(this.gameObject); }
    }

    public IEnumerator showNotification(string notification)
    {
        gameObject.GetComponent<Text>().text = notification;
        yield return new WaitForSeconds(2);
        gameObject.GetComponent<Text>().text = "";
    }

    public void Update()
    {
        if(Notification.instance == null)
        {
            instance = this;
        }
    }
}
