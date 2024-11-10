using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class MainCharacter : Actor {
    private static readonly int Bl = Animator.StringToHash("IsBlocking");     //Bool
    private static readonly int Ab = Animator.StringToHash("IsUsingAbility"); //Bool

    [SerializeField] private GameObject _ability;

    private Vector2 _moveInput;
    private List<Tuple<UpgradeStats, int>> _upgrades = new();

    public int MaxHealth { get; private set; } = 10;

    private void UseAbility() {
        _ability.GetComponent<Ability>().OnUse();
    }

    public void GetAbility(GameObject ability) {
        _ability = ability;
    }

    public void ApplyUpgrade(UpgradeStats upgrade) {
        var tuple = _upgrades.Find(t => t.Item1.Stat == upgrade.Stat);
        if (tuple == null)
            _upgrades.Add(new Tuple<UpgradeStats, int>(upgrade, 1));
        else
            _upgrades[_upgrades.IndexOf(tuple)] = new Tuple<UpgradeStats, int>(tuple.Item1, tuple.Item2 + 1);

        switch (upgrade.Stat) {
            case StatUpgrade.Health:
                MaxHealth += (int)upgrade.StatUp;
                break;
            case StatUpgrade.Defense:
                defense += (int)upgrade.StatUp;
                break;
            case StatUpgrade.Attack:
                attack += (int)upgrade.StatUp;
                break;
            case StatUpgrade.Speed:
                speed += upgrade.StatUp;
                break;
            case StatUpgrade.AttackSpeed:
                attackSpeed += upgrade.StatUp;
                break;
            case StatUpgrade.AbilityDamage:
                _ability.GetComponent<Ability>().IncreaseDamage((int)upgrade.StatUp);
                break;
            case StatUpgrade.RechargeRate:
                _ability.GetComponent<Ability>().DecreaseRechargeSpeed(upgrade.StatUp);
                break;
            case StatUpgrade.AbilityExtra:
                _ability.GetComponent<Ability>().ChangeAbilityExtra(upgrade.StatUp);
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
        SetAnimationBool(false, Ab, (int)StateOrder.Ability);
        Attack();
    }

    public void OnBlockActivated(InputAction.CallbackContext bl) {
        if (!bl.started) return;
        SetAnimationBool(true, Bl, (int)StateOrder.Blocking);
        StartCoroutine(ResetFlag(Bl, (int)StateOrder.Blocking,
                                 animations[(int)StateOrder.Blocking].length));
    }

    public void OnAbilityActivated(InputAction.CallbackContext ab) {
        if (!ab.started || !HasAbility()) return; //checking whether he has the ability to use ability
        if (!_ability.GetComponent<Ability>().CanUseAbility()) return; //checking if ability is ready
        SetAnimationBool(true, Ab, (int)StateOrder.Ability);
        SetAnimationBool(false, At, (int)StateOrder.Attack);
        StartCoroutine(ResetFlag(Ab, (int)StateOrder.Ability,
                                 animations[(int)StateOrder.Ability].length * (1 + 1 / attackSpeed)));
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
            _moveInput  = Vector2.zero;
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