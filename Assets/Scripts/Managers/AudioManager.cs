using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    [SerializeField] private Sound[] sounds;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
        }
    }

    public void Play (string sound, GameObject target) {
        Sound s = Array.Find(sounds, item => item.name == sound);
        s.source = CreateSource(s, target);
        s.source.Play();
    }

    private AudioSource CreateSource(Sound s, GameObject target) {
        var src = target.GetComponent<AudioSource>();
        if (src) return src;
        
        src = target.AddComponent<AudioSource>();
        
        src.clip = s.clip;
        src.volume = s.volume;
        src.pitch = s.pitch;
        src.loop = s.loop;
        src.playOnAwake = false;

        return src;
    }

    public void Stop(string sound) {
        Sound s = Array.Find(sounds, item => item.name == sound);
        s.source.Stop();
    }
}