using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameEnd : MonoBehaviour {
    public TextMeshProUGUI gameState;
    public TextMeshProUGUI score;
    public Transform inventoryPanel;
    public GameObject itemDisplay;
    public LoadMultAudio loadAudio;
    private List<Tuple<UpgradeStats, int>> _upgradeStats;


    private void Awake() {
        var state = MainCharacter.Instance?.health > 0;
        gameState.text = state ? "Game Finished" : "Game Over";
        loadAudio.condition = state ? 0 : 1;

        // Displaying Final Score
        score.text = "Final Score: " + GameStatTracker.Instance?.GetScore();

        //Displaying everything that was bought
        _upgradeStats = MainCharacter.Instance?.GetUpgradeList();
        if (_upgradeStats == null) return;
        foreach (var tuple in _upgradeStats) {
            var temp = Instantiate(itemDisplay, inventoryPanel);
            temp.GetComponent<ItemChange>().Init(tuple.Item1.Stat, tuple.Item2);
        }
    }
}