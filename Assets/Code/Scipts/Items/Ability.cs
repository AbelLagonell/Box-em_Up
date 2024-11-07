using UnityEngine;

public class Ability : MonoBehaviour {
    public int Damage { get; private set; }
    public float RechargeSpeed { get; private set; }

    public virtual void OnUse() { }
    public virtual void SpawnHitbox() { }
    public virtual void InInventory() { }

    public void IncreaseDamage(int amount) {
        Damage += amount;
    }

    // Might change this to be more of a multiplier
    public void DecreaseRechargeSpeed(float amount) {
        RechargeSpeed -= amount;
    }
}