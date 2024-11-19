using System;
using UnityEngine;

[Serializable]
public enum EntityType {
    Player,
    Enemy,
    Breakable
}

public class Hitbox : MonoBehaviour {
    public float timeSec = 1;
    public int damage = 1;

    public EntityType hitboxType;

    public void Death() {
        //time is in seconds.
        Destroy(gameObject, timeSec);
    }
}