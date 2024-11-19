using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameEnd : MonoBehaviour {
    public TextMeshProUGUI gameState;
    public TextMeshProUGUI score;
    public Transform inventoryPanel;
    public GameObject itemDisplay;
    private List<Tuple<UpgradeStats, int>> _upgradeStats;

    private void Start() {
        gameState.text = MainCharacter.Instance?.health > 0 ? "Game Finished" : "Game Over";

        //Displaying everything that was bought
        _upgradeStats = MainCharacter.Instance?.GetUpgradeList();
        foreach (var tuple in _upgradeStats) {
            var temp = Instantiate(itemDisplay, inventoryPanel);
            temp.GetComponent<ItemChange>().Init(tuple.Item1.Stat, tuple.Item2);
        }

        // Displaying Final Score
        score.text = "Final Score: " + GameStatTracker.Instance?.GetScore();
    }
}