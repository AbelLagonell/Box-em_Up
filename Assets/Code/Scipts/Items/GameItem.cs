using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItem : MonoBehaviour {
    private Rigidbody rb;

    private void Start() {
        var rb = GetComponent<Rigidbody>();
        rb.angularVelocity = new Vector3(0f, 1f, 0f);
    }

    private void FixedUpdate() {
        transform.position =
            new Vector3(transform.position.x, 0.33f * Mathf.Sin(Time.time * 1.5f) + 1.5f, transform.position.z);
    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) Destroy(gameObject);
    }
}