using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class ProjectileSpawner : Ability {
    [SerializeField] private float projectileSpeed = 1f;

    public override void OnUse() {
        currentCharge = maxRechargeSpeed * RechargeSpeedMultiplier;
        var projectile = Instantiate(ability,
                                     transform.position + transform.up * 1.5f + transform.forward,
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