using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {
    public enum SoundType {
        Music,
        SFX,
        UI,
        Master
    }

    public List<SoundSettings> soundSettings;

    private readonly Dictionary<string, AudioSource> _activeSources = new();
    private AudioMixer _masterMixer;

    public static SoundManager Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public AudioSource PlaySound(
        AudioClip clip,
        SoundType soundType,
        bool shouldFadeIn = false,
        float customVolumeOverride = -1f,
        bool loop = false,
        float loopStartTime = 0f,
        float loopEndTime = -1f) {
        if (clip == null) return null;

        var settings = soundSettings.Find(s => s.type == soundType);

        var volume = 1f;
        if (settings != null)
            volume = customVolumeOverride >= 0
                ? customVolumeOverride
                : settings.volumeScale;

        var soundObject = new GameObject($"Sound_{clip.name}");
        var audioSource = soundObject.AddComponent<AudioSource>();

        if (settings?.mixerGroup != null) audioSource.outputAudioMixerGroup = settings.mixerGroup;

        audioSource.clip   = clip;
        audioSource.volume = shouldFadeIn ? 0f : volume;
        audioSource.loop   = loop;

        // Setup looping if specified
        if (loop && loopEndTime > loopStartTime)
            StartCoroutine(ManageLoopedSound(audioSource, clip, loopStartTime, loopEndTime));

        audioSource.Play();

        _activeSources[soundObject.name] = audioSource;

        if (shouldFadeIn) StartCoroutine(FadeInSound(audioSource, volume, 1f));

        // Only destroy if not looping
        if (!loop) StartCoroutine(DestroyAfterPlay(soundObject, clip.length));

        return audioSource;
    }

    private IEnumerator ManageLoopedSound(AudioSource source, AudioClip clip, float loopStart, float loopEnd) {
        while (source.isPlaying) {
            // Wait until we're near the loop end
            yield return new WaitUntil(() => source.time >= loopEnd);

            // Immediately jump back to loop start
            source.time = loopStart;
        }
    }

    public void PlayClipSegment(
        AudioClip clip,
        float startTime,
        float endTime,
        SoundType soundType,
        float customVolumeOverride = -1f) {
        if (clip == null || startTime >= endTime || endTime > clip.length) return;

        // Find the corresponding sound settings
        var settings = soundSettings.Find(s => s.type == soundType);

        // Determine volume
        var volume = 1f; // Default to full volume if no settings found
        if (settings != null)
            // Use custom override if provided, otherwise use volume scale
            volume = customVolumeOverride >= 0
                ? customVolumeOverride
                : settings.volumeScale;

        var soundObject = new GameObject($"SoundSegment_{clip.name}");
        var audioSource = soundObject.AddComponent<AudioSource>();

        if (settings?.mixerGroup != null) audioSource.outputAudioMixerGroup = settings.mixerGroup;

        audioSource.clip   = clip;
        audioSource.time   = startTime;
        audioSource.volume = volume;
        audioSource.Play();

        _activeSources[soundObject.name] = audioSource;

        var duration = endTime - startTime;
        StartCoroutine(DestroyAfterPlay(soundObject, duration));
    }

    private IEnumerator FadeInSound(AudioSource source, float targetVolume, float fadeDuration) {
        var currentTime = 0f;
        while (currentTime < fadeDuration) {
            currentTime   += Time.deltaTime;
            source.volume =  Mathf.Lerp(0f, targetVolume, currentTime / fadeDuration);
            yield return null;
        }
    }

    private IEnumerator DestroyAfterPlay(GameObject soundObject, float delay) {
        yield return new WaitForSeconds(delay);

        if (soundObject == null) yield break;
        _activeSources.Remove(soundObject.name);
        Destroy(soundObject);
    }

    public void StopAllSounds() {
        foreach (var source in _activeSources.Values.Where(source => source != null)) {
            source.Stop();
            Destroy(source.gameObject);
        }

        _activeSources.Clear();
    }

    public void CrossfadeSounds(
        AudioClip newClip,
        SoundType soundType,
        float crossfadeDuration = 1f,
        bool loop = false,
        float loopStartTime = 0f,
        float loopEndTime = -1f) {
        // Find currently playing source of this sound type
        var currentSource = FindCurrentlyPlayingSource(soundType);

        if (currentSource != null)
            // Fade out current source
            StartCoroutine(FadeOutAndReplaceSoundCoroutine(
                               currentSource,
                               newClip,
                               soundType,
                               crossfadeDuration,
                               loop,
                               loopStartTime,
                               loopEndTime
                           ));
        else
            // If no current source, just play new sound
            PlaySound(newClip, soundType, false, -1f, loop, loopStartTime, loopEndTime);
    }

    private AudioSource FindCurrentlyPlayingSource(SoundType soundType) {
        return _activeSources.Values.FirstOrDefault(source => source != null && source.isPlaying &&
                                                              source.outputAudioMixerGroup.name ==
                                                              soundType.ToString());
    }

    private IEnumerator FadeOutAndReplaceSoundCoroutine(
        AudioSource currentSource,
        AudioClip newClip,
        SoundType soundType,
        float crossfadeDuration,
        bool loop,
        float loopStartTime,
        float loopEndTime) {
        var settings = soundSettings.Find(s => s.type == soundType);
        var volume   = settings?.volumeScale ?? 1f;

        // Fade out current source
        var startVolume = currentSource.volume;
        var currentTime = 0f;
        while (currentTime < crossfadeDuration) {
            currentTime += Time.deltaTime;
            if (currentSource == null) yield break;
            currentSource.volume = Mathf.Lerp(startVolume, 0f, currentTime / crossfadeDuration);

            yield return null;
        }

        if (currentSource == null) yield return null;

        // Stop and destroy current source
        currentSource.Stop();
        _activeSources.Remove(currentSource.gameObject.name);
        Destroy(currentSource.gameObject);

        // Play new source with fade in
        PlaySound(
            newClip,
            soundType,
            true,
            volume,
            loop,
            loopStartTime,
            loopEndTime
        );
    }

    public void FadeOutSound(SoundType soundType, float fadeDuration = 1f) {
        var sourceToFadeOut = FindCurrentlyPlayingSource(soundType);

        if (sourceToFadeOut != null) StartCoroutine(FadeOutSoundCoroutine(sourceToFadeOut, fadeDuration));
    }

    public IEnumerator FadeOutSoundCoroutine(AudioSource source, float fadeDuration) {
        var startVolume = source.volume;
        var currentTime = 0f;

        while (currentTime < fadeDuration) {
            currentTime   += Time.deltaTime;
            source.volume =  Mathf.Lerp(startVolume, 0f, currentTime / fadeDuration);
            yield return null;
        }

        source.Stop();
        _activeSources.Remove(source.gameObject.name);
        Destroy(source.gameObject);
    }

    [Serializable]
    public class SoundSettings {
        public SoundType type;
        public AudioMixerGroup mixerGroup;
        [Range(0f, 1f)] public float volumeScale = 1f; // Multiplier for final volume
    }
}