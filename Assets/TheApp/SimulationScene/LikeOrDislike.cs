using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class LikeOrDislike : MonoBehaviour
{

    public Text likesCountText;
    public Text dislikesCountText;
    public GameObject likeButton;
    public GameObject dislikeButton;

    private Text likeButtonText;
    private Text dislikeButtonText;

    public void Start()
    {
        //likesCountText.text = Buffer.instance.currentSimulation.likesCount.ToString();
        //dislikesCountText.text = Buffer.instance.currentSimulation.dislikesCount.ToString();
        likeButtonText = likeButton.transform.GetChild(0).GetComponent<Text>();
        dislikeButtonText = dislikeButton.transform.GetChild(0).GetComponent<Text>();
        if (Buffer.instance.authenticatedUser.ID == 0) { likeButton.SetActive(false); dislikeButton.SetActive(false); }
        StartCoroutine(loadLikesAndDislikes());
    }

    IEnumerator loadLikesAndDislikes()
    {
        //Create an HTML form with the data
        WWWForm form = new WWWForm();
        form.AddField("class", "LikesDislikesController\\likesDislikes");
        form.AddField("function", "read");
        form.AddField("userID", Buffer.instance.authenticatedUser.ID);
        form.AddField("simulationID", Buffer.instance.currentSimulation.ID);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        //we display the success notification and put the received simulation data in the buffer
        if (www.text != "" && www.text[0] == '0')
        {
            //Display the notification
            if (www.text[2] == '1') likeButton.transform.GetChild(0).GetComponent<Text>().text = "Unlike";
            else if (www.text[2] == '0') dislikeButton.transform.GetChild(0).GetComponent<Text>().text = "Undislike";

        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
        }
    }

    public void like()
    {
        if (likeButtonText.text == "Like") { StartCoroutine(addLike( dislikeButtonText.text == "Undislike" )); }
        else { StartCoroutine(removeLike()); }
    }

    IEnumerator addLike(bool isDisliked)
    {
        //Create an HTML form with the data
        WWWForm form = new WWWForm();
        form.AddField("class", "LikesDislikesController\\likesDislikes");
        form.AddField("function", isDisliked ? "update" : "create");
        form.AddField("userID", Buffer.instance.authenticatedUser.ID);
        form.AddField("simulationID", Buffer.instance.currentSimulation.ID);
        form.AddField("isLike", 1);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        //we display the success notification and put the received simulation data in the buffer
        if (www.text != "" && www.text[0] == '0')
        {
            //Display the notification
            StartCoroutine(Notification.instance.showNotification(www.text));

            Buffer.instance.currentSimulation.likesCount++;
            likesCountText.text = Buffer.instance.currentSimulation.likesCount.ToString();
            likeButtonText.text = "Unlike";
            if (isDisliked)
            {
                Buffer.instance.currentSimulation.dislikesCount--;
                dislikesCountText.text = Buffer.instance.currentSimulation.dislikesCount.ToString();
                dislikeButtonText.text = "Dislike";
            }
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
        }
    }

    IEnumerator removeLike()
    {
        //Create an HTML form with the data
        WWWForm form = new WWWForm();
        form.AddField("class", "LikesDislikesController\\likesDislikes");
        form.AddField("function", "delete");
        form.AddField("userID", Buffer.instance.authenticatedUser.ID);
        form.AddField("simulationID", Buffer.instance.currentSimulation.ID);
        form.AddField("wasLike", 1);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        //we display the success notification and put the received simulation data in the buffer
        if (www.text != "" && www.text[0] == '0')
        {
            //Display the notification
            StartCoroutine(Notification.instance.showNotification(www.text));

            Buffer.instance.currentSimulation.likesCount--;
            likesCountText.text = Buffer.instance.currentSimulation.likesCount.ToString();
            likeButtonText.text = "Like";
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
        }
    }

    public void dislike()
    {
        if (dislikeButtonText.text == "Dislike") { StartCoroutine(addDislike(likeButtonText.text == "Unlike")); }
        else { StartCoroutine(removeDislike()); }
    }

    IEnumerator addDislike(bool isLiked)
    {
        WWWForm form = new WWWForm();
        form.AddField("class", "LikesDislikesController\\likesDislikes");
        form.AddField("function", isLiked ? "update" : "create");
        form.AddField("userID", Buffer.instance.authenticatedUser.ID);
        form.AddField("simulationID", Buffer.instance.currentSimulation.ID);
        form.AddField("isLike", 0);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        //we display the success notification and put the received simulation data in the buffer
        if (www.text != "" && www.text[0] == '0')
        {
            //Display the notification
            StartCoroutine(Notification.instance.showNotification(www.text));

            Buffer.instance.currentSimulation.dislikesCount++;
            dislikesCountText.text = Buffer.instance.currentSimulation.dislikesCount.ToString();
            dislikeButtonText.text = "Undislike";
            if (isLiked)
            {
                Buffer.instance.currentSimulation.likesCount--;
                likesCountText.text = Buffer.instance.currentSimulation.likesCount.ToString();
                likeButtonText.text = "Like";
            }
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
        }
    }

    IEnumerator removeDislike()
    {
        //Create an HTML form with the data
        WWWForm form = new WWWForm();
        form.AddField("class", "LikesDislikesController\\likesDislikes");
        form.AddField("function", "delete");
        form.AddField("userID", Buffer.instance.authenticatedUser.ID);
        form.AddField("simulationID", Buffer.instance.currentSimulation.ID);
        form.AddField("wasLike", 0);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        //we display the success notification and put the received simulation data in the buffer
        if (www.text != "" && www.text[0] == '0')
        {
            //Display the notification
            StartCoroutine(Notification.instance.showNotification(www.text));

            Buffer.instance.currentSimulation.dislikesCount--;
            dislikesCountText.text = Buffer.instance.currentSimulation.dislikesCount.ToString();
            dislikeButtonText.text = "Dislike";
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
        }
    }
}
