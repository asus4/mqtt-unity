using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MQTTnet;
using MQTTnet.Server;

public class SampleServer : MonoBehaviour
{
    IMqttServer server;

    async void Start()
    {
        server = new MqttFactory().CreateMqttServer();
        var options = new MqttServerOptionsBuilder()
            .WithConnectionBacklog(10)
            .WithDefaultEndpointPort(1883)
            .Build();
        await server.StartAsync(options);

        Debug.Log("server started");
    }

    async void OnDestroy()
    {
        Debug.Log("server stopping");
        await server.StopAsync();
        Debug.Log("server stoped");
    }
}
