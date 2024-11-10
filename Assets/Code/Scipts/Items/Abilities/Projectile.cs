using System;
using UnityEngine;

public class Projectile : Ability {
    public Projectile() {
        MaxRechargeSpeed = 10;
    }

    private void Start() {
        Rb = GetComponent<Rigidbody>();
    }

    public override void OnUse() {
        Rb.velocity   = Vector3.forward;
        CurrentCharge = MaxRechargeSpeed * RechargeSpeedMultiplier;
    }

    public override void SpawnHitbox() {
        //Hitbox is in prefab
    }

    public override bool CanUseAbility() {
        return CurrentCharge < 0;
    }

    public override void ChangeAbilityExtra(float amount) {
        // Make the projectile Faster
    }
}