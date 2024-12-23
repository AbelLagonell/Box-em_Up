using System;
using UnityEngine;

public class ProjectileSpawner : Ability {
    private ProjectileSpawner() {
        extraAbility = 5f;
    }

    public override void OnUse() {
        currentCharge = maxRechargeSpeed * RechargeSpeedMultiplier;
        var proj = Instantiate(ability,
                               transform.position + transform.up * 1.5f + transform.forward,
                               Quaternion.Euler(-90, 0, 0));

        proj.GetComponent<Projectile>().Init(transform.forward * extraAbility);
        proj.transform.GetChild(0).GetComponent<Hitbox>().damage = Damage;
    }

    public override void InInventory() {
        throw new NotImplementedException();
    }

    public override void ChangeAbilityExtra(float amount) {
        extraAbility *= 1 + 1 / amount;
    }

    public override AbilityType GetAbilityType() {
        return AbilityType.Projectile;
    }
}