using UnityEngine;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;
using System.Threading.Tasks;
using System.Collections;
using System.Linq.Expressions;

public class IRBRequest_14000 : MonoBehaviour
{
    [Header("Connection")]
    [SerializeField]
    private string IpAddress = "192.168.125.1";
    [SerializeField]
    private string username = "Default User";
    [SerializeField]
    private string password = "robotics";


    private HttpClient client;



    private bool demoState = false;

    [Header("Request Rate")]
    [SerializeField]
    private float requestInterval = 0.1f; // 100ms interval

    [SerializeField]
    private MoveRobot_14000 ROB_R;
    [SerializeField]
    private MoveRobot_14000 ROB_L;

    [Header("Demo Live Data (Degrees)")]
    [SerializeField]
    public bool demo = false;
    [SerializeField]
    public float[] sampleData = new float[7]{
        10,
        20,
        30,
        40,
        50,
        60,
        70
    };

    void Start()
    {
        if (ROB_R == null)
        {
            Debug.LogError("R_ROB MoveRobot script not found");
        }
        if (ROB_L == null)
        {
            Debug.LogError("L_ROB MoveRobot script not found");
        }
        if (!demo)
        {
            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(username, password)
            };
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            StartCoroutine(UpdateJointTargetRoutine());

        }
        StartCoroutine(UpdateJointTargetRoutine());



    }

    private IEnumerator UpdateJointTargetRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(GetJointTargetCoroutine("ROB_R"));
            yield return new WaitForSeconds(requestInterval); // Wait for 100ms before calling again
            yield return StartCoroutine(GetJointTargetCoroutine("ROB_L"));
            yield return new WaitForSeconds(requestInterval); // Wait for 100ms before calling again
        }
    }

    private IEnumerator GetJointTargetCoroutine(string robotName)
    {
        string url = "http://" + IpAddress + "/rw/motionsystem/mechunits/" + robotName + "/jointtarget";

        if (demo)
        {
            if (!demoState)
            {
                Debug.Log("Demo mode, Default values set");
                demoState = true;
            }
            for (int i = 0; i < 6; i++)
            {
                switch (robotName) {
                    case "ROB_R":
                        ROB_R.SetLinkRotation(i + 1, sampleData[i]);
                        break;
                    case "ROB_L":
                        ROB_L.SetLinkRotation(i + 1, sampleData[i]);
                        break;
                    default:
                        Debug.LogError("Invalid robot name: " + robotName);
                        yield break;
                }
            }
            yield break;
        }

        var task = client.GetAsync(url);
        while (!task.IsCompleted) yield return null;

        if (task.IsFaulted || task.Result == null)
        {
            Debug.LogError("HTTP request failed");
            yield break;
        }

        var response = task.Result;
        var contentTask = response.Content.ReadAsStringAsync();
        while (!contentTask.IsCompleted) yield return null;
        var content = contentTask.Result;

        bool proceed = true;
        var xmlDoc = new System.Xml.XmlDocument();
        try
        {
            xmlDoc.LoadXml(content);
        }
        catch
        {
            Debug.Log("Catch block reached");
            proceed = false;
        }

        if (proceed)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("x", "http://www.w3.org/1999/xhtml");

            // Select all <span> elements under the first <li> with class "ms-jointtarget"
            XmlNodeList spanNodes = xmlDoc.SelectNodes("//x:li[@class='ms-jointtarget']/x:span", nsmgr);

            for (int i = 0; i < 7 && i < spanNodes.Count; i++)
            {
                XmlNode node = spanNodes[i];
                if (node != null && float.TryParse(node.InnerText, out float value))
                {
                    switch (robotName)
                    {
                        case "ROB_R":
                            ROB_R.SetLinkRotation(i + 1, value); // +1 because SetLinkRotation is 1-based
                            break;
                        case "ROB_L":
                            ROB_L.SetLinkRotation(i + 1, value); // +1 because SetLinkRotation is 1-based
                            break;
                        default:
                            Debug.LogError("Invalid robot name: " + robotName);
                            yield break;
                    }
                    //ROB_R.SetLinkRotation(i + 1, value); // +1 because SetLinkRotation is 1-based
                }
                else
                {
                    Debug.LogWarning($"Span at index {i} not found or value could not be parsed.");
                }
            }
        }

    }

    private void OnDestroy()
    {
        if (client != null)
        {
            client.Dispose();
            client = null;
            Debug.Log("HttpClient disposed. Session Ended.");
        }
    }
}