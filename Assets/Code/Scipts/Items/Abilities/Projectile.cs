using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour {
    private readonly string[] _avoidList = { "Player", "HitboxPlayer", "Respawn" };
    private Rigidbody _rb;

    private void OnTriggerEnter(Collider other) {
        if (_avoidList.Any(other.CompareTag)) return;
        Destroy(gameObject);
    }

    public void Init(Vector3 vector3) {
        _rb = GetComponent<Rigidbody>();
        _rb.velocity = vector3;
    }
}