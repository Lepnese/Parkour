using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    [SerializeField] private Sound[] sounds;

    private void Start ()
    {
        if (instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        gameObject.AddComponent<ONSPAudioSource>();
    }

    public void Play (string sound) {
        Sound s = Array.Find(sounds, item => item.name == sound);
        print(s.name);
        s.source.Play();
    }

    public void Stop(string sound) {
        Sound s = Array.Find(sounds, item => item.name == sound);
        s.source.Stop();
    }

}