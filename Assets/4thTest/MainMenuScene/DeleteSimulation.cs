using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeleteSimulation : MonoBehaviour
{

    public void delete(GameObject ID)
    {
        StartCoroutine(deleteSimulation(ID));
        StartCoroutine(deleteItsPayoffFormulas(ID));
        LoadOrRefreshSimulations.instance.loadPopular(true);
        LoadOrRefreshSimulations.instance.loadUser(true);


        //LoadOrRefreshSimulations.loadPopular(true);
        //LoadOrRefreshSimulations.loadUser(true);
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
            Debug.Log("0; Update succesful");
        }
        else
        {
            Debug.Log("Loading agents failed. Error #" + www.text);
        }
    }

    IEnumerator deleteItsPayoffFormulas(GameObject ID)
    {
        WWWForm form = new WWWForm();
        form.AddField("class", "PayoffFormulasController\\payoffFormulas");
        form.AddField("function", "delete");
        form.AddField("simulationID", ID.transform.GetComponent<Text>().text);

        WWW www = new WWW("http://localhost/sqlconnect/index.php", form);
        yield return www; //tells Unity to put this on the backburner. Once we get the info back, we'll run the rest of the code

        if (www.text != "" && www.text[0] == '0')
        {
            Debug.Log("0; Update succesful");
        }
        else
        {
            Debug.Log("Loading agents failed. Error #" + www.text);
        }
    }
}
