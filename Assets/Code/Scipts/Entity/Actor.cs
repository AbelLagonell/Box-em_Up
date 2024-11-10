using System.Collections;
using UnityEngine;

public enum StateOrder {
    Attack,
    Ability,
    Blocking
}

public class Actor : Entity {
    protected static readonly int At = Animator.StringToHash("IsAttacking");   //Bool
    protected static readonly int Mv = Animator.StringToHash("IsMoving");      //Bool
    protected static readonly int Ht = Animator.StringToHash("Hit");           //Trigger
    protected static readonly int Dd = Animator.StringToHash("Dead");          //Trigger
    protected static readonly int AtSp = Animator.StringToHash("AttackSpeed"); //Float

    [SerializeField] protected AnimationClip[] animations;

    public float speed = 1;
    public float attackSpeed = 1;
    public int defense, attack = 1;

    [SerializeField] protected GameObject hitbox;
    [SerializeField] protected Vector3 sizeHitbox = Vector3.one;
    protected Animator AnimatorController;
    protected bool[] CurrentState = { false, false, false }; 
    protected Rigidbody Rb;

    private void Start() {
        Rb = GetComponent<Rigidbody>();
        AnimatorController = GetComponent<Animator>();
    }

    private void FixedUpdate() {
        Movement();
    }

    protected override void DecreaseHealth(int amount) {
        if (!CurrentState[(int)StateOrder.Blocking]) return;
        health = amount - defense;
        if (health <= 0) TriggerDeath();
    }

    protected virtual void Movement() {
        //Move towards the player crudely and then switch to navmesh would be better
        //Animation for movement
    }


    //? Once the player is close enough stop moving and then attack towards the last known location.

    protected virtual void Attack() {
        var cState = AnimatorController.GetCurrentAnimatorStateInfo(0);
        if (cState.IsName("Attack") && cState.normalizedTime >= 1) return;
        AnimatorController.SetFloat(AtSp, attackSpeed);
        StartCoroutine(ResetFlag(At, (int)StateOrder.Attack, 
                                 animations[(int)StateOrder.Attack].length * (1+1/attackSpeed)));
        SpawnHitbox(animations[0].length * (1 + 1 / attackSpeed));
        SetAnimationBool(true, At, (int)StateOrder.Attack);
    }

    protected virtual void SpawnHitbox(float duration = .75f) {
        if (CurrentState[(int)StateOrder.Attack]) return;
        var newHitBox = Instantiate(hitbox, 
                                    transform.forward + transform.position + transform.up * 1.5f, 
                                    transform.rotation, transform);
        newHitBox.transform.localScale = sizeHitbox;
        var script = newHitBox.GetComponent<Hitbox>();
        script.damage = attack;
        script.timeSec = duration;
        script.Death();
    }

    protected void SetAnimationBool(bool value, int animationHash, int stateIndex) {
        AnimatorController.SetBool(animationHash, value);
        CurrentState[stateIndex] = value;
    }
    
   protected IEnumerator ResetFlag(int animationHash, int stateIndex, float delay) {
           yield return new WaitForSeconds(delay);
           SetAnimationBool(false, animationHash, stateIndex);
   } 
}