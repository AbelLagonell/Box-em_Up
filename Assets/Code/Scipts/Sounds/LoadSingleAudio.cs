using UnityEngine;

public class LoadSingleAudio : BaseAudio {
    [SerializeField] protected AudioClip[] audioClips;

    // Start is called before the first frame update
    private void Start() {
        if (onStart)
            PlayAudio(audioClips[Random.Range(0, audioClips.Length)]);
    }

    public void PlayAsync() {
        PlayAudio(audioClips[Random.Range(0, audioClips.Length)]);
    }

}