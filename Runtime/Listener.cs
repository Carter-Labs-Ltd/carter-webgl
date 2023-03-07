using System.Collections.Generic;
using UnityEngine;
using System;


public class Listener : MonoBehaviour
{
    int frameRate = 16000;
    bool isRecording = false;
    AudioSource audioSource;
    List<float> tempRecording = new List<float>();
    bool microphoneAccess;

    public void Initialize(){

        // if class Microhpone exists, set microphoneAccess to True
        if (typeof(Microphone).IsClass)
        {
            microphoneAccess = true;
        }
        else
        {
            microphoneAccess = false;
        }
        

    }

    void Start(){
        Initialize();
        if(!microphoneAccess)
        {
            Debug.Log("Microphone access not granted");
            return;
        }
        Debug.Log("Listening...");
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = Microphone.Start(null, true, 1, 16000);
        audioSource.Play();
        Invoke("ResizeRecording", 1);
          
    }

    public void StartListening()
    {
        if(!microphoneAccess)
        {
            Debug.Log("Microphone access not granted");
            return;
        }
        if(!isRecording)
        {                     
            isRecording = true;
            audioSource.Stop();
            tempRecording.Clear();
            Microphone.End(null);
            audioSource.clip = Microphone.Start(null, true, 20, frameRate);
            Invoke("ResizeRecording", 1);
        } 
    }

    public string StopListening(){
        if(!microphoneAccess)
        {
            Debug.Log("Microphone access not granted");
            return null;
        }

        if (isRecording)
        {
            isRecording = false;
            Microphone.End(null);
            byte[] bytes = WavUtility.FromAudioClip(audioSource.clip);
            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        return null;
    }

    void ResizeRecording()
    {
        if(!microphoneAccess)
        {
            Debug.Log("Microphone access not granted");
            return;
        }
        if (isRecording)
        {
            int length = frameRate;
            float[] clipData = new float[length];
            audioSource.clip.GetData(clipData, 0);
            tempRecording.AddRange(clipData);
            Invoke("ResizeRecording", 1);
        }
    }

   

}