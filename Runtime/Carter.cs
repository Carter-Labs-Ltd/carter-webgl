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
        Listener listener;


        public void StartListening(){
            listener = gameObject.AddComponent<Listener>();
        }


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


        // AUDIO //

        public void listen(){
            listener.StartListening();
        }

        public string stopListening(){
            return listener.StopListening();
        }

        public void say(string audioId){
            StartCoroutine(PlayAudio(url + "/speak/" + audioId));
        }

        public void sendAudio(){

            string base64 = stopListening();

            if(base64 != null){
                Debug.Log("Sending audio");

                if (socket == null) {
                    Debug.Log("Error sending message, socket is null");
                    return;
                } else {
                    socket.EmitStringAsJSON("voice_message", "{\"audio\": \"" + base64 + "\", \"apiKey\": \"" + this.apiKey + "\", \"playerId\": \"" + this.playerId + "\"}");
                }
            }
        }

        public IEnumerator PlayAudio(string url)
        {
            Debug.Log("Playing audio from: " + url);
        
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    // Debug.Log("Playing audio");
                    AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                    AudioSource audioSource2 = gameObject.AddComponent<AudioSource>();
                    audioSource2.clip = myClip;
                    audioSource2.Play();
                    www.Dispose();
                }
            }
        }


    }
}
