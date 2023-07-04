using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    [SerializeField] private Slider sliderGlobalVolume;
    [SerializeField] private Slider sliderMusicSound;
    [SerializeField] private Slider sliderSFXSound;
    [SerializeField] private AudioSource musicVolume;
    [SerializeField] private AudioSource[] sfxVolumes;

    private void Start()
    {
        Debug.Log("sound");
        sliderGlobalVolume.onValueChanged.AddListener(ChangeGlobalVolume);
        sliderMusicSound.onValueChanged.AddListener(ChangeMusicVolume);
        sliderSFXSound.onValueChanged.AddListener(ChangeSFXVolume);
    }

    private void OnEnable()
    {
        sliderGlobalVolume.value = GameSettingsScript.GlobalVolume*100;
        sliderMusicSound.value = GameSettingsScript.MusicVolume*100;
        sliderSFXSound.value = GameSettingsScript.SfxVolume*100;    }

    private void ChangeGlobalVolume(float percent)
    {
        GameSettingsScript.GlobalVolume = ( percent / 100);
        if(musicVolume)
            musicVolume.volume = GameSettingsScript.MusicVolume*GameSettingsScript.GlobalVolume;
        if(musicVolume)
            musicVolume.volume = GameSettingsScript.MusicVolume*GameSettingsScript.GlobalVolume;
    }    
    private void ChangeMusicVolume(float percent)
    {
        GameSettingsScript.MusicVolume = ( percent / 100);
        if(musicVolume)
            musicVolume.volume = GameSettingsScript.MusicVolume*GameSettingsScript.GlobalVolume;
    }    
    private void ChangeSFXVolume(float percent)
    {
        GameSettingsScript.SfxVolume = ( percent / 100);
            foreach (var sfxVolume in sfxVolumes)
            {
                if(sfxVolume)
                    sfxVolume.volume = GameSettingsScript.SfxVolume*GameSettingsScript.GlobalVolume;
            }
    }
}
