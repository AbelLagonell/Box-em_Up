using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct AudioClips {
    public AudioClip[] audioClips;

    public AudioClip GetAudioClip() {
        return audioClips[Random.Range(0, audioClips.Length)];
    }
}

public class LoadMultAudio : BaseAudio {
    public int condition;
    public AudioClips[] audioClipsVector;

    private void Start() {
        var clip = audioClipsVector[condition].GetAudioClip();
        if (onStart) PlayAudio(clip);
    }

    public void PlayAsync(int index = -1) {
        var clip = audioClipsVector[index == -1 ? condition : index].GetAudioClip();
        PlayAudio(clip);
    }


    public void PlayFadeOut(int index = -1) {
        var clip = audioClipsVector[index == -1 ? condition : index].GetAudioClip();
        StartCoroutine(SoundManager.Instance.FadeOutSoundCoroutine(PlayAudio(clip), 1F));
    }
}