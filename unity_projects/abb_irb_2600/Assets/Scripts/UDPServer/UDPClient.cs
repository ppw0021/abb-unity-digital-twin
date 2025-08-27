using UnityEngine;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
//using UnityEditor.PackageManager;
using System.Collections;
using TMPro;
// using NUnit.Framework.Constraints;
using System.Text.RegularExpressions;

[SerializeField]
public class IRB2600
{
    public double joint_1;
    public double joint_2;
    public double joint_3;
    public double joint_4;
    public double joint_5;
    public double joint_6;
}
public class UDPClient : MonoBehaviour
{
    [SerializeField]
    public string serverIP = "127.0.0.1";
    [SerializeField]
    private TMP_InputField inputField;  // This should be the input parent field and not the child text field. See this post for more details.
    [SerializeField]
    private Button connectButton;
    [SerializeField]
    private Button disconnectButton;
    [SerializeField]
    private int port = 9999;
    private UdpClient client;
    [SerializeField]
    int requestInterval = 200;
    private UDPMoveRobot moveRobot;
    public bool connectedFlag = false;
    public async Task Start()
    {
        moveRobot = GetComponent<UDPMoveRobot>();
        if (moveRobot == null)
        {
            Debug.LogError("MoveRobot script not found on this GameObject.");
        }

        await MainRepeatingLoop();
    }
    public void StartComm()
    {
        serverIP = inputField.text;
        // serverIP = inputField.GetComponent<TMP_InputField>().text;
        connectedFlag = true;
    }

    public void StopComm()
    {
        connectedFlag = false;
    }

    private async Task SendUDPRequestToServerAsync()
    {
        string message = "Hello from Unity";

        byte[] data = Encoding.ASCII.GetBytes(message);

        IRB2600 robotDataIn = new IRB2600();

        string recv;
        try
        {
            bool isValid = Regex.IsMatch(serverIP, @"^[0-9.]+$");
            if (!isValid)
            {
                Debug.Log("Invalid IP");
                connectedFlag = false;
                return;
            }
            client = new UdpClient();

            client.Connect(serverIP, port);

            await client.SendAsync(data, data.Length);

            int timeoutMs = 3000; // 3 seconds

            var receiveTask = client.ReceiveAsync();
            var delayTask = Task.Delay(timeoutMs);

            var completedTask = await Task.WhenAny(receiveTask, delayTask);

            if (completedTask == receiveTask)
            {
                UdpReceiveResult result = receiveTask.Result;
                recv = Encoding.ASCII.GetString(result.Buffer);
                Debug.Log($"UDP received: {recv}");
            }
            else
            {
                Debug.Log("Receive timed out.");
                connectedFlag = false;
                return;
            }
        }
        catch
        {
            Debug.Log("Server is not running");
            connectedFlag = false;
            return;
        }
        try
        {
            robotDataIn = JsonUtility.FromJson<IRB2600>(recv);
            // Debug.Log("Successfully Objectified");
            moveRobot.SetLinkRotation(robotDataIn);
            // Debug.Log($"Joint 1: {robotDataIn.joint_1}");
        }
        catch
        {
            Debug.Log("Didn't work lol");
        }

        // Debug.Log($"Received: {result}");
    }

    private async Task<IEnumerator> MainRepeatingLoop()
    {
        while (true && Application.isPlaying)
        {
            if (connectedFlag)
            {
                await SendUDPRequestToServerAsync();
                await Task.Delay(requestInterval);
            }
            else
            {
                await Task.Delay(1000);
            }

        }
        return null;
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
