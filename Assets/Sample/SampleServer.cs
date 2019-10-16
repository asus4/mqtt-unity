using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using MQTTnet;
using MQTTnet.Server;
using MQTTnet.Protocol;

public class SampleServer : MonoBehaviour,
                            IMqttServerClientConnectedHandler,
                            IMqttServerClientDisconnectedHandler,
                            IMqttServerClientSubscribedTopicHandler,
                            IMqttServerClientUnsubscribedTopicHandler
{
    [SerializeField]
    int port = 1883;

    IMqttServer server;

    async void Start()
    {
        server = new MqttFactory().CreateMqttServer();
        server.ClientConnectedHandler = this;
        server.ClientDisconnectedHandler = this;
        server.ClientSubscribedTopicHandler = this;
        server.ClientUnsubscribedTopicHandler = this;

        var options = new MqttServerOptionsBuilder()
            .WithConnectionBacklog(10)
            .WithDefaultEndpointPort(port)
            // .WithConnectionValidator((c) =>
            // {
            //     Debug.Log($"ClientID: {c.ClientId}\nUsername: {c.Username}\nPassword:{c.Password}\nEndpoint:{c.Endpoint}");
            //     c.ReasonCode = MqttConnectReasonCode.Success;
            //     c.ReturnCode
            // })
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
        Debug.Log("server stopping");
        await server.StopAsync();
        Debug.Log("server stoped");
    }


    public Task HandleClientConnectedAsync(MqttServerClientConnectedEventArgs eventArgs)
    {
        return new Task(() =>
        {
            Debug.Log($"HandleClientConnectedAsync : {eventArgs}");
        });
    }


    public Task HandleClientDisconnectedAsync(MqttServerClientDisconnectedEventArgs eventArgs)
    {
        return new Task(() =>
        {
            Debug.Log($"HandleClientDisconnectedAsync : {eventArgs}");
        });
    }

    public Task HandleClientSubscribedTopicAsync(MqttServerClientSubscribedTopicEventArgs eventArgs)
    {
        return new Task(() =>
        {
            Debug.Log($"HandleClientSubscribedTopicAsync : {eventArgs}");
        });
    }

    public Task HandleClientUnsubscribedTopicAsync(MqttServerClientUnsubscribedTopicEventArgs eventArgs)
    {
        return new Task(() =>
        {
            Debug.Log($"HandleClientUnsubscribedTopicAsync : {eventArgs}");
        });
    }


}
