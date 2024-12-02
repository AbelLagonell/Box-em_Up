using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI multiplier;
    [SerializeField] private TextMeshProUGUI wave;
    [SerializeField] private Slider healthObject;
    [SerializeField] private GameObject abilityCooldown;

    private float _maxHealth;
    private Slider _ability;

    public static StatDisplay Instance { get; private set; }

    private void Start() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Subscribe to the events
        GameStatTracker.Instance.OnWaveChange          += OnWaveChanged;
        GameStatTracker.Instance.OnScoreChanged        += OnScoreChanged;
        GameStatTracker.Instance.OnMultiplierChanged   += OnMultiplierChanged;
        GameStatTracker.Instance.OnPlayerHealthChanged += OnPlayerHealthChanged;
        MainCharacter.Instance.OnChargeChange          += OnChargeChanged;

        _ability = abilityCooldown.GetComponent<Slider>();
    }

    private void OnChargeChanged(float obj) {
        abilityCooldown.SetActive(true);
        _ability.value = Mathf.Clamp(obj, 0, 3f);
    }

    private void FixedUpdate() {
        if (SceneManager.GetActiveScene().buildIndex == 4) Destroy(gameObject);
    }

    private void OnDestroy() {
        // Unsubscribe from the events
        GameStatTracker.Instance.OnWaveChange          -= OnWaveChanged;
        GameStatTracker.Instance.OnScoreChanged        -= OnScoreChanged;
        GameStatTracker.Instance.OnMultiplierChanged   -= OnMultiplierChanged;
        GameStatTracker.Instance.OnPlayerHealthChanged -= OnPlayerHealthChanged;
        MainCharacter.Instance.OnChargeChange          -= OnChargeChanged;
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
        if (newHealth > _maxHealth) {
            _maxHealth            = newHealth;
            healthObject.maxValue = _maxHealth;
        }

        healthObject.value = newHealth;
    }
}