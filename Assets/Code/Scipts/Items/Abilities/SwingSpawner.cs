using System;
using UnityEngine;

public class SwingSpawner : Ability {
    private float _startDegrees = -45f;
    [SerializeField] private float end = 45f;

    public override void OnUse() {
        
    }

    public override void InInventory() {
        throw new NotImplementedException();
    }

    public override void ChangeAbilityExtra(float amount) {
        end += amount;
    }

    private void Update() {
        currentCharge -= Time.deltaTime;
    }
}