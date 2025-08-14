using UnityEngine;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using UnityEditor.PackageManager;

[SerializeField]
public class IRB2600
{
    public double joint1;
    public double joint2;
    public double joint3;
    public double joint4;
    public double joint5;
    public double joint6;
}
public class UDPClient : MonoBehaviour
{
    [SerializeField]
    private Button RequestButton;
    [SerializeField]
    private string serverIP = "127.0.0.1";
    [SerializeField]
    private int port = 9999;

    private UdpClient client;
    public void Start()
    {
        RequestButton.onClick.AddListener(async () =>
        {
            await SendUDPRequestToServerAsync();
        });
    }

    void Update()
    {
        
    }

    private async Task SendUDPRequestToServerAsync()
    {
        client = new UdpClient();
        client.Connect(serverIP, port);

        string message = "Hello from Unity";

        byte[] data = Encoding.ASCII.GetBytes(message);

        await client.SendAsync(data, data.Length);

        byte[] buffer = new byte[1024];

        UdpReceiveResult result = await client.ReceiveAsync();

        string recv = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length);

        Debug.Log($"Received: {recv}");
    }
}
