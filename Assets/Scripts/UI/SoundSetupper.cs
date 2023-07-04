using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSetupper : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource[] sfxSources;

    private void Start()
    {
        foreach (var sfxVolume in sfxSources)
        {
            if(sfxVolume)
                sfxVolume.volume = GameSettingsScript.SfxVolume*GameSettingsScript.GlobalVolume;
        }
        if(musicSource)
            musicSource.volume = GameSettingsScript.MusicVolume*GameSettingsScript.GlobalVolume;
    }
}
