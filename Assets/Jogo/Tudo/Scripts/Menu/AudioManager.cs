using UnityEngine.Audio;
using System;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public AudioMixer audioMixer;
    public static AudioManager instance;
   
     void Awake()
    {
        if(instance == null)
            instance = this;
        else{
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;


        }
    }

     void Start(){
        Play("Background");
    }


     public void Play(string name)
    {
       Sound s = Array.Find(sounds , sound => sound.name == name);
       if(s == null){
           Debug.LogWarning("Efeito:" + name + " Năo encontrado");
           return;
       }

       s.source.Play();
    }

        public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Efeito: " + name + " năo encontrado");
            return;
        }

        s.source.Stop();
    }

    public void SetVolume(string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Efeito: " + name + " năo encontrado");
            return;
        }

        s.source.volume = volume;
    }
    public void SetMasterVolume(float volume)
    {
        // O volume do mixer vai de -80 dB (mudo) até 0 dB (máximo)
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void MuteMaster(bool isMuted)
    {
        if (isMuted)
            audioMixer.SetFloat("MasterVolume", -80f); // Mudo
        else
        {
            float volume = PlayerPrefs.GetFloat("GameVolume", 0.7f);
            SetMasterVolume(volume);
        }
    }


}
