using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ProjectileSpawner : Ability {
    private float _projectileSpeed = 1f;

    public override void OnUse() {
        MaxRechargeSpeed = 10f;
        currentCharge    = MaxRechargeSpeed * RechargeSpeedMultiplier;
        Debug.Log(ability.name);
        Instantiate(ability);
    }

    public override void InInventory() {
        throw new NotImplementedException();
    }

    public override void ChangeAbilityExtra(float amount) {
        _projectileSpeed *= 1 + 1 / amount;
    }
}