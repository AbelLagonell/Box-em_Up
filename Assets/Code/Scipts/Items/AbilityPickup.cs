using UnityEngine;

public enum AbilityType {
    Projectile,
    Swing
}

public class AbilityPickup : GameItem {
    public AbilityType ability;
    [SerializeField] private GameObject[] abilitySpawner;

    public void StartUp(AbilityType abilityType) {
        ability = abilityType;
        Init();
        UpdateTexture((int)ability);
    }

    public override void Apply() {
        MainCharacter.Instance.GetAbility(abilitySpawner[(int)ability]);
    }
}