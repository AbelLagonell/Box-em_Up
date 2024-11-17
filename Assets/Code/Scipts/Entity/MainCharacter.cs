using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCharacter : Actor {
    private static readonly int Bl = Animator.StringToHash("IsBlocking");     //Bool
    private static readonly int Ab = Animator.StringToHash("IsUsingAbility"); //Bool

    [SerializeField] private GameObject ability;
    private readonly List<Tuple<UpgradeStats, int>> _upgrades = new();
    private Ability _abilityScript;

    private int _maxHealth = 10;
    private Vector2 _moveInput;

    public static MainCharacter Instance { get; private set; }

    protected new void Start() {
        base.Start();
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void FixedUpdate() {
        Movement();
        if (HasAbility() == -1) return;
        if (_abilityScript.currentCharge > _abilityScript.GetMaxRechargeSpeed())
            _abilityScript.currentCharge -= Time.fixedDeltaTime;
    }

    protected override void DecreaseHealth(int amount) {
        if (!CurrentState[(int)StateOrder.Blocking]) return;
        health -= amount - defense;
        GameStatTracker.Instance?.HealthUpdate(health);
        GameStatTracker.Instance?.ResetMultiplier();
        if (health <= 0) TriggerDeath();
    }

    public void GetAbility(GameObject abilityObject) {
        // Store the ability GameObject reference
        ability = abilityObject;

        // Get the Ability component from the source object
        var sourceAbility = abilityObject.GetComponent<Ability>();

        // If there's an existing ability component on this object, remove it
        var existingAbility = GetComponent<Ability>();
        if (existingAbility != null) Destroy(existingAbility);

        // Add the same type of Ability component to this object
        _abilityScript = gameObject.AddComponent(sourceAbility.GetType()) as Ability;
        Debug.Assert(_abilityScript != null, nameof(_abilityScript) + " != null");
        _abilityScript.ability = sourceAbility.ability;
        _abilityScript.maxRechargeSpeed = sourceAbility.maxRechargeSpeed;
        _abilityScript.extraAbility = sourceAbility.extraAbility;
    }

    public void ApplyUpgrade(UpgradeStats upgrade) {
        var tuple = _upgrades.Find(t => t.Item1.Stat == upgrade.Stat);
        if (tuple == null)
            _upgrades.Add(new Tuple<UpgradeStats, int>(upgrade, 1));
        else
            _upgrades[_upgrades.IndexOf(tuple)] = new Tuple<UpgradeStats, int>(tuple.Item1, tuple.Item2 + 1);

        switch (upgrade.Stat) {
            case UpgradeType.Health:
                _maxHealth += (int)upgrade.StatUp;
                health += _maxHealth;
                GameStatTracker.Instance?.HealthUpdate(_maxHealth);
                break;
            case UpgradeType.Defense:
                defense += (int)upgrade.StatUp;
                break;
            case UpgradeType.Attack:
                attack += (int)upgrade.StatUp;
                break;
            case UpgradeType.Speed:
                speed += upgrade.StatUp;
                break;
            case UpgradeType.AttackSpeed:
                attackSpeed *= 1 + 1 / upgrade.StatUp;
                break;
            case UpgradeType.AbilityDamage:
                _abilityScript.IncreaseDamage((int)upgrade.StatUp);
                break;
            case UpgradeType.RechargeRate:
                _abilityScript.DecreaseRechargeSpeed(upgrade.StatUp);
                break;
            case UpgradeType.AbilityExtra:
                _abilityScript.ChangeAbilityExtra(upgrade.StatUp);
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
        if (!ab.started || HasAbility() == -1) return; //checking whether he has the ability to use ability
        if (!_abilityScript.CanUseAbility()) return;   //checking if ability is ready
        _abilityScript.OnUse();
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
    } // ReSharper disable Unity.PerformanceAnalysis

    public int HasAbility() {
        if (_abilityScript == null) return -1;
        return _abilityScript.GetAbilityType() switch {
            AbilityType.Projectile => 0,
            AbilityType.Swing      => 1,
            _                      => -1
        };
    }

    public int GetUpgradeAmount(UpgradeType upgradeType) {
        var tuple = _upgrades.Find(t => t.Item1.Stat == upgradeType);
        return tuple?.Item2 ?? 0;
    }
}