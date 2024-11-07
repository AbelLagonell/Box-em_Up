using UnityEngine;

public class Actor : Entity {
    public float speed = 1, attackSpeed = 1;
    public int defense;
    protected Rigidbody Rb;

    private void Start() {
        Rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        Movement();
    }

    protected override void DecreaseHealth(int amount) {
        health = amount - defense;
        if (health <= 0) TriggerDeath();
    }

    protected virtual void Movement() { }
    protected virtual void Attack() { }
}