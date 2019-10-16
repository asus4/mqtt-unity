using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;


public class SampleManagedClient : MonoBehaviour,
                                   IMqttClientConnectedHandler,
                                   IMqttClientDisconnectedHandler,
                                   IMqttApplicationMessageReceivedHandler
{
    [SerializeField]
    string ipAddress = "";

    [SerializeField]
    int port = 1883;

    [SerializeField]
    Transform target = null;


    IManagedMqttClient client;
    StringBuilder sb = new StringBuilder();

    async void Start()
    {
        client = new MqttFactory().CreateManagedMqttClient();
        client.ConnectedHandler = this;
        client.DisconnectedHandler = this;
        client.ApplicationMessageReceivedHandler = this;

        await ConnectAsync(ipAddress);
    }

    async void OnDestroy()
    {
        Debug.Log("start disconnect");
        await client.StopAsync();
        Debug.Log("disconnected");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PublishMessage();
        }
        SendPosition();
    }

    public async void Connect(string address)
    {
        Debug.Log($"connect to {address}");
        await ConnectAsync(address);
    }

    public async Task ConnectAsync(string address)
    {
        if (client.IsStarted)
        {
            await client.StopAsync();
        }

        var clientOpitons = new MqttClientOptionsBuilder()
            .WithTcpServer(address, port)
            .Build();
        var managedOptions = new ManagedMqttClientOptionsBuilder()
            .WithClientOptions(clientOpitons)
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
            .Build();

        await client.StartAsync(managedOptions);
        await client.SubscribeAsync("/my/test");
        await client.SubscribeAsync("/my/transform");

        Debug.Log("Subscribed");
    }

    public async void PublishMessage()
    {
        await PublishMessageAsync();
    }

    public async Task PublishMessageAsync()
    {
        var msg = new MqttApplicationMessageBuilder()
                .WithTopic("/my/test")
                .WithPayload("hgoehoge")
                .WithExactlyOnceQoS()
                .Build();
        await client.PublishAsync(msg);
    }

    private async void SendPosition()
    {
        var msg = new MqttApplicationMessageBuilder()
                        .WithTopic("/my/transform")
                        .WithPayload(target.position.ToString())
                        .WithExactlyOnceQoS()
                        .Build();
        await client.PublishAsync(msg);
    }


    public Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
    {
        Debug.Log($"HandleConnectedAsync: {eventArgs}");

        return new Task(() =>
        {
        });
    }

    public Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
    {
        Debug.Log($"On Disconnected: {eventArgs}");

        return new Task(() =>
        {
        });
    }

    public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Message:");
        sb.AppendFormat("ClientID: {0}\n", e.ClientId);
        sb.AppendFormat("Topic: {0}\n", e.ApplicationMessage.Topic);
        sb.AppendFormat("Payload: {0}\n", Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
        sb.AppendFormat("QoS: {0}\n", e.ApplicationMessage.QualityOfServiceLevel);
        sb.AppendFormat("Retain: {0}\n", e.ApplicationMessage.Retain);

        Debug.Log(sb);
        
        return new Task(() =>
        {

        });
    }
}

