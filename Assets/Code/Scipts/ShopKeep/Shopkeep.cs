using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
}

public struct PickupType {
    public bool Pickup; // false => Upgrade, true=> Ability
    public int Index;

    public PickupType(bool pickup, int index) {
        Pickup = pickup;
        Index = index;
    }
}

public class Shopkeep : MonoBehaviour {
    private static int _hasAbility;
    [SerializeField] private GameObject[] pickups;
    public Multipliers multipliers;
    private readonly List<PickupType> _displayPickups = new();
    private Transform[] _group;
    private Transform[] _replacement;
    private int _waveCount = 1;

    private void Awake() {
        GameStatTracker.Instance.OnWaveChange += DestroySelf;
        _waveCount = GameStatTracker.Instance.GetWaveCount();
        //Checking if character has a ability
        _hasAbility = MainCharacter.Instance.HasAbility();
        _group = new Transform[transform.childCount - 1];
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
        //?Need to change if both can work
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
        var text = group.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = price.ToString();

        var upgrade = Instantiate(pickups[1], pos, Quaternion.identity, group);
        upgrade.GetComponent<Upgrade>().StartUp(upgradeType);
        _displayPickups.Add(new PickupType(false, (int)upgradeType));
    }

    private void Replace(GameObject replacement, Transform group, AbilityType abilityType) {
        var pos = replacement.transform.position;
        Destroy(replacement);
        var price = GetPrice(abilityType);
        var text = group.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = price.ToString();

        var upgrade = Instantiate(pickups[0], pos, Quaternion.identity, group);
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
            AbilityType.Projectile => 10,
            AbilityType.Swing      => 10,
            _                      => throw new ArgumentOutOfRangeException(nameof(abilityType), abilityType, null)
        };

        return Mathf.RoundToInt(_waveCount * scalar * .5f);
    }

    private void BuyingItem(int childIndex) {
        var pickup = _displayPickups[childIndex];
        if (pickup.Pickup) {
            var price = GetPrice((AbilityType)pickup.Index);
            if (price > GameStatTracker.Instance.GetScore()) return;
            GameStatTracker.Instance.DecrementScore(price);
            GetComponentInChildren<AbilityPickup>().Apply();
        } else {
            var price = GetPrice((UpgradeType)pickup.Index);
            if (price > GameStatTracker.Instance.GetScore()) return;
            GameStatTracker.Instance.DecrementScore(price);
            GetComponentInChildren<Upgrade>().Apply();
        }

        Debug.Log(childIndex);

        Destroy(transform.GetChild(childIndex).GetComponentInChildren<ButtonScript>().gameObject);
        Destroy(transform.GetChild(childIndex).GetComponentInChildren<Canvas>().gameObject);
        Destroy(transform.GetChild(childIndex).GetComponentInChildren<GameItem>().gameObject);
    }

    public static int GetRandomPickup() {
        var possiblePickups = new List<int>();

        // Add all basic upgrades (first 5 from UpgradeType enum)
        for (var i = 0; i < 5; i++) possiblePickups.Add(i);

        if (_hasAbility != -1) // Has an ability
        {
            // Add RechargeRate, AbilityDamage, and AbilityExtra
            possiblePickups.Add((int)UpgradeType.RechargeRate);
            possiblePickups.Add((int)UpgradeType.AbilityDamage);
            possiblePickups.Add((int)UpgradeType.AbilityExtra);

            // Add the ability they don't have
            possiblePickups.Add(8 + (_hasAbility == 0 ? 1 : 0)); // 8 offset for abilities
        } else                                                   // No ability
        {
            // Add both abilities
            possiblePickups.Add(8); // Projectile
            possiblePickups.Add(9); // Swing
        }

        // Get random index
        var randomIndex = Random.Range(0, possiblePickups.Count);
        return possiblePickups[randomIndex];
    }

    // Helper method to convert the random number to the appropriate type
    public static (bool isAbility, int index) ConvertRandomToType(int random) {
        return random >= 8
            ? // Ability types start at 8
            (true, random - 8)
            :                // Subtract 8 to get actual ability index
            (false, random); // Upgrade type
    }

    private void DestroySelf(int dummy) {
        Debug.Log(dummy);
        Destroy(gameObject);
    }
}