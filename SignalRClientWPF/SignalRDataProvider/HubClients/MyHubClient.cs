using Damienbod.SignalR.IHubSync.Client;
using Damienbod.SignalR.IHubSync.Client.Dto;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using PortableSignalR.Model;
using SignalRDataProvider.Logging;
using System;

namespace SignalRDataProvider.HubClients
{
    public class MyHubClient : BaseHubClient, ISendHubSync, IRecieveHubSync
    {
        public event Action<MyMessage> RecievedMessageEvent;

        public MyHubClient()
        {
            Init();
        }

        public new void Init()
        {
            HubConnectionUrl = "http://localhost:8000/";
            HubProxyName = "signalr";
            QueryString = "enc_auth_token=wNYmO41%2F48SHNstaLVXxHCCre29BZQl1NhC6NM3R3rzpXtPQxVzH6jEzA%2FQhXFN5tu6Fk7pO53uppm1mVXMZgxbyRVz26dnepi%2FFyB6axBY%2B6gq1GL%2BuRQgoiFUCjRN2p8w6LevViwKlHyWZZJZO1DGVSjAi1m2U%2Bog9pkHw9%2FQR4Nl%2FDPnoP9JYDMpZ1zxx09u6s0GZ9%2FQ5Sjk%2BL0UfcSCbl38X8he5w9UIn%2FHvxh7ysM1CiPLsoOwtbiieSRVmrmt0JjnipAn4%2FK283F8GrGwzwgehWsqefmUnM0ckMwP9ZAdwQxWDhxv0IqNw4tDhwUYs%2F1SYdYozdNzgByhgNOBPzQDObNLlWc4vV5VMOib8EjSYXFXLU59%2BtB4PleJN5jPFAmQ8w%2BsSUWe%2FAC2nzAM9ucuzy7hXu56Mge6FCOBNXMQkqn4OmiLiVSIAy3NzTfl38kXN%2Fqiit9GmFTupj8uh79091F8PaC71HoeG8ydTce6Ev4lAFGkfSbkJhX4PSjTAepx8M5zcJr3lamV8eVl9AL8qTv3geEOSBCL8TSkqseblin7ZnZ7d2fvtjSHatIiUJ3rZSU1XbsvHfF2uOIN%2FZ4AVsmoKCP0q4wAMKd%2BQ30a1helO5z%2B3Six2RmgXwKEAV8ihCAPP9EW2Tt96Ayiyju1WrsTXYyCcoMHHx15thkNVXMyagugbUxlfTWv8OgkVQzJTUukLiuvhh94vNTLSv%2BDG0quW52f%2BSkr9t2HSzzL2GCddv4%2BFS%2F%2BTGOgFKard4EGMaRjqxW4J3Gk6g5RhPmZazo5O3tZ8whM5GxgqH4w5TolLkO3YezFhI3Xw1%2BaB3NbH3sgRRAC6x8iezEasPMMR2d6AqPOMEsWShZHxEC5DWKMKhy3Ef65k8cJiKmAn2CZnrDO6UuHHE27LLPFRLzltKoSyWZ8oZ09j63WehU2GfmevOn8BGI1wS6D%2F0b0cJVaN1Q7NHaVaEOU%2FH1%2BWP%2Fo4yg2B%2FzObz2y%2F7%2FwdRZHXoMw8Ngbg1xeDQBEydRCZyV8GvMgyqWVx0UKRCrBq3JClXI0vvv43nhkxAEE%3D";

            //HubConnectionUrl = "http://localhost:8089/";
            //HubProxyName = "Hubsync";

            HubTraceLevel = TraceLevels.None;
            HubTraceWriter = Console.Out;

            base.Init();

            _myHubProxy.On<string, string>("getNotification", Recieve_AddMessage);
            _myHubProxy.On("Heartbeat", Recieve_Heartbeat);
            _myHubProxy.On<HelloModel>("SendHelloObject", Recieve_SendHelloObject);

            StartHubInternal();
        }

        public override void StartHub()
        {
            //_hubConnection.Dispose();
            Init();
        }

        public void Recieve_AddMessage(string name, string message)
        {
            if (RecievedMessageEvent != null) RecievedMessageEvent.Invoke(new MyMessage { Name = name , Message = message});
            HubClientEvents.Log.Informational("Recieved addMessage: " + name + ": " + message);
        }

        public void Recieve_Heartbeat()
        {
            if (RecievedMessageEvent != null) RecievedMessageEvent.Invoke(new MyMessage { Name = "Heartbeat", Message = "recieved" });
            HubClientEvents.Log.Informational("Recieved heartbeat ");
        }

        public void Recieve_SendHelloObject(HelloModel hello)
        {
            if (RecievedMessageEvent != null) RecievedMessageEvent.Invoke(new MyMessage { Name = hello.Age.ToString(), Message = hello.Molly });
            HubClientEvents.Log.Informational("Recieved sendHelloObject " + hello.Molly + ", " + hello.Age);
        }

        public void AddMessage(string name, string message)
        {
            _myHubProxy.Invoke("AddMessage", name, message).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    HubClientEvents.Log.Error("There was an error opening the connection:" + task.Exception.GetBaseException());
                }

            }).Wait();
            HubClientEvents.Log.Informational("Client Sending addMessage to server");
        }

        public void Heartbeat()
        {
            _myHubProxy.Invoke("Heartbeat").ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    HubClientEvents.Log.Error("There was an error opening the connection:" + task.Exception.GetBaseException());
                }

            }).Wait();
            HubClientEvents.Log.Informational("Client heartbeat sent to server");
        }

        public void SendHelloObject(HelloModel hello)
        {
            _myHubProxy.Invoke<HelloModel>("SendHelloObject", hello).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    HubClientEvents.Log.Error("There was an error opening the connection:" + task.Exception.GetBaseException());
                }

            }).Wait();
            HubClientEvents.Log.Informational("Client sendHelloObject sent to server");
        }


    }
}
