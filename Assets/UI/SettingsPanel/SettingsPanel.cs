using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField]
    private GameConfig gameConfig;

    [SerializeField]
    private SliderSetting masterVolume;
    [SerializeField]
    private SliderSetting musicVolume;
    [SerializeField]
    private SliderSetting sfxVolume;

    void Start()
    {
        float tmp;
        gameConfig.mixer.GetFloat("MasterVolume", out tmp);
        masterVolume.SetValue(tmp);
        gameConfig.mixer.GetFloat("MusicVolume", out tmp);
        musicVolume.SetValue(tmp);
        gameConfig.mixer.GetFloat("SFXVolume", out tmp);
        sfxVolume.SetValue(tmp);

        masterVolume.GetOnValueChanged().AddListener(value => ApplySettings());
        musicVolume.GetOnValueChanged().AddListener(value => ApplySettings());
        sfxVolume.GetOnValueChanged().AddListener(value => ApplySettings());
    }

    private void ApplySettings()
    {
        gameConfig.mixer.SetFloat("MasterVolume", masterVolume.GetValue());
        gameConfig.mixer.SetFloat("MusicVolume", musicVolume.GetValue());
        gameConfig.mixer.SetFloat("SFXVolume", sfxVolume.GetValue());
    }
}
