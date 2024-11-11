using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Projectile : MonoBehaviour {
    private Rigidbody _rb;
    private float _speed;

    public void Init(float speed) {
        _rb    = GetComponent<Rigidbody>();
        _speed = speed;
    }

    private void FixedUpdate() {
        _rb.velocity = transform.forward * _speed;
    }
}