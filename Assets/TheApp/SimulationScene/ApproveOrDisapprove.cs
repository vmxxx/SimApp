using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApproveOrDisapprove : MonoBehaviour
{
    public Text approvedValText;
    public GameObject approveButton;

    private Text approveButtonText;

    // Start is called before the first frame update
    void Start()
    {
        approveButtonText = approveButton.transform.GetChild(0).GetComponent<Text>();
        if (Buffer.instance.authenticatedUser.isAdmin) { approveButton.SetActive(true); }
    }

    //This function gets called when an administrator user clicks on the "approve" button
    public void approve()
    {
        StartCoroutine(approveSimulation( (approvedValText.text == "yes") ? true : false ));
    }

    IEnumerator approveSimulation(bool alreadyApproved)
    {
        //Create an HTML form with the data
        WWWForm form = new WWWForm();
        form.AddField("class", "SimulationsController\\simulations");
        form.AddField("function", "update");
        form.AddField("onApproval", "true");
        form.AddField("approved", alreadyApproved ? "0" : "1");
        form.AddField("ID", Buffer.instance.currentSimulation.ID);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        //If there is no NULL notification AND if the notification code is 0 (no error)
        if (www.text != "" && www.text[0] == '0')
        {
            Buffer.instance.currentSimulation.approved = alreadyApproved ? false : true;
            approvedValText.text = alreadyApproved ? "no" : "yes";
            approveButtonText.text = alreadyApproved ? "Approve" : "Unapprove";
        }
        else //Display error notification
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }
    }
}
