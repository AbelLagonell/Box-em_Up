using System;
using UnityEngine;

public abstract class Ability : MonoBehaviour {
    protected int Damage { get; private set; }
    protected float RechargeSpeedMultiplier { get; private set; } = 1;
    protected float MaxRechargeSpeed = 1;
    protected float CurrentCharge = 0;

    [SerializeField] protected GameObject hitbox;
    protected Rigidbody Rb;

    private void FixedUpdate() {
        if (CurrentCharge < 0) return;
        CurrentCharge -= Time.deltaTime;
    }

    public virtual void OnUse() {
        throw new NotImplementedException();
    }

    public virtual void SpawnHitbox() {
        throw new NotImplementedException();
    }

    public virtual void InInventory() {
        throw new NotImplementedException();
    }

    public virtual bool CanUseAbility() {
        throw new NotImplementedException();
    }

    public void IncreaseDamage(int amount) {
        Damage += amount;
    }

    // Might change this to be more of a multiplier
    public void DecreaseRechargeSpeed(float amount) {
        RechargeSpeedMultiplier += 1 / amount - 1;
    }

    public virtual void ChangeAbilityExtra(float amount) {
        throw new NotImplementedException();
    }
}