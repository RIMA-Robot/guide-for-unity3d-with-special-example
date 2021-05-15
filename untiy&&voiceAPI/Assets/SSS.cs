using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSS : MonoBehaviour {

    AudioSource _audio;
    AudioSource audio
    {
        get
        {
            if (_audio == null)
            {
                _audio = gameObject.AddComponent<AudioSource>();
            }
            return _audio;
        }
    }

    void Start()
    {
        string[] ms = Microphone.devices;
        deviceCount = ms.Length;
        if (deviceCount == 0)
        {
            Log("no microphone found");
        }
    }

    string sLog = "";
    void Log(string log)
    {
        sLog += log;
        sLog += "\r\n";
    }
    int deviceCount;
    string sFrequency = "10000";
    void OnGUI()
    {
        if (deviceCount > 0)
        {
            GUILayout.BeginHorizontal();
            if (!Microphone.IsRecording(null) && GUILayout.Button("Start", GUILayout.Height(Screen.height / 20), GUILayout.Width(Screen.width / 5)))
            {
                StartRecord();
            }
            if (Microphone.IsRecording(null) && GUILayout.Button("Stop", GUILayout.Height(Screen.height / 20), GUILayout.Width(Screen.width / 5)))
            {
                StopRecord();
            }
            if (!Microphone.IsRecording(null) && GUILayout.Button("Play", GUILayout.Height(Screen.height / 20), GUILayout.Width(Screen.width / 5)))
            {
                PlayRecord();
            }
            if (!Microphone.IsRecording(null) && GUILayout.Button("Print", GUILayout.Height(Screen.height / 20), GUILayout.Width(Screen.width / 5)))
            {
                PrintRecord();
            }
            sFrequency = GUILayout.TextField(sFrequency, GUILayout.Width(Screen.width / 5), GUILayout.Height(Screen.height / 20));
            GUILayout.EndHorizontal();
        }
        GUILayout.Label(sLog);
    }
    void StartRecord()
    {
        audio.Stop();
        audio.loop = false;
        audio.mute = true;
        audio.clip = Microphone.Start(null, false, 10, int.Parse(sFrequency));
        while (!(Microphone.GetPosition(null) > 0))
        {
        }
        audio.Play();
        Log("StartRecord");
    }
    void StopRecord()
    {
        if (!Microphone.IsRecording(null))
        {
            return;
        }
        Microphone.End(null);
        audio.Stop();
    }
    void PrintRecord()
    {
        if (Microphone.IsRecording(null))
        {
            return;
        }
        byte[] data = GetClipData();
        string slog = "total length:" + data.Length + " time:" + audio.time;
        Log(slog);
    }
    void PlayRecord()
    {
        if (Microphone.IsRecording(null))
        {
            return;
        }
        if (audio.clip == null)
        {
            return;
        }
        audio.mute = false;
        audio.loop = false;
        audio.Play();
    }
    public byte[] GetClipData()
    {
        if (audio.clip == null)
        {
            Debug.Log("GetClipData audio.clip is null");
            return null;
        }

        float[] samples = new float[audio.clip.samples];

        audio.clip.GetData(samples, 0);


        byte[] outData = new byte[samples.Length * 2];

        int rescaleFactor = 32767;

        for (int i = 0; i < samples.Length; i++)
        {
            short temshort = (short)(samples[i] * rescaleFactor);

            byte[] temdata = System.BitConverter.GetBytes(temshort);

            outData[i * 2] = temdata[0];
            outData[i * 2 + 1] = temdata[1];


        }
        if (outData == null || outData.Length <= 0)
        {
            Debug.Log("GetClipData intData is null");
            return null;
        }
        return outData;
    }

}