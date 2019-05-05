using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MQTTnet;
using MQTTnet.Client;

public class SampleClient : MonoBehaviour
{
    IMqttClient client;

    async void Start()
    {
        client = new MqttFactory().CreateMqttClient();
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883)
            .Build();

        await client.ConnectAsync(options);
        Debug.Log("connected to the server");
    }

    async void OnDestroy()
    {
        Debug.Log("start disconnect");
        await client.DisconnectAsync();
        Debug.Log("disconnected");
    }
}
