using System;
using UnityEngine;

public abstract class Ability : MonoBehaviour {
    public float maxRechargeSpeed = 1;
    public float extraAbility = 1;
    public float currentCharge;
    [SerializeField] public GameObject ability;
    protected AbilityType abilityType;

    protected int Damage;
    protected float RechargeSpeedMultiplier { get; private set; } = 1;

    public abstract void OnUse();
    public abstract void InInventory();
    public abstract void ChangeAbilityExtra(float amount);
    public abstract AbilityType GetAbilityType();

    private void Update() {
        currentCharge -= Time.deltaTime;
        OnCurrentChargeChanged?.Invoke(currentCharge);
    }

    public bool CanUseAbility() {
        return currentCharge <= 0f;
    }

    public void IncreaseDamage(int amount) {
        Damage += amount;
    }

    public void DecreaseRechargeSpeed(float amount) {
        RechargeSpeedMultiplier *= 1 / amount;
    }

    public float GetMaxRechargeSpeed() {
        return maxRechargeSpeed * RechargeSpeedMultiplier;
    }

    public event Action<float> OnCurrentChargeChanged;
}