using UnityEngine;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class IRBRequest : MonoBehaviour
{
    private string url = "http://192.168.125.1/rw/motionsystem/mechunits/ROB_1/jointtarget";
    private string username = "Default User";
    private string password = "robotics";

    async void Start()
    {
        await GetJointTarget();
    }

    async Task GetJointTarget()
    {
        try
        {
            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(username, password)
            };

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                HttpResponseMessage response = await client.GetAsync(url);

                Debug.Log("Response Code: " + (int)response.StatusCode);
                string content = await response.Content.ReadAsStringAsync();

                // Load XML and parse rax_1
                var xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.LoadXml(content);
                float[] raxValues = new float[6];

                for (int i = 1; i <= 6; i++)
                {
                    string className = $"rax_{i}";
                    string xpath = $"//*[local-name()='span' and @class='{className}']";
                    var node = xmlDoc.SelectSingleNode(xpath);

                    if (node != null && float.TryParse(node.InnerText, out float value))
                    {
                        raxValues[i - 1] = value;
                        Debug.Log($"{className} value: {value}");
                    }
                    else
                    {
                        Debug.LogWarning($"{className} not found or could not be parsed.");
                    }
                }
                /*
                                // Example: access rax_3
                                float rax3 = raxValues[2]; // Index 2 == rax_3

                                for (int i = 0; i < raxValues.Length; i++)
                                {
                                    Debug.Log($"rax_{i + 1} value: {raxValues[i]}");
                                }
                */
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error fetching joint target: " + ex.Message);
        }
    }
}
