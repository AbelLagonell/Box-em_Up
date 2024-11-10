using UnityEngine;

public class AbilityPickup : GameItem {
    public GameObject abilityPickup;

    protected override void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag("Player")) return;
        other.gameObject.GetComponent<MainCharacter>().GetAbility(abilityPickup);
        Destroy(gameObject);
    }
}