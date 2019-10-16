using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;

public class SampleClient : MonoBehaviour,
                            IMqttClientConnectedHandler,
                            IMqttClientDisconnectedHandler,
                            IMqttApplicationMessageReceivedHandler
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
        client.ConnectedHandler = this;
        client.DisconnectedHandler = this;
        client.ApplicationMessageReceivedHandler = this;

        await ConnectAsync(ipAddress);
    }

    async void OnDestroy()
    {

        Debug.Log("start disconnect");
        await client.DisconnectAsync();
        Debug.Log("disconnected");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PublishMessage();
        }
    }

    public async void Connect(string address)
    {
        await ConnectAsync(address);
    }

    public async Task ConnectAsync(string address)
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(address, port)
            .Build();

        var result = await client.ConnectAsync(options);
        Debug.Log($"Connected to the broker: {result.IsSessionPresent}");

        var topic = new TopicFilterBuilder()
            .WithTopic("my/test")
            .Build();
        await client.SubscribeAsync("/my/test");

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

    public Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
    {
        return new Task(() =>
        {
            Debug.Log($"HandleConnectedAsync: {eventArgs}");
        });
    }

    public Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
    {
        return new Task(() =>
        {
            Debug.Log($"On Disconnected: {eventArgs}");
        });
    }

    public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        return new Task(() =>
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Message:");
            sb.AppendFormat("ClientID: {0}\n", e.ClientId);
            sb.AppendFormat("Topic: {0}\n", e.ApplicationMessage.Topic);
            sb.AppendFormat("Payload: {0}\n", Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
            sb.AppendFormat("QoS: {0}\n", e.ApplicationMessage.QualityOfServiceLevel);
            sb.AppendFormat("Retain: {0}\n", e.ApplicationMessage.Retain);

            Debug.Log(sb);
        });
    }

}
