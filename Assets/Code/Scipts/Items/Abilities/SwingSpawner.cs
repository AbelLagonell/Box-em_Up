using System;
using UnityEngine;

public class SwingSpawner : Ability {
    [SerializeField] private float totalAngle = 90f;
    public float radius = 2.3f;

    public override void OnUse() {
        currentCharge = maxRechargeSpeed * RechargeSpeedMultiplier;
        var cSwing = Instantiate(ability,
                                 transform.position + transform.forward * 1.5f + transform.right * radius -
                                 transform.up * 2,
                                 Quaternion.Euler(-90, 0, 0));
        cSwing.GetComponent<Swing>().newAngle(totalAngle, radius);
    }

    public override void InInventory() {
        throw new NotImplementedException();
    }

    public override void ChangeAbilityExtra(float amount) {
        totalAngle += amount;
    }

    private void Update() {
        currentCharge -= Time.deltaTime;
    }
}