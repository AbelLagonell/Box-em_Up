using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class StatDisplay : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI multiplier;
    [SerializeField] private GameObject healthObject;

    private float maxHealth;

    private void Start() {
        // Subscribe to the events
        GameStatTracker.Instance.OnScoreChanged        += OnScoreChanged;
        GameStatTracker.Instance.OnMultiplierChanged   += OnMultiplierChanged;
        GameStatTracker.Instance.OnPlayerHealthChanged += OnPlayerHealthChanged;
    }

    private void OnDestroy() {
        // Unsubscribe from the events
        GameStatTracker.Instance.OnScoreChanged        -= OnScoreChanged;
        GameStatTracker.Instance.OnMultiplierChanged   -= OnMultiplierChanged;
        GameStatTracker.Instance.OnPlayerHealthChanged -= OnPlayerHealthChanged;
    }

    private void OnScoreChanged(int newScore) {
        score.text = newScore.ToString();
    }

    private void OnMultiplierChanged(int newMultiplier) {
        multiplier.text = "x" + newMultiplier;
    }

    private void OnPlayerHealthChanged(int newHealth) {
        if (newHealth > maxHealth) {
            maxHealth                                     = newHealth;
            healthObject.GetComponent<Slider>().highValue = maxHealth;
        }

        healthObject.GetComponent<Slider>().value = newHealth;
    }
}