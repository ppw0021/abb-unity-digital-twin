using UnityEngine;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections;
using System.Linq.Expressions;

public class IRBRequest : MonoBehaviour
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

    private MoveRobot moveRobot;

    [Header("Demo Live Data (Degrees)")]
    [SerializeField]
    public bool demo = false;
    [SerializeField]
    public float[] sampleData = new float[6]{
        10,
        20,
        30,
        40,
        50,
        60
    };

    void Start()
    {
        moveRobot = GetComponent<MoveRobot>();
        if (moveRobot == null)
        {
            Debug.LogError("MoveRobot script not found on this GameObject.");
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
            yield return StartCoroutine(GetJointTargetCoroutine());
            yield return new WaitForSeconds(requestInterval); // Wait for 100ms before calling again
        }
    }

    private IEnumerator GetJointTargetCoroutine()
    {
        string url = "http://" + IpAddress + "/rw/motionsystem/mechunits/ROB_1/jointtarget";

        if (demo)
        {
            if (!demoState)
            {
                Debug.Log("Demo mode, Default values set");
                demoState = true;
            }
            for (int i = 0; i < 6; i++)
            {
                moveRobot.SetLinkRotation(i + 1, sampleData[i]);
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
            for (int i = 1; i <= 6; i++)
            {
                string className = $"rax_{i}";
                string xpath = $"//*[local-name()='span' and @class='{className}']";
                var node = xmlDoc.SelectSingleNode(xpath);

                if (node != null && float.TryParse(node.InnerText, out float value))
                {
                    moveRobot.SetLinkRotation(i, value);
                }
                else
                {
                    Debug.LogWarning($"{className} not found or could not be parsed.");
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