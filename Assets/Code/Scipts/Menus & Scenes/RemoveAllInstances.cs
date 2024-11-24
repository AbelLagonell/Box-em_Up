using UnityEngine;

public class RemoveAllInstances : MonoBehaviour {
    public void Awake() {
        Destroy(MainCharacter.Instance?.gameObject);
        Destroy(Waves.Instance?.gameObject);
        Destroy(GameStatTracker.Instance?.gameObject);
        Destroy(StatDisplay.Instance?.gameObject);
    }
}