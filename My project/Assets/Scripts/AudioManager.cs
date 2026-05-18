using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource systemSource;
    private List<AudioSource> activeSources;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            systemSource = gameObject.GetComponent<AudioSource>();
            activeSources = new List<AudioSource>();
        }
        else
        {
            {
                Destroy(gameObject);
            }
        }
    }
    
    private void Start()
    {
        
    }
    
    //Funcoes de gerenciamento de audio
    public void PlaySound(AudioClip clip)
    {
            systemSource.Stop();
            systemSource.clip = clip;
            systemSource.Play();
    }

    public void StopSound()
    {
        systemSource.Stop();
    }

    public void PauseSound()
    {
        systemSource.Pause();
    }

    public void ResumeSound()
    {
        systemSource.UnPause();
    }
    
    public void PlayOneShot(AudioClip clip)
    {
        systemSource.PlayOneShot(clip);
    }
    
    //funções de gerenciamente de audio 3d
    public void playSound(AudioClip clip, AudioSource source)
    {
        if(!activeSources.Contains(source)) activeSources.Add(source);
        source.Stop();
        source.clip = clip;
        source.Play();
    }

    public void StopSound(AudioSource source)
    {
        systemSource.Stop();
        activeSources.Remove(source);
    }

    public void PauseSound(AudioSource source)
    {
        source.Pause();
    }

    public void ResumeSound(AudioSource source)
    {
        source.UnPause();
    }

    public void PlayOneShot(AudioClip clip, AudioSource source)
    {
        source.PlayOneShot(clip);
    }
}

