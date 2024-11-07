using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity : MonoBehaviour {
    //score is the amount of score this has
    [SerializeField] public int health = 1, score = 1;

    //-1=> None, 0 => Player, 1 => Enemy, 2 => Both 
    [SerializeField] private int hurtboxType = 2;
    protected Dictionary<string, bool> HitboxConditions = new();

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
        health -= amount;
        if (health <= 0) TriggerDeath();
    }

    protected virtual void TriggerDeath() {
        GameStatTracker.Instance?.AddScore(score);
        Destroy(gameObject);
    }

    protected bool CheckHitboxTag(string componentTag) {
        return HitboxConditions.Any(kvp => kvp.Key == componentTag);
    }

    protected void InitHitboxDict() {
        HitboxConditions.Add("HitboxPlayer", hurtboxType is 0 or 2);
        HitboxConditions.Add("HitboxEnemy", hurtboxType is 1 or 2);
    }
}