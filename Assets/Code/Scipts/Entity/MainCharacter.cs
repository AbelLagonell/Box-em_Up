using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCharacter : Actor {
    private static readonly int Bl = Animator.StringToHash("IsBlocking");     //Bool
    private static readonly int Ab = Animator.StringToHash("IsUsingAbility"); //Bool

    private Ability _ability;

    private Vector2 _moveInput;
    private List<Tuple<Upgrade, int>> _upgrades;

    public int MaxHealth { get; private set; } = 10;

    private void UseAbility() { }

    public void ApplyUpgrade(Upgrade upgrade) {
        var tuple = _upgrades.Find(t => t.Item1.Equals(upgrade));
        if (tuple != default(Tuple<Upgrade, int>))
            _upgrades[_upgrades.IndexOf(tuple)] = new Tuple<Upgrade, int>(tuple.Item1, tuple.Item2 + 1);

        switch (upgrade.stat) {
            case StatUpgrade.Health:
                MaxHealth += (int)upgrade.statUp;
                break;
            case StatUpgrade.Defense:
                defense += (int)upgrade.statUp;
                break;
            case StatUpgrade.Attack:
                attack += (int)upgrade.statUp;
                break;
            case StatUpgrade.Speed:
                speed += upgrade.statUp;
                break;
            case StatUpgrade.AttackSpeed:
                attackSpeed += upgrade.statUp;
                break;
            case StatUpgrade.AbilityDamage:
                _ability.IncreaseDamage((int)upgrade.statUp);
                break;
            case StatUpgrade.RechargeRate:
                _ability.DecreaseRechargeSpeed(upgrade.statUp);
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

    public void OnAttackedActivated(InputAction.CallbackContext aa) {
        if (!aa.started) return;
        Attack();
    }

    protected override void TriggerDeath() {
        //When the Player dies
        AnimatorController.SetBool(Dd, true);
    }

    protected override void Movement() {
        //IF No input
        if (_moveInput == Vector2.zero) AnimatorController.SetBool(Mv, false);

        //IF another action is currently happening don't move
        if (CurrentState.Contains(true)) {
            _moveInput = Vector2.zero;
            Rb.velocity = Vector3.zero;
        }

        //IF there is an action update the looking
        if (_moveInput != Vector2.zero) {
            AnimatorController.SetBool(Mv, true);
            //Making sure that the player is facing the correct direction (used for hitboxes)
            var angle = Mathf.Atan2(_moveInput.x, _moveInput.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        //Moving
        Rb.velocity = Vector3.Lerp(new Vector3(Rb.velocity.x, 0, Rb.velocity.z),
                                   new Vector3(_moveInput.x, 0, _moveInput.y) * speed, 0.7f);
    }

    public bool HasAbility() {
        return _ability != null;
    }
}