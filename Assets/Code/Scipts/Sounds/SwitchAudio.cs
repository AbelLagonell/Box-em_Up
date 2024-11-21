using UnityEngine;

public class SwitchAudio : BaseAudio {
    public AudioClip primaryClip;
    public AudioClip secondaryClip;
    private bool _cClip;

    private void Start() {
        PlayAudio(primaryClip);
    }

    public void SwitchAudioClips() {
        SoundManager.Instance.CrossfadeSounds(_cClip ? primaryClip : secondaryClip, soundType, loop: loop);
        _cClip = !_cClip;
    }
}