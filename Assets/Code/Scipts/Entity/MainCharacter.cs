using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCharacter : Actor {
    private Ability _ability;
    private Vector2 _moveInput;
    private List<Tuple<Upgrade, int>> _upgrades;
    
    private 

    public int MaxHealth { get; private set; } = 10;

    private void UseAbility() { }

    public void ApplyUpgrade(Upgrade upgrade) {
        var tuple = _upgrades.Find(t => t.Item1.Equals(upgrade));
        if (tuple != default(Tuple<Upgrade, int>))
            _upgrades[_upgrades.IndexOf(tuple)] = new Tuple<Upgrade, int>(tuple.Item1, tuple.Item2 + 1);

        switch (upgrade.Stat) {
            case Upgrade.StatUpgrade.Health:
                MaxHealth += (int)upgrade.StatUp;
                break;
            case Upgrade.StatUpgrade.Defense:
                defense += (int)upgrade.StatUp;
                break;
            case Upgrade.StatUpgrade.Attack:
                attack += (int)upgrade.StatUp;
                break;
            case Upgrade.StatUpgrade.Speed:
                speed += upgrade.StatUp;
                break;
            case Upgrade.StatUpgrade.AttackSpeed:
                attackSpeed += upgrade.StatUp;
                break;
            case Upgrade.StatUpgrade.AbilityDamage:
                _ability.IncreaseDamage((int)upgrade.StatUp);
                break;
            case Upgrade.StatUpgrade.RechargeRate:
                _ability.DecreaseRechargeSpeed(upgrade.StatUp);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnMoveActivated(InputAction.CallbackContext mv) {
        if (mv.started || mv.performed)
            _moveInput = mv.ReadValue<Vector2>();
        if (mv.canceled) _moveInput = Vector2.zero;
    }

    public void OnAttackedActivated(InputAction.CallbackContext mv) {
        if (mv.started)
            
            spawnHitbox();
    }

    protected override void TriggerDeath() {
        //When the Player dies
    }

    protected override void Movement() {
        //Making sure that the player is facing the correct direction (used for hitboxes)
        if (_moveInput != Vector2.zero) {
            float angle = Mathf.Atan2(_moveInput.x, _moveInput.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        Rb.velocity = Vector3.Lerp(new Vector3(Rb.velocity.x, 0, Rb.velocity.z),
                                   new Vector3(_moveInput.x, 0, _moveInput.y) * speed, 0.7f);
    }

    public bool HasAbility() {
        return _ability != null;
    }


}