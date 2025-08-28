using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Text;

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
    private float requestInterval = 0.1f;
    private UDPMoveRobot moveRobot;
    [SerializeField]
    private bool connectedFlag = false;

    public void Start()
    {
        moveRobot = GetComponent<UDPMoveRobot>();
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
            if (!connectedFlag)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            string url = $"http://{serverIP}:{port}/joints";
            UnityWebRequest uwr = UnityWebRequest.Get($"http://{serverIP}:{port}/joints");
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log($"Error While Sending to {url}: {uwr.error}");
            }
            else
            {
                // Debug.Log("Received: " + uwr.downloadHandler.text);
                string result = uwr.downloadHandler.text;

                IRB2600 robotDataIn;
                try
                {
                    robotDataIn = JsonUtility.FromJson<IRB2600>(result);
                    // Debug.Log("Successfully Objectified");
                    moveRobot.SetLinkRotation(robotDataIn);
                    // Debug.Log($"Joint 1: {robotDataIn.joint_1}");
                }
                catch
                {
                    Debug.Log("Didn't work lol");
                }
            }
            yield return new WaitForSeconds(requestInterval);
        }
    }

    public void Update()
    {
        if (connectedFlag)
        {
            inputField.interactable = false;
            connectButton.interactable = false;
            disconnectButton.interactable = true;
        }
        else
        {
            inputField.interactable = true;
            connectButton.interactable = true;
            disconnectButton.interactable = false;
        }
    }
}