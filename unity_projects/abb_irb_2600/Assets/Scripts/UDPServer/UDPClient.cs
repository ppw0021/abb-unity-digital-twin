using UnityEngine;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using UnityEditor.PackageManager;
using System.Collections;

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
    // [SerializeField]
    // private Button RequestButton;
    [SerializeField]
    private string serverIP = "127.0.0.1";
    [SerializeField]
    private int port = 9999;
    private UdpClient client;
    [SerializeField]
    int requestInterval = 200;
    private UDPMoveRobot moveRobot;
    public async Task Start()
    {
        // RequestButton.onClick.AddListener(async () =>
        // {
        //     await SendUDPRequestToServerAsync();
        // });

        moveRobot = GetComponent<UDPMoveRobot>();
        if (moveRobot == null)
        {
            Debug.LogError("MoveRobot script not found on this GameObject.");
        }

        await MainRepeatingLoop();
    }
    private async Task SendUDPRequestToServerAsync()
    {
        client = new UdpClient();
        client.Connect(serverIP, port);

        string message = "Hello from Unity";

        byte[] data = Encoding.ASCII.GetBytes(message);

        await client.SendAsync(data, data.Length);

        // byte[] buffer = new byte[1024];

        UdpReceiveResult result = await client.ReceiveAsync();

        string recv = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length);

        IRB2600 robotDataIn = new IRB2600();

        try
        {
            robotDataIn = JsonUtility.FromJson<IRB2600>(recv);
            Debug.Log("Successfully Objectified");
            moveRobot.SetLinkRotation(robotDataIn);
            Debug.Log($"Joint 1: {robotDataIn.joint_1}");
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
            await SendUDPRequestToServerAsync();
            await Task.Delay(requestInterval);
        }
        return null;
    }
}
