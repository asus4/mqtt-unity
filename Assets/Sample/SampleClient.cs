using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MQTTnet;
using MQTTnet.Client;

public class SampleClient : MonoBehaviour
{
    [SerializeField]
    string ipAddress = "";
    [SerializeField]
    int port = 1883;

    IMqttClient client;

    async void Start()
    {
        client = new MqttFactory().CreateMqttClient();
        client.Connected += OnConnected;
        client.Disconnected += OnDisconnected;
        client.ApplicationMessageReceived += OnApplicationMessageReceived;

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(ipAddress, port)
            .Build();

        await client.ConnectAsync(options);
        Debug.Log("connected to the server");
    }

    async void OnDestroy()
    {
        client.Connected -= OnConnected;
        client.Disconnected -= OnDisconnected;
        client.ApplicationMessageReceived -= OnApplicationMessageReceived;

        Debug.Log("start disconnect");
        await client.DisconnectAsync();
        Debug.Log("disconnected");
    }



    private void OnConnected(object sender, MqttClientConnectedEventArgs e)
    {
        Debug.Log($"On Connected: {e}");
    }

    private void OnDisconnected(object sender, MqttClientDisconnectedEventArgs e)
    {
        Debug.Log($"On Disconnected: {e}");
    }

    private void OnApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
    {
        Debug.Log($"OnApplicationMessageReceived: {e}");
    }
}

