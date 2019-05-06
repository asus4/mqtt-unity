using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

public class SampleManagedClient : MonoBehaviour
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
        client.Connected += OnConnected;
        client.Disconnected += OnDisconnected;
        client.ApplicationMessageReceived += OnApplicationMessageReceived;

        await ConnectAsync(ipAddress);
    }

    async void OnDestroy()
    {
        client.Connected -= OnConnected;
        client.Disconnected -= OnDisconnected;
        client.ApplicationMessageReceived -= OnApplicationMessageReceived;

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

