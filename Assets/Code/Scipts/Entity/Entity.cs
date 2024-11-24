using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour {
    //score is the amount of score this has
    [Header("Entity Information")] public int health = 1;
    public int score = 1;

    [Header("Hitbox/HurtBox")] [SerializeField]
    private EntityType hurtboxType;

    [SerializeField] protected UnityEvent OnHit;

    protected void Start() {
        if (health < 0) health = 10;
    }

    protected void OnTriggerEnter(Collider collision) {
        var hitbox = collision.GetComponent<Hitbox>();
        if (hitbox == null) return;

        //We know its a hitbox that is attack this entity;
        if (hitbox.hitboxType != hurtboxType) DecreaseHealth(hitbox.damage);
    }

    protected virtual void DecreaseHealth(int amount) {
        if (amount < 0) return;
        health -= amount;
        OnHit.Invoke();
        if (health <= 0) TriggerDeath();
    }

    protected virtual void TriggerDeath() {
        Destroy(gameObject);
    }
}