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

    private void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag("Player")) return;
        other.gameObject.GetComponent<MainCharacter>().GetAbility(abilitySpawner[(int)ability]);
        Destroy(gameObject);
    }
}