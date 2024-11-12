using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Projectile : MonoBehaviour {
    private Rigidbody _rb;

    public void Init(Vector3 vector3) {
        _rb          = GetComponent<Rigidbody>();
        _rb.velocity = vector3;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") || other.CompareTag("HitboxPlayer")) return;
        Destroy(gameObject);
    }
}