using UnityEngine;

public class Actor : Entity {
    public float speed = 1, attackSpeed = 1;
    public int defense, attack = 1;
    protected Rigidbody Rb;
    [SerializeField] protected GameObject hitbox;
    [SerializeField] protected Vector3 sizeHitbox = Vector3.one;

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

    protected virtual void Movement() {
        //Move towards the player crudely and then switch to navmesh would be better
        //Animation for movement
    }
    protected virtual void Attack() {
        //Once the player is close enough stop moving and then attack towards the last known location.
        //Animation for attacking
    }

    protected virtual void spawnHitbox() {
        GameObject newHitBox = Instantiate(hitbox, transform.forward + transform.position, transform.rotation, transform);
        newHitBox.transform.localScale = sizeHitbox;
        newHitBox.GetComponent<Hitbox>().initDamage(attack);
    }
}