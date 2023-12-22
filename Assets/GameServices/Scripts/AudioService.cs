using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioService : ServiceBase
{

    [SerializeField] private AudioMixer AudioMixer;
    [SerializeField] private AudioSource _SFXAudioSource;
    [SerializeField] private AudioSource _musicAudioSource;

    private float MusicVolume = 1f;
    private float SFXVolume = 1f;

    protected override void Initialize()
    {
        Debug.Log("Initialized Game Audio Service");
        LoadAudioSettings();
    }

    public void Play(AudioClip clip)
    {
        if (clip != null)
        {
            _SFXAudioSource.PlayOneShot(clip);
        }
    }
    public float GetMusicVolume() { return MusicVolume; }

    public float GetSFXVolume() { return SFXVolume; }

    public void PlayMusic(AudioClip clip)
    {
        if (clip != null)
        {
            _musicAudioSource.clip = clip;
            _musicAudioSource.Play();
        }
    }

    public void OnMusicVolumeChange(float newVolume)
    {
        MusicVolume = newVolume;
        AudioMixer.SetFloat("MusicVolume", VolumeTodB(newVolume));
        SaveAudioSettings();
    }

    public void OnSFXVolumeChange(float newVolume)
    {
        SFXVolume = newVolume;
        AudioMixer.SetFloat("SFXVolume", VolumeTodB(newVolume));
        SaveAudioSettings();
    }

    private float VolumeTodB(float volume)
    {
        if (volume <= 0) volume = 0.0001f;
        return MathF.Log10(volume) * 20;
    }

    private void LoadAudioSettings()
    {
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f); 
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f); 

        AudioMixer.SetFloat("MusicVolume", VolumeTodB(MusicVolume));
        AudioMixer.SetFloat("SFXVolume", VolumeTodB(SFXVolume));
    }

    public void StopSFXAudio()
    {
        _SFXAudioSource.Stop();
    }

    private void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
        PlayerPrefs.Save();
    }
}
