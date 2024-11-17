using System;

public enum UpgradeType {
    Health,
    Speed,
    Defense,
    Attack,
    AttackSpeed,
    RechargeRate,
    AbilityDamage,
    AbilityExtra
}

public struct UpgradeStats {
    public readonly UpgradeType Stat;
    public readonly float StatUp;

    public UpgradeStats(UpgradeType stat, float statUp) {
        StatUp = statUp;
        Stat = stat;
    }
}

public class Upgrade : GameItem {
    public float statUp;
    public UpgradeType stat;

    public void StartUp(UpgradeType upgradeType) {
        Init();
        stat = upgradeType;
        SetStatUp(upgradeType);
        UpdateTexture((int)upgradeType);
    }

    public override void Apply() {
        MainCharacter.Instance.ApplyUpgrade(new UpgradeStats(stat, statUp));
    }

    public void InInventory() {
        // A function that when called allows the item to be able to be viewed in the inventory
    }

    private void SetStatUp(UpgradeType upgradeType) {
        // ReSharper disable once ConvertSwitchStatementToSwitchExpression
        switch (upgradeType) {
            case UpgradeType.Health:
            case UpgradeType.Speed:
            case UpgradeType.Defense:
            case UpgradeType.Attack:
            case UpgradeType.AbilityDamage:
                statUp = 1;
                break;
            case UpgradeType.AttackSpeed:
            case UpgradeType.RechargeRate:
            case UpgradeType.AbilityExtra:
                statUp = 2;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null);
        }
    }
}