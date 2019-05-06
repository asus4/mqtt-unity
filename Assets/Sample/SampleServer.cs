using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MQTTnet;
using MQTTnet.Server;
using MQTTnet.Protocol;

public class SampleServer : MonoBehaviour
{
    [SerializeField]
    int port = 1883;

    IMqttServer server;

    async void Start()
    {
        server = new MqttFactory().CreateMqttServer();
        server.ClientConnected += OnClientConnected;
        server.ClientDisconnected += OnClientDisconnected;
        server.ClientSubscribedTopic += OnClientSubscribedTopic;
        server.ClientUnsubscribedTopic += OnClientUnsubscribedTopic;

        var options = new MqttServerOptionsBuilder()
            .WithConnectionBacklog(10)
            .WithDefaultEndpointPort(port)
            .WithConnectionValidator((c) =>
            {
                Debug.Log($"ClientID: {c.ClientId}\nUsername: {c.Username}\nPassword:{c.Password}\nEndpoint:{c.Endpoint}");
                c.ReturnCode = MqttConnectReturnCode.ConnectionAccepted;
            })
            .WithSubscriptionInterceptor((c) =>
            {
                Debug.Log($"ClientID: {c.ClientId}\n Topic:{c.TopicFilter.ToString()}");
                c.AcceptSubscription = true;
            })
            .Build();

        await server.StartAsync(options);

        Debug.Log("server started");
    }

    async void OnDestroy()
    {
        server.ClientConnected -= OnClientConnected;
        server.ClientDisconnected -= OnClientDisconnected;
        server.ClientSubscribedTopic -= OnClientSubscribedTopic;
        server.ClientUnsubscribedTopic -= OnClientUnsubscribedTopic;

        Debug.Log("server stopping");
        await server.StopAsync();
        Debug.Log("server stoped");
    }


    private void OnClientConnected(object sender, MqttClientConnectedEventArgs e)
    {
        Debug.Log($"OnClientConnected: {e}");
    }

    private void OnClientDisconnected(object sender, MqttClientDisconnectedEventArgs e)
    {
        Debug.Log($"OnClientDisconnected: {e}");
    }

    private void OnClientSubscribedTopic(object sender, MqttClientSubscribedTopicEventArgs e)
    {
        Debug.Log($"OnClientSubscribedTopic: {e}");
    }

    private void OnClientUnsubscribedTopic(object sender, MqttClientUnsubscribedTopicEventArgs e)
    {
        Debug.Log($"OnClientUnsubscribedTopic: {e}");
    }


}
