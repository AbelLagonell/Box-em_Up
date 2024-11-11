using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Ability : MonoBehaviour {
    protected int Damage;
    protected float RechargeSpeedMultiplier { get; private set; } = 1;
    protected float MaxRechargeSpeed = 1;
    public float currentCharge = 0;

    [SerializeField] public GameObject ability;

    public abstract void OnUse();
    public abstract void InInventory();
    public abstract void ChangeAbilityExtra(float amount);

    public bool CanUseAbility() {
        Debug.Log(currentCharge <= 0f ? "Can Use Ability: " : "Cant Use Ability: " + currentCharge);
        return currentCharge <= 0f;
    }

    public void IncreaseDamage(int amount) {
        Damage += amount;
    }

    // Might change this to be more of a multiplier
    public void DecreaseRechargeSpeed(float amount) {
        RechargeSpeedMultiplier *= 1 / amount;
    }

    public float GetMaxRechargeSpeed() {
        return MaxRechargeSpeed * RechargeSpeedMultiplier;
    }
}