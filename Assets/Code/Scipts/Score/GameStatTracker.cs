using System;
using System.Collections;
using UnityEngine;

public class GameStatTracker : MonoBehaviour {
    [SerializeField] private GameStats _curStats;

    public static GameStatTracker Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        _curStats = new GameStats();
        Instance  = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddScore(int score) {
        _curStats.totalScore += score * _curStats.currentMultiplier;
        OnScoreChanged?.Invoke(_curStats.totalScore);
    }

    public void IncrementMultiplier() {
        _curStats.currentMultiplier++;
        StartCoroutine(UpdateMultiplier());
    }

    public void ResetMultiplier() {
        _curStats.currentMultiplier = 1;
        OnMultiplierChanged?.Invoke(_curStats.currentMultiplier);
    }

    public void HealthUpdate(int health) {
        _curStats.cPlayerHealth = health;
        OnPlayerHealthChanged?.Invoke(health);
    }

    public void MaxHealthUpdate(int health) {
        _curStats.maxPlayerHealth = health;
    }

    public void ResetCurStats() {
        _curStats = new GameStats();
    }

    public void IncrementWaveCount() {
        _curStats.waveCount++;
        OnWaveChange?.Invoke(_curStats.waveCount);
    }

    public int GetScore() {
        return _curStats.totalScore;
    }

    public int GetWaveCount() {
        return _curStats.waveCount;
    }

    public void DecrementScore(int amount) {
        _curStats.totalScore -= amount;
        OnScoreChanged?.Invoke(_curStats.totalScore);
    }

    private IEnumerator UpdateMultiplier() {
        yield return new WaitForSeconds(0.1f);
        OnMultiplierChanged?.Invoke(_curStats.currentMultiplier);
    }

    public event Action<int> OnScoreChanged;
    public event Action<int> OnMultiplierChanged;
    public event Action<int> OnPlayerHealthChanged;
    public event Action<int> OnWaveChange;
}