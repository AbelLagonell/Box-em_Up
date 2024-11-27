using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

// ReSharper disable CheckNamespace

//  UpgradeType, AbilityType

[Serializable]
public struct Multipliers {
    public int health;
    public int speed;
    public int defense;
    public int attack;
    public int attackSpeed;
    public int rechargeRate;
    public int abilityDamage;
    public int abilityExtra;
    public int projectile;
    public int swing;
}

public struct PickupType {
    public readonly bool Pickup; // false => Upgrade, true=> Ability
    public readonly int Index;

    public PickupType(bool pickup, int index) {
        Pickup = pickup;
        Index  = index;
    }
}

public class Shopkeep : MonoBehaviour {
    private static int _hasAbility;
    [SerializeField] private GameObject[] pickups;
    public Multipliers multipliers;
    public UnityEvent OnPickup;
    private readonly HashSet<int> _currentShopPickups = new();
    private readonly List<PickupType> _displayPickups = new();
    private Transform[] _group;
    private Transform[] _replacement;
    private int _waveCount = 1;


    // ReSharper disable once MemberCanBePrivate.Global
    public static Shopkeep Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;

        GameStatTracker.Instance.OnWaveChange += DestroySelf;
        _waveCount                            =  GameStatTracker.Instance.GetWaveCount();
        //Checking if character has a ability
        _hasAbility  = MainCharacter.Instance.HasAbility();
        _group       = new Transform[transform.childCount - 1];
        _replacement = new Transform[transform.childCount - 1];
        for (var i = 0; i < transform.childCount - 1; i++) {
            //Populating the children with the different groups
            _group[i] = transform.GetChild(i);
            //Getting the empty game Objects
            _replacement[i] = _group[i].GetChild(0);
            //Subscribing to events from buttons
            _group[i].GetChild(2).GetComponent<ButtonScript>().OnButtonPressed += BuyingItem;
            _group[i].GetChild(2).GetComponent<ButtonScript>().SetId(i);
        }

        // Setting up the upgrade pickups
        for (var i = 0; i < transform.childCount - 1; i++) {
            var rand = GetRandomPickup();
            var (isAbility, index) = ConvertRandomToType(rand);
            if (isAbility)
                Replace(_replacement[i].gameObject, _group[i], (AbilityType)index);
            else
                Replace(_replacement[i].gameObject, _group[i], (UpgradeType)index);
        }
    }

    private void OnDestroy() {
        GameStatTracker.Instance.OnWaveChange -= DestroySelf;
    }

    private void Replace(GameObject replacement, Transform group, UpgradeType upgradeType) {
        var pos = replacement.transform.position;
        Destroy(replacement);
        var price = GetPrice(upgradeType);
        var text  = group.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = price.ToString();

        var upgrade = Instantiate(pickups[1], pos, Quaternion.identity, group);
        upgrade.GetComponent<Upgrade>().StartUp(upgradeType);
        _displayPickups.Add(new PickupType(false, (int)upgradeType));
    }

    private void Replace(GameObject replacement, Transform group, AbilityType abilityType) {
        var pos = replacement.transform.position;
        Destroy(replacement);
        var price = GetPrice(abilityType);
        var text  = group.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = price.ToString();

        var abilityPickup = Instantiate(pickups[0], pos, Quaternion.identity, group);
        abilityPickup.GetComponent<AbilityPickup>().StartUp(abilityType);
        _displayPickups.Add(new PickupType(true, (int)abilityType));
    }

    private int GetPrice(UpgradeType upgradeType) {
        var scalar = upgradeType switch {
            UpgradeType.Health        => multipliers.health,
            UpgradeType.Speed         => multipliers.speed,
            UpgradeType.Defense       => multipliers.defense,
            UpgradeType.Attack        => multipliers.attack,
            UpgradeType.AttackSpeed   => multipliers.attackSpeed,
            UpgradeType.RechargeRate  => multipliers.rechargeRate,
            UpgradeType.AbilityDamage => multipliers.abilityDamage,
            UpgradeType.AbilityExtra  => multipliers.abilityExtra,
            _                         => throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null)
        };

        return Mathf.RoundToInt(_waveCount * scalar + MainCharacter.Instance.GetUpgradeAmount(upgradeType));
    }

    private int GetPrice(AbilityType abilityType) {
        var scalar = abilityType switch {
            AbilityType.Projectile => multipliers.projectile,
            AbilityType.Swing      => multipliers.swing,
            _                      => throw new ArgumentOutOfRangeException(nameof(abilityType), abilityType, null)
        };

        return Mathf.RoundToInt(_waveCount * scalar);
    }

    private void BuyingItem(int childIndex) {
        var pickup = _displayPickups[childIndex];
        if (pickup.Pickup) {
            var price = GetPrice((AbilityType)pickup.Index);
            if (price > GameStatTracker.Instance.GetScore()) return;
            GameStatTracker.Instance.DecrementScore(price);
            transform.GetChild(childIndex).GetComponentInChildren<AbilityPickup>().Apply(pickup.Index);
        } else {
            var price = GetPrice((UpgradeType)pickup.Index);
            if (price > GameStatTracker.Instance.GetScore()) return;
            GameStatTracker.Instance.DecrementScore(price);
            transform.GetChild(childIndex).GetComponentInChildren<Upgrade>().Apply();
        }

        OnPickup.Invoke();

        Destroy(transform.GetChild(childIndex).GetComponentInChildren<ButtonScript>().gameObject);
        Destroy(transform.GetChild(childIndex).GetComponentInChildren<Canvas>().gameObject);
        Destroy(transform.GetChild(childIndex).GetComponentInChildren<GameItem>().gameObject);
    }

    private int GetRandomPickup() {
        var possiblePickups = new List<int>();

        // Add all basic upgrades (first 5 from UpgradeType enum)
        for (var i = 0; i < 5; i++)
            // Only add if not already picked in this shop
            if (!_currentShopPickups.Contains(i))
                possiblePickups.Add(i);

        if (_hasAbility != -1) // Has an ability
        {
            // Add ability-related upgrades if not already picked
            if (!_currentShopPickups.Contains((int)UpgradeType.RechargeRate))
                possiblePickups.Add((int)UpgradeType.RechargeRate);
            if (!_currentShopPickups.Contains((int)UpgradeType.AbilityDamage))
                possiblePickups.Add((int)UpgradeType.AbilityDamage);
            if (!_currentShopPickups.Contains((int)UpgradeType.AbilityExtra))
                possiblePickups.Add((int)UpgradeType.AbilityExtra);

            // Add the ability they don't have if not already picked
            var otherAbility = 8 + (_hasAbility == 0 ? 1 : 0);
            if (!_currentShopPickups.Contains(otherAbility))
                possiblePickups.Add(otherAbility);
        } else // No ability
        {
            // Add abilities if not already picked
            if (!_currentShopPickups.Contains(8))
                possiblePickups.Add(8); // Projectile
            if (!_currentShopPickups.Contains(9))
                possiblePickups.Add(9); // Swing
        }

        // If we've used all possible pickups, something's wrong
        if (possiblePickups.Count == 0) {
            Debug.LogWarning("No more unique pickups available!");
            return -1; // Or handle this case as needed
        }

        // Get random index
        var randomIndex    = Random.Range(0, possiblePickups.Count);
        var selectedPickup = possiblePickups[randomIndex];

        // Add to current shop pickups
        _currentShopPickups.Add(selectedPickup);

        return selectedPickup;
    }

    private static (bool isAbility, int index) ConvertRandomToType(int random) {
        return random >= 8
            ? (true, random - 8)
            : (false, random);
    }


    private void DestroySelf(int dummy) {
        Destroy(gameObject);
    }
}