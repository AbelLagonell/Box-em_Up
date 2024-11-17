using UnityEngine;

public class NextWaveButton : MonoBehaviour {
    public NextWaveButton nextWaveButton { get; private set; }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        GameStatTracker.Instance.IncrementWaveCount();
    }
}