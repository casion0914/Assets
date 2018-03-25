using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneManager : MonoBehaviour {

    private static MicrophoneManager m_instance;

    private static string[] micArray = null; //录音设备列表
    private AudioClip audioClip;
    private DateTime beginTime;

    public float sensitivity = 100;
    public float loudness = 0;

    const int HEADER_SIZE = 44;
    const int RECORD_TIME = 10;
    const int RECORD_RATE = 24000; //录音采样率

    //void Awake()
    //{
    //    m_instance = this;
    //}
    public static MicrophoneManager GetInstance()
    {
        if (m_instance == null)
        {
            micArray = Microphone.devices;
            if (micArray.Length == 0)
            {
				Debug.Log("no mic device");
            }
            foreach (string deviceStr in Microphone.devices)
            {
                Debug.Log("device name = " + deviceStr);
            }
            GameObject micManager = new GameObject("MicManager");
            m_instance = micManager.AddComponent<MicrophoneManager>();
        }
        return m_instance;
    }

    /// <summary>
    /// 开始录音
    /// </summary>
    public void StartRecord()
    {
        if (micArray.Length == 0)
        {
            Debug.Log("No Record Device!");
            return;
        }
        Microphone.End(null);//录音时先停掉录音，录音参数为null时采用默认的录音驱动
        beginTime = DateTime.Now;
        audioClip = Microphone.Start(null, false, RECORD_TIME, RECORD_RATE);
        while (!(Microphone.GetPosition(null) > 0))
        {
        }
        Debug.Log("StartRecord");
    }

    /// <summary>
    /// 停止录音
    /// </summary>
    public void StopRecord()
    {
        if (micArray.Length == 0)
        {
            Debug.Log("No Record Device!");
            return;
        }
        if (!Microphone.IsRecording(null))
        {
            return;
        }
        Microphone.End(null);
        Debug.Log("StopRecord");

    }

    /// <summary>
    ///播放录音 
    /// </summary>
    public void PlayRecord()
    {
        PlayRecord(audioClip);
    }
    public void PlayRecord(AudioClip clip)
    {
        PlayRecord(clip, Vector3.zero);
    }

    public void PlayRecord(AudioClip clip, Vector3 pos)
    {
        if (clip == null)
        {
            Debug.Log("audioClip is null");
            return;
        }
        AudioSource.PlayClipAtPoint(clip, pos);
        Debug.Log("PlayRecord");
    }


    public byte[] Save()
    {
		if (audioClip == null) {
			Debug.Log("audioClip is null");
			return null;
		}
        return WavUtility.FromAudioClip(audioClip);
    }

    //public void Save(string path, string filename)
    //{
    //    Save(audioClip, path, filename);
    //}

    //public void Save(AudioClip clip, string path, string filename)
    //{
    //    if (clip == null)
    //    {
    //        Debug.Log("clip is null,can't be saved");
    //        return;
    //    }
    //    WavUtility.FromAudioClip(clip, path, filename, true);
    //}

    public AudioClip Read(string path)
    {
        return WavUtility.ToAudioClip(path);
    }

    public AudioClip Read(byte[] data)
    {
        return WavUtility.ToAudioClip(data);
    }
}
