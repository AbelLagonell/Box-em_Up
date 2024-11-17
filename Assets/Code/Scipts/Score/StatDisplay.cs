using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI multiplier;
    [SerializeField] private TextMeshProUGUI wave;
    [SerializeField] private Slider healthObject;

    private float maxHealth;

    private void Start() {
        // Subscribe to the events
        GameStatTracker.Instance.OnWaveChange += OnWaveChanged;
        GameStatTracker.Instance.OnScoreChanged += OnScoreChanged;
        GameStatTracker.Instance.OnMultiplierChanged += OnMultiplierChanged;
        GameStatTracker.Instance.OnPlayerHealthChanged += OnPlayerHealthChanged;

        healthObject = GetComponentInChildren<Slider>();
    }

    private void OnDestroy() {
        // Unsubscribe from the events
        GameStatTracker.Instance.OnWaveChange -= OnWaveChanged;
        GameStatTracker.Instance.OnScoreChanged -= OnScoreChanged;
        GameStatTracker.Instance.OnMultiplierChanged -= OnMultiplierChanged;
        GameStatTracker.Instance.OnPlayerHealthChanged -= OnPlayerHealthChanged;
    }

    private void OnScoreChanged(int newScore) {
        score.text = newScore.ToString();
    }

    private void OnMultiplierChanged(int newMultiplier) {
        multiplier.text = "x" + newMultiplier;
    }

    private void OnWaveChanged(int newWave) {
        wave.text = "Wave: " + newWave;
    }

    private void OnPlayerHealthChanged(int newHealth) {
        if (newHealth > maxHealth) {
            maxHealth = newHealth;
            healthObject.maxValue = maxHealth;
        }

        healthObject.value = newHealth;
    }
}