using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
    StringBuilder sb = new StringBuilder();

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

        var topic = new TopicFilterBuilder()
            .WithTopic("my/test")
            .Build();
        await client.SubscribeAsync("/my/test");

        Debug.Log("Subscribed");
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

    async void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            var msg = new MqttApplicationMessageBuilder()
                .WithTopic("/my/test")
                .WithPayload("hgoehoge")
                .WithExactlyOnceQoS()
                .Build();
            await client.PublishAsync(msg);
        }
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
        sb.Clear();
        sb.AppendLine("Message:");
        sb.AppendFormat("ClientID: {0}\n", e.ClientId);
        sb.AppendFormat("Topic: {0}\n", e.ApplicationMessage.Topic);
        sb.AppendFormat("Payload: {0}\n", Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
        sb.AppendFormat("QoS: {0}\n", e.ApplicationMessage.QualityOfServiceLevel);
        sb.AppendFormat("Retain: {0}\n", e.ApplicationMessage.Retain);
        
        Debug.Log(sb);
    }
}

