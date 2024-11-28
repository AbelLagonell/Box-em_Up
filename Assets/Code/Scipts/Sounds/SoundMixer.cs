using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixer : BaseAudio {
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private AudioClips audioClips;

    public void Start() {
        if (PlayerPrefs.HasKey("masterVolume")) {
            LoadVolume();
        } else {
            SetMusicVolume();
            SetSfxVolume();
            SetUIVolume();
            SetMasterVolume();
        }
    }

    public void SetMusicVolume() {
        var volume = musicSlider.value;
        mixer.SetFloat("music", volume);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetMasterVolume() {
        var volume = masterSlider.value;
        mixer.SetFloat("master", volume);
        PlayerPrefs.SetFloat("masterVolume", volume);
    }

    public void SetSfxVolume() {
        var volume = sfxSlider.value;
        mixer.SetFloat("sfx", volume);
        PlayerPrefs.SetFloat("sfxVolume", volume);
        PlayAudio(audioClips.audioClips[0]);
    }

    public void SetUIVolume() {
        var volume = uiSlider.value;
        mixer.SetFloat("ui", volume);
        PlayerPrefs.SetFloat("uiVolume", volume);
        PlayAudio(audioClips.audioClips[1]);
    }

    public void LoadVolume() {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        musicSlider.value  = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value    = PlayerPrefs.GetFloat("sfxVolume");
        uiSlider.value     = PlayerPrefs.GetFloat("uiVolume");

        SetMusicVolume();
        SetMasterVolume();
        SetSfxVolume();
        SetUIVolume();
    }
}