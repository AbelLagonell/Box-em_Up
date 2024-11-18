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

    public void Apply(int abilityIndex) {
        MainCharacter.Instance.GetAbility(abilitySpawner[abilityIndex]);
    }
}