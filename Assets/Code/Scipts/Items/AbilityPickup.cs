using UnityEngine;

public enum AbilityType {
    Projectile,
    Swing
}

public class AbilityPickup : GameItem {
    public AbilityType ability;
    [SerializeField] private GameObject[] abilitySpawner;

    private void Start() {
        Init();
        UpdateTexture((int)ability);
    }

    public override void Apply() {
        MainCharacter.Instance.GetAbility(abilitySpawner[(int)ability]);
    }
}