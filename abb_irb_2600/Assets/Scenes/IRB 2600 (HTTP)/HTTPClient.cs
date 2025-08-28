using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;


public class HTTPClient : MonoBehaviour
{
    [SerializeField]
    private string serverIP = "127.0.0.1";
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private Button connectButton;
    [SerializeField]
    private Button disconnectButton;
    [SerializeField]
    private int port = 8000;
    [SerializeField]
    private int requestInterval = 100;
    private MoveRobot moveRobot;
    [SerializeField]
    private bool connectedFlag = false;

    public void Start()
    {
        moveRobot = GetComponent<MoveRobot>();
        if (moveRobot == null)
        {
            Debug.LogError("MoveRobot script not found on this GameObject.");
        }

        StartCoroutine(MakeHTTPRequest());
    }
    public void StartComm()
    {
        serverIP = inputField.text;
        connectedFlag = true;
    }

    public void StopComm()
    {
        connectedFlag = false;
    }

    public IEnumerator MakeHTTPRequest()
    {
        while (true)
        {
            string url = $"http://{serverIP}:{port}/joints";
            UnityWebRequest uwr = UnityWebRequest.Get($"http://{serverIP}:{port}/joints");
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log($"Error While Sending to {url}: {uwr.error}");
            }
            else
            {
                Debug.Log("Received: " + uwr.downloadHandler.text);
            }
        }
    }
}