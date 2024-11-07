using UnityEngine;

public class Upgrade : MonoBehaviour {
    public enum StatUpgrade {
        Health,
        Speed,
        Defense,
        AttackSpeed,
        RechargeRate,
        AbilityDamage
    }

    public float StatUp = 1;
    public StatUpgrade Stat;

    public void InInventory() {
        // A function that when called allows the item to be able to be viewed in the inventory
    }
}