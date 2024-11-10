using System;
using UnityEngine;

public enum StatUpgrade {
    Health,
    Speed,
    Defense,
    Attack,
    AttackSpeed,
    RechargeRate,
    AbilityDamage,
    AbilityExtra
}

public struct UpgradeStats {
    public StatUpgrade Stat;
    public float StatUp;

    public UpgradeStats(StatUpgrade stat, float statUp) {
        StatUp = statUp;
        Stat   = stat;
    }
}

public class Upgrade : GameItem {
    public float statUp = 1;
    public StatUpgrade stat;

    private void ApplyUpgrade(GameObject player) {
        player.GetComponent<MainCharacter>().ApplyUpgrade(new UpgradeStats(stat, statUp));
    }

    protected override void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag("Player")) return;
        ApplyUpgrade(other.gameObject);
        Destroy(gameObject);
    }

    public void InInventory() {
        // A function that when called allows the item to be able to be viewed in the inventory
    }
}