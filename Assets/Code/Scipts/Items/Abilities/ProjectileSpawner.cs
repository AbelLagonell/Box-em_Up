using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class ProjectileSpawner : Ability {
    [SerializeField] private float projectileSpeed = 1f;

    public override void OnUse() {
        maxRechargeSpeed = 10f;
        currentCharge    = maxRechargeSpeed * RechargeSpeedMultiplier;
        var projectile = Instantiate(ability,
                                     transform.forward + Vector3.up * 1.5f,
                                     Quaternion.Euler(-90, 0, 0));

        projectile.GetComponent<Projectile>().Init(transform.forward * projectileSpeed);
    }

    public override void InInventory() {
        throw new NotImplementedException();
    }

    public override void ChangeAbilityExtra(float amount) {
        projectileSpeed *= 1 + 1 / amount;
    }

    private void Update() {
        currentCharge -= Time.deltaTime;
    }
}