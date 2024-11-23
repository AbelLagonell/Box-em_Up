using UnityEngine;

public class Spawner : MonoBehaviour {
    public bool Active { get; private set; } = true;

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        Active = false;
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player")) return;
        Active = true;
    }

    public void Init(float radius) {
        var sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = radius;
    }
}