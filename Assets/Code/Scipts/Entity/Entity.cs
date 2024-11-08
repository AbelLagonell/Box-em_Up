using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity : MonoBehaviour {
    //score is the amount of score this has
    [SerializeField] public int health = 1, score = 1;

    //-1=> None, 0 => Player, 1 => Enemy, 2 => Both 
    [SerializeField] private int hurtboxType = 2;
    private readonly Dictionary<string, bool> _hitboxConditions = new();

    private void Start() {
        InitHitboxDict();
        if (health < 0) health = 10;
    }

    protected void OnTriggerEnter(Collider collision) {
        if (CheckHitboxTag(collision.tag))
            DecreaseHealth(collision.GetComponent<Hitbox>().damage);
    }

    protected void IncreaseHealth(int amount) {
        health += amount;
    }

    protected virtual void DecreaseHealth(int amount) {
        if (amount < 0) return;
        health -= amount;
        if (health <= 0) TriggerDeath();
    }

    protected virtual void TriggerDeath() {
        GameStatTracker.Instance?.AddScore(score);
        Destroy(gameObject);
    }

    private bool CheckHitboxTag(string componentTag) {
        return _hitboxConditions.Any(kvp => kvp.Key == componentTag);
    }

    private void InitHitboxDict() {
        _hitboxConditions.Add("HitboxPlayer", hurtboxType is 0 or 2);
        _hitboxConditions.Add("HitboxEnemy", hurtboxType is 1 or 2);
    }
}