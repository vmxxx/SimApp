using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeleteSimulation : MonoBehaviour
{

    public static DeleteSimulation instance;

    private void Update()
    {
        if (DeleteSimulation.instance == null) instance = this;
    }

    public void delete(GameObject ID)
    {
        StartCoroutine(deleteSimulation(ID));
        LoadOrRefreshSimulations.instance.loadPopular(true);
        LoadOrRefreshSimulations.instance.loadUser(true);
    }

    IEnumerator deleteSimulation(GameObject ID)
    {
        WWWForm form = new WWWForm();
        form.AddField("class", "SimulationsController\\simulations");
        form.AddField("function", "delete");
        form.AddField("ID", ID.GetComponent<Text>().text);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {

        }
        else
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }


        form = new WWWForm();
        form.AddField("class", "PayoffFormulasController\\payoffFormulas");
        form.AddField("function", "delete");
        form.AddField("simulationID", ID.transform.GetComponent<Text>().text);

        www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {

        }
        else
        {
            if (www.text != "") StartCoroutine(Notification.instance.showNotification(www.text));
            else StartCoroutine(Notification.instance.showNotification("Couldn't connect to server. Either we have technical difficulties or you have no internet."));
            yield break;
        }
    }
}
