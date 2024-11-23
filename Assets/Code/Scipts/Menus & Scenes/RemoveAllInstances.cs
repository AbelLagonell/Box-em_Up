using UnityEngine;

public class RemoveAllInstances : MonoBehaviour {
    public void DeleteAll() {
        Destroy(MainCharacter.Instance);
        Destroy(Waves.Instance);
        Destroy(GameStatTracker.Instance);
        Destroy(StatDisplay.Instance);
    }
}