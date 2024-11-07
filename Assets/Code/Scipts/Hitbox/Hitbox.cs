using UnityEngine;

public class Hitbox : MonoBehaviour {
    public int damage = 1;
    public float timeSec = 1;

    public void initDamage(int _damage) {
        if (_damage < 0) damage = 0;
        damage = _damage;
    }

    private void Awake() {
        //time is in seconds.
        Destroy(gameObject, timeSec);
    }
}