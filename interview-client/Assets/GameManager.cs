using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using NativeWebSocket;
using UnityEngine.XR;

public enum ToServerMessageType
{
    Initialize = 0
}

public enum ToClientMessageType
{
    Initialize = 0
}

public class GameManager : MonoBehaviour
{
    WebSocket websocket;

    private AudioClip micClip;
    int sampleWindow = 128;
    private string micDevice;

    [HideInInspector]
    public static float micLoudness;

    public static string uid;
    public static bool isVR = false;

    private bool RunningInVR()
    {
        var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
        foreach (var xrDisplay in xrDisplaySubsystems)
        {
            if (xrDisplay.running)
            {
                return true;
            }
        }
        return false;
    }

    async void Start()
    {
        uid = SystemInfo.deviceUniqueIdentifier;
        isVR = RunningInVR();

        micDevice = Microphone.devices[0];
        micClip = Microphone.Start(null, true, 1, 44100);

        string url = Debug.isDebugBuild ? "ws://localhost:8080" : "ws://localhost:8080";

        websocket = new WebSocket(url);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");

            SendMessage(ToServerMessageType.Initialize, "Hello World!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
        };

        // waiting for messages
        await websocket.Connect();
    }

    public async void SendMessage(ToServerMessageType type, string message)
    {
        await websocket.SendText("{\"uid\":\"" + uid + "\",\"type\":" + (int)type + ",\"message\":\"" + message + "\"}");
    }

    float LevelMax()
    {
        float levelMax = 0;
        float[] waveData = new float[sampleWindow];
        int micPosition = Microphone.GetPosition(null) - (sampleWindow + 1); // null means the first microphone
        if (micPosition < 0) return 0;
        micClip.GetData(waveData, micPosition);
        // Getting a peak on the last 128 samples
        for (int i = 0; i < sampleWindow; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak)
            {
                levelMax = wavePeak;
            }
        }
        return levelMax;
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif

        micLoudness = LevelMax();
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

}