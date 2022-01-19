using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LikeOrDislike : MonoBehaviour
{
    public void like()
    {
        StartCoroutine(addLike());
    }

    IEnumerator addLike()
    {
        //Create an HTML form with the data
        WWWForm form = new WWWForm();
        form.AddField("class", "LikesDislikesController\\likesDislikes");
        form.AddField("function", "create");
        form.AddField("userID", Buffer.instance.authenticatedUser.ID);
        form.AddField("simulationID", Buffer.instance.currentSimulation.ID);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        //we display the success notification and put the received simulation data in the buffer
        if (www.text != "" && www.text[0] == '0')
        {
            //Display the notification
            StartCoroutine(Notification.instance.showNotification(www.text));
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification("Loading agents failed. Error #" + www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
        }
    }

    public void unLike()
    {
        StartCoroutine(removeLike());
    }

    IEnumerator removeLike()
    {
        //Create an HTML form with the data
        WWWForm form = new WWWForm();
        form.AddField("class", "LikesDislikesController\\likesDislikes");
        form.AddField("function", "delete");
        form.AddField("ID", Buffer.instance.authenticatedUser.ID);
        form.AddField("simulationID", Buffer.instance.currentSimulation.ID);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        //we display the success notification and put the received simulation data in the buffer
        if (www.text != "" && www.text[0] == '0')
        {
            //Display the notification
            StartCoroutine(Notification.instance.showNotification(www.text));
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification("Loading agents failed. Error #" + www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
        }
    }

    public void dislike()
    {
        StartCoroutine(addDislike());
    }

    IEnumerator addDislike()
    {
        WWWForm form = new WWWForm();
        form.AddField("class", "LikesDislikesController\\likesDislikes");
        form.AddField("function", "create");
        form.AddField("userID", Buffer.instance.authenticatedUser.ID);
        form.AddField("simulationID", Buffer.instance.currentSimulation.ID);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code
    }

    public void unDislike()
    {
        StartCoroutine(removeDislike());
    }

    IEnumerator removeDislike()
    {
        //Create an HTML form with the data
        WWWForm form = new WWWForm();
        form.AddField("class", "LikesDislikesController\\likesDislikes");
        form.AddField("function", "delete");
        form.AddField("userID", Buffer.instance.authenticatedUser.ID);
        form.AddField("simulationID", Buffer.instance.currentSimulation.ID);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        //we display the success notification and put the received simulation data in the buffer
        if (www.text != "" && www.text[0] == '0')
        {
            //Display the notification
            StartCoroutine(Notification.instance.showNotification(www.text));
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification("Loading agents failed. Error #" + www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
        }
    }
}
