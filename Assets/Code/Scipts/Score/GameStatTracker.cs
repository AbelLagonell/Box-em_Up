using System;
using System.Collections;
using UnityEngine;

public class GameStatTracker : MonoBehaviour {
    private GameStats _curStats;
    public static GameStatTracker Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        _curStats = new GameStats();
        Instance  = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        //_curStats.PlayTime = Time.timeSinceLevelLoad;
    }

    public void AddScore(int score) {
        _curStats.TotalScore += score * _curStats.CurrentMultiplier;
        OnScoreChanged?.Invoke(_curStats.TotalScore);
    }

    public void IncrementMultiplier() {
        _curStats.CurrentMultiplier++;
        StartCoroutine(UpdateMultiplier());
    }

    public void ResetMultiplier() {
        _curStats.CurrentMultiplier = 1;
        OnMultiplierChanged?.Invoke(_curStats.CurrentMultiplier);
    }

    public void HealthUpdate(int health) {
        _curStats.CPlayerHealth = health;
        OnPlayerHealthChanged?.Invoke(health);
    }

    public void MaxHealthUpdate(int health) {
        _curStats.MaxPlayerHealth = health;
    }

    public void ResetCurStats() {
        _curStats = new GameStats();
    }

    public void IncrementWaveCount() {
        _curStats.WaveCount++;
        OnWaveChange?.Invoke(_curStats.WaveCount);
    }

    public int GetScore() {
        return _curStats.TotalScore;
    }

    public void DecrementScore(int amount) {
        _curStats.TotalScore -= amount;
        OnScoreChanged?.Invoke(_curStats.TotalScore);
    }

    private IEnumerator UpdateMultiplier() {
        yield return new WaitForSeconds(0.1f);
        OnMultiplierChanged?.Invoke(_curStats.CurrentMultiplier);
    }

    public event Action<int> OnScoreChanged;
    public event Action<int> OnMultiplierChanged;
    public event Action<int> OnPlayerHealthChanged;
    public event Action<int> OnWaveChange;
}