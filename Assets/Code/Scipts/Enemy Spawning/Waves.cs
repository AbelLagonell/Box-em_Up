using UnityEngine;

public class Waves : MonoBehaviour {
    public Waves Instance { get; private set; }

    private void Start() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(gameObject);
        GameStatTracker.Instance.OnWaveChange += OnWaveChange;
    }

    private void OnDestroy() {
        GameStatTracker.Instance.OnWaveChange -= OnWaveChange;
    }

    private void OnWaveChange(int wave) { }
}