using UnityEngine;

public class BaseAudio : MonoBehaviour {
    [Header("Sound Manager")] public SoundManager.SoundType soundType;
    public bool shouldFadeIn;
    public bool loop;
    public bool changeVolume;
    [Range(0, 1f)] public float volumeOverride;
    public bool onStart = true;

    protected AudioSource PlayAudio(AudioClip clip) {
        return SoundManager.Instance?.PlaySound(clip, soundType, shouldFadeIn, changeVolume ? volumeOverride : -1f,
                                                loop);
    }
}