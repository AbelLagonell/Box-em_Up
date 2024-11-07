using UnityEngine;

public enum StatUpgrade {
    Health,
    Speed,
    Defense,
    Attack,
    AttackSpeed,
    RechargeRate,
    AbilityDamage
}

public class Upgrade : MonoBehaviour {
    public float statUp = 1;
    public StatUpgrade stat;

    public void InInventory() {
        // A function that when called allows the item to be able to be viewed in the inventory
    }
}