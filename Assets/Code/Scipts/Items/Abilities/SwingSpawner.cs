using System;
using UnityEngine;

public class SwingSpawner : Ability {
    public float radius = 2.3f;

    private SwingSpawner() {
        extraAbility = 90f;
    }

    public override void OnUse() {
        currentCharge = maxRechargeSpeed * RechargeSpeedMultiplier;
        var cSwing = Instantiate(ability,
                                 transform.position + transform.up * 1.5f,
                                 transform.rotation);
        cSwing.GetComponent<Swing>().NewAngle(extraAbility, radius, transform.rotation);
        cSwing.transform.GetChild(0).GetComponent<Hitbox>().damage = Damage;
    }

    public override void InInventory() {
        throw new NotImplementedException();
    }

    public override void ChangeAbilityExtra(float amount) {
        extraAbility += 30f;
    }

    public override AbilityType GetAbilityType() {
        return AbilityType.Swing;
    }
}