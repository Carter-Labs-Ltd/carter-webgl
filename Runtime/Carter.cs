using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;


using UnityEngine.Networking;
using System.Collections;


namespace Carter {

    [System.Serializable]
    class AgentOutput {
        public string text;
        public string audio;
    }

    [System.Serializable]
    class AgentResponse {
        public AgentOutput output;
        public string input;
    }


     public class Agent : MonoBehaviour
    {
        public SocketIOUnity socket;

        public string url { get; set; }
        public string apiKey { get; set; }
        public string playerId { get; set; }
        public string agentId { get; set; }

        public Boolean connected { get; set; }
        public Boolean voice = true;

        public delegate void OnMessage(string message);
        public delegate void OnConnect();
        public delegate void OnDisconnect();
        public delegate void OnVoice(string audioId);

        OnConnect onConnect { get; set;}
        OnDisconnect onDisconnect { get; set;}
        OnMessage onMessage { get; set;}
        OnMessage onDictate { get; set;}
        OnVoice onVoice { get; set;}
       
        public void init(string apiKey, string playerId, string url, OnConnect onConnect, OnDisconnect onDisconnect, OnMessage onMessage, OnVoice onVoice, OnMessage onDictate = null)
        {
            this.apiKey = apiKey;
            this.agentId = agentId;
            this.url = url;
            this.playerId = playerId;
            this.onConnect = onConnect;
            this.onDisconnect = onDisconnect;
            this.onMessage = onMessage;
            this.onVoice = onVoice;
            this.onDictate = onDictate;

            connect();
        }


        public void connect()
        {

            var uri = new Uri(url);
            socket = new SocketIOUnity(uri, new SocketIOOptions
            {
                Query = new Dictionary<string, string>
                    {
                        {"token", "UNITY" }
                    }
                ,
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
            });

            socket.JsonSerializer = new NewtonsoftJsonSerializer();

            socket.OnConnected += (sender, e) =>
            {
                connected = true;
                onConnect();
            };

            socket.OnDisconnected += (sender, e) =>
            {
                connected = false;
                onDisconnect();
            };

            socket.OnUnityThread("message", (data) =>
            {
                AgentResponse response = data.GetValue<AgentResponse>();

                if(voice == true){
                    onVoice(response.output.audio);
                }

                onMessage(response.output.text);
            });

            socket.OnUnityThread("dictate", (data) =>
            {
                Debug.Log(data.GetValue<string>());
                onDictate(data.GetValue<string>());
            });


            socket.Connect();

        }

        public void disconnect() {
            socket.Disconnect();
        }

        public void send(string message) {
            if (socket == null) {
                Debug.Log("Error sending message, socket is null");
                return;
            } else {
                socket.EmitStringAsJSON("message", "{\"text\": \"" + message + "\", \"apiKey\": \"" + this.apiKey + "\", \"playerId\": \"" + this.playerId + "\"}");
            }
        }

        public void listen(){           
         
        }

        public string stopListening(){
            return null;
        }

        public void say(string audioId){
           
          
        }

        public void sendAudio(){

        }

        public IEnumerator PlayAudio(string url)
        {
          return null;
        }
      

    }
}
