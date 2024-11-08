using System;
using UnityEngine;

public class GameStatTracker : MonoBehaviour {
    public GameStats CurStats;
    public static GameStatTracker Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        CurStats.playTime = Time.timeSinceLevelLoad;
    }

    public void AddScore(int score) {
        CurStats.totalScore += score * CurStats.currentMultiplier;
        OnScoreChanged?.Invoke(CurStats.totalScore);
    }

    public void IncrementMultiplier() {
        CurStats.currentMultiplier++;
        OnMultiplierChanged?.Invoke(CurStats.currentMultiplier);
    }

    public void ResetMultiplier() {
        CurStats.currentMultiplier = 1;
        OnMultiplierChanged?.Invoke(CurStats.currentMultiplier);
    }

    public void HealthUpdate(int health) {
        CurStats.playerHealth = health;
        OnPlayerHealthChanged?.Invoke(health);
    }

    public void resetCurStats() {
        CurStats = new GameStats();
    }

    public void IncrementWaveCount() {
        CurStats.waveCount++;
        OnWaveChange?.Invoke(CurStats.waveCount);
    }

    public event Action<int> OnScoreChanged;
    public event Action<int> OnMultiplierChanged;
    public event Action<int> OnPlayerHealthChanged;
    public event Action<int> OnWaveChange;
}