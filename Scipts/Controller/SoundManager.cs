using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject v = new GameObject("SoundManager");
                v.transform.parent = GameObject.Find("MainController").transform;
                _instance = v.AddComponent<SoundManager>();
                bgAudioSource = v.AddComponent<AudioSource>();
                bgAudioSource.loop = true;
                audioSourceEffect = v.AddComponent<AudioSource>();
            }
            return _instance;
        }
    }

    public static SoundManager _instance;
    private SoundManager()
    {

    }
    private Dictionary<string, AudioClip> _soundDictionary;
    private AudioSource[] audioSources;

    private static AudioSource bgAudioSource;
    private static AudioSource audioSourceEffect;

    public AudioClip[] audioArray;

    void Start()
    {
        //SoundManager.instance = this;

        _soundDictionary = new Dictionary<string, AudioClip>();

        //本地加载 
        AudioClip[] audioArray = Resources.LoadAll<AudioClip>("AudioCilp");


        foreach (AudioClip item in audioArray)
        {
            _soundDictionary.Add(item.name, item);
        }

        PlayBGaudio("sound_roombg");
    }

    //播放背景音乐
    public void PlayBGaudio(string audioName)
    {
        if (_soundDictionary.ContainsKey(audioName))
        {
            bgAudioSource.clip = _soundDictionary[audioName];
            bgAudioSource.volume = 0;
            bgAudioSource.Play();
        }
    }
    //播放音效
    public void PlayAudioEffect(string audioEffectName)
    {
        if (_soundDictionary.ContainsKey(audioEffectName))
        {
            audioSourceEffect.clip = _soundDictionary[audioEffectName];
            audioSourceEffect.Play();
        }
    }

    public void StopPlayAudioEffect()
    {
        audioSourceEffect.Stop();
    }
}
