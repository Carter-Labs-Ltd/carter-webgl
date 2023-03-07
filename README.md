<img src="https://www.carterapi.com/carter-full-white-transparent.svg" style="width: 200px"/>

# Carter Unity

Make your game characters talk in minutes with next generation artificial intelligence. This package is still under active development and is not yet ready for production use.

## Installation

This package uses the web socket protocol under the hood. To get started you will first need to install [SocketIOUnity](https://github.com/itisnajim/SocketIOUnity):

To do this, go to: Window -> Package Manager -> Click the (+) add package from git URL -> Paste:

`https://github.com/itisnajim/SocketIOUnity.git`

Then install the Carter Package! To do this, go to: Window -> Package Manager -> Click the (+) add package from git URL -> Paste:

`https://github.com/Carter-Labs-Ltd/carter-unity.git`

## Usage

Getting started with Carter is easy. First configure your agent [here](https://studio.carterlabs.ai). Once you've got your agent's API Key come back here and add Carter to your game as below.

### Text Based Chat

```...
using Carter;

public class MyExampleScript : MonoBehaviour
{
    Agent mika;

    // Define Event Handlers //

    void onConnect(){
        Debug.Log("Connected to agent!");
    }

    void onDisconnect(){
        Debug.Log("Disconnected from agent!");
    }

    void onMessage(string message){
        Debug.Log("Message from agent: " + message);
    }

    void onVoice(string audioId){
        // Make your agent speak the response!
        mika.say(audioId);
    }

    void Start()
    {
        mika = gameObject.AddComponent<Agent>();
        mika.init("API_KEY", "PLAYER_ID", "https://dev-api.carterapi.com", onConnect, onDisconnect, onMessage, onVoice);
    }

    void Update()
    {
        // ... on button click etc
        mika.send("Hello");


        // ... OTHER OPTIONS

        mika.connected // true/false

        mika.connect(); // reconnect

        mika.disconnect(); // disconnect
    }
}
```

### Voice Based Chat

```...
using Carter;

public class MyExampleScript : MonoBehaviour
{
    Agent mika;

    // Define Event Handlers //

    void onConnect(){
        Debug.Log("Connected to agent!");
    }

    void onDisconnect(){
        Debug.Log("Disconnected from agent!");
    }

    void onMessage(string message){
        Debug.Log("Message from agent: " + message);
    }

    void onVoice(string audioId){
        // Make your agent speak the response!
        mika.say(audioId);
    }

    void Start()
    {
        mika = gameObject.AddComponent<Agent>();
        mika.init("API_KEY", "PLAYER_ID", "https://dev-api.carterapi.com", onConnect, onDisconnect, onMessage, onVoice);
        mika.StartListening(); //activate microphone access
    }

    void Update()
    {
        if (Input.GetKey("space"))
        {
            // start recording audio (won't trigger more than once before stopRecording() is called)
            mika.listen();
        }

        if (!Input.GetKey("space"))
        {
            // stop recording audio and send to agent
            mika.sendAudio();
        }

    }
}
```

[Further Documentation](https://carterapi.gitbook.io/carter-docs/)

## Join The Community

Join our awesome [Discord community](https://discord.gg/YqWwCVU8UH).

Follow us on [TikTok](https://www.tiktok.com/@carterlabs)

Binge & Learn on [YouTube](https://www.youtube.com/@Carter_Labs)
