using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainCharacter : Actor {
    private static readonly int Bl = Animator.StringToHash("IsBlocking");     //Bool
    private static readonly int Ab = Animator.StringToHash("IsUsingAbility"); //Bool
    [Header("Inventory")] [SerializeField] private GameObject inventoryPrefab;
    [SerializeField] private GameObject itemPrefab;

    [Header("Unity SFX")] [SerializeField] private UnityEvent OnLowHealth;
    [SerializeField] private UnityEvent OnAbilityUse;

    [Header("Debug")] [SerializeField] private GameObject ability;
    private readonly List<GameObject> _items = new();

    private readonly List<Tuple<UpgradeStats, int>> _upgrades = new();
    private Ability _abilityScript;
    private ChangeScene _changeScene;
    private GameObject _inventory;
    private int _maxHealth = 10;
    private Vector2 _moveInput;
    private Transform _panel;
    private bool _warn;

    public static MainCharacter Instance { get; private set; }

    protected void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    protected new void Start() {
        GameStatTracker.Instance.OnPlayerHealthChanged += OnHealthUpdate;
        _changeScene                                   =  GetComponent<ChangeScene>();
        base.Start();
        _inventory = Instantiate(inventoryPrefab, transform);
        _panel     = _inventory.transform.Find("Panel");
        _inventory.SetActive(false);
    }

    private void FixedUpdate() {
        if (SceneManager.GetActiveScene().buildIndex == 4) Destroy(gameObject);
        if (transform.position.y                     < -10f) DecreaseHealth(health + 1);
        Movement();
        if (HasAbility() == -1) return;
        if (_abilityScript.currentCharge > _abilityScript.GetMaxRechargeSpeed())
            _abilityScript.currentCharge -= Time.fixedDeltaTime;
    }

    private void OnDestroy() {
        GameStatTracker.Instance.OnPlayerHealthChanged -= OnHealthUpdate;
    }

    public void OnInventoryActivated(InputAction.CallbackContext ia) {
        if (ia.started || ia.performed) _inventory.SetActive(true);
        if (ia.canceled) _inventory.SetActive(false);
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
        OnAbilityUse?.Invoke();
        SetAnimationBool(true, Ab, (int)StateOrder.Ability);
        SetAnimationBool(false, At, (int)StateOrder.Attack);
        StartCoroutine(ResetFlag(Ab, (int)StateOrder.Ability,
                                 animations[(int)StateOrder.Ability].length * (1 + 1 / attackSpeed)));
    }

    protected override void DecreaseHealth(int amount) {
        if (CurrentState[(int)StateOrder.Blocking]) return;
        health -= amount - defense >= 0 ? amount - defense : 0;
        if (health <= 0 && !AnimatorController.GetBool(Dd)) TriggerDeath();
        GameStatTracker.Instance?.HealthUpdate(health);
        GameStatTracker.Instance?.ResetMultiplier();
        if (health <= _maxHealth * .25f && _warn) {
            _warn = false;
            OnLowHealth?.Invoke();
        }

        GotHit();
    }

    protected override void TriggerDeath() {
        //When the Player dies 
        //TODO Animation does not play the entire time
        OnDeath?.Invoke();
        AnimatorController.SetBool(Dd, true);
        _changeScene.SceneChange();
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
                                   new Vector3(_moveInput.x, 0, _moveInput.y) * speed, 0.7f) +
                      new Vector3(0, Rb.velocity.y, 0);
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
        _abilityScript.ability          = sourceAbility.ability;
        _abilityScript.maxRechargeSpeed = sourceAbility.maxRechargeSpeed;
        _abilityScript.extraAbility     = sourceAbility.extraAbility;

        // Remove all upgrade related Stats
        for (var i = 5; i < 8; i++) {
            var removed = _upgrades.FindIndex(t => t.Item1.Stat == (UpgradeType)i);
            if (removed == -1) continue;
            _upgrades.RemoveAt(removed);
            _items.RemoveAt(removed);
        }

        _abilityScript.OnCurrentChargeChanged += OnChargeChange;
    }

    public void ApplyUpgrade(UpgradeStats upgrade) {
        var index = _upgrades.FindIndex(t => t.Item1.Stat == upgrade.Stat);
        if (index == -1) {
            _upgrades.Add(new Tuple<UpgradeStats, int>(upgrade, 1));
            var temp = Instantiate(itemPrefab, _panel.transform);
            temp.GetComponent<ItemChange>().Init(upgrade.Stat);
            _items.Add(temp);
        } else {
            _upgrades[index] = new Tuple<UpgradeStats, int>(upgrade, _upgrades[index].Item2 + 1);
            _items[index].GetComponent<ItemChange>().IncrementAmount();
        }

        switch (upgrade.Stat) {
            case UpgradeType.Health:
                _maxHealth += (int)upgrade.StatUp;
                health     += _maxHealth;
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

    public List<Tuple<UpgradeStats, int>> GetUpgradeList() {
        return _upgrades;
    }

    private void OnHealthUpdate(int input) {
        if (input >= _maxHealth * .25f) _warn = true;
    }

    public void AddHealth(int amount = -1) {
        if (amount == -1) amount = _maxHealth / 2;
        if (amount + health >= _maxHealth) health =  _maxHealth;
        else health                               += amount;
        GameStatTracker.Instance?.HealthUpdate(health);
    }

    public event Action OnDeath;
    public event Action<float> OnChargeChange;
}