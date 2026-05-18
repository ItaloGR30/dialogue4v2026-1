using UnityEngine;
using System;

public class AudioPlayer : MonoBehaviour
{
    [Tooltip("Reference to an AudioCollection containing clips to choose from")]
    public AudioCollection myAudioCollection;

    [Tooltip("Index of the clip to play from the AudioCollection")]
    public int playIndex;

    [Tooltip("If true, the selected clip will be played on Start using the AudioManager singleton")]
    public bool playOnStart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playOnStart)
        {
            PlaySelected();
        }
    }

    // Plays the clip at the configured playIndex using the AudioManager singleton
    public void PlaySelected()
    {
        if (myAudioCollection == null)
        {
            Debug.LogWarning("AudioPlayer: No AudioCollection assigned.");
            return;
        }

        var list = myAudioCollection.AudioClipCollection;
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("AudioPlayer: AudioCollection has no clips.");
            return;
        }

        if (playIndex < 0 || playIndex >= list.Count)
        {
            Debug.LogWarning($"AudioPlayer: playIndex {playIndex} is out of range (0..{Math.Max(0, list.Count - 1)}).");
            return;
        }

        var clip = list[playIndex];
        if (clip == null)
        {
            Debug.LogWarning($"AudioPlayer: Clip at index {playIndex} is null.");
            return;
        }

        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioPlayer: AudioManager.Instance is null. Make sure an AudioManager exists in the scene.");
            return;
        }

        AudioManager.Instance.PlaySound(clip);
    }

    // Convenience: change index and play
    public void PlayIndex(int index)
    {
        playIndex = index;
        PlaySelected();
    }

    // Pause, Stop and Resume control using AudioManager
    public void Pause()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PauseSound();
    }

    public void Stop()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopSound();
    }

    public void Resume()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.ResumeSound();
    }

    // Helper for editor to know how many clips are available
    public int GetClipCount()
    {
        return myAudioCollection?.AudioClipCollection?.Count ?? 0;
    }
}
