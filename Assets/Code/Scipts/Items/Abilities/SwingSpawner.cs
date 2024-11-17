using System;
using UnityEngine;

public class SwingSpawner : Ability {
    public float radius = 2.3f;

    private SwingSpawner() {
        extraAbility = 90f;
    }

    private void Update() {
        currentCharge -= Time.deltaTime;
    }

    public override void OnUse() {
        currentCharge = maxRechargeSpeed * RechargeSpeedMultiplier;
        Debug.Log("Grumpy: " + transform.position);
        var cSwing = Instantiate(ability,
                                 transform.position + transform.up * 1.5f,
                                 Quaternion.Euler(-90, 0, 0));
        cSwing.GetComponent<Swing>().NewAngle(extraAbility, radius);
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