using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum StateOrder {
    Attack,
    Ability,
    Blocking,
    Hit
}

public class Actor : Entity {
    protected static readonly int At = Animator.StringToHash("IsAttacking");   //Bool
    protected static readonly int Mv = Animator.StringToHash("IsMoving");      //Bool
    protected static readonly int Ht = Animator.StringToHash("Hit");           //Trigger
    protected static readonly int Dd = Animator.StringToHash("Dead");          //Trigger
    protected static readonly int AtSp = Animator.StringToHash("AttackSpeed"); //Float

    [SerializeField] protected GameObject hitbox;
    [SerializeField] protected Vector3 sizeHitbox = Vector3.one;

    [Header("")] [SerializeField] protected AnimationClip[] animations;

    [Header("Stats")] public float speed = 1;
    public float attackSpeed = 1;
    public int defense, attack = 1;


    private UnityEvent _onDeath;
    protected Animator AnimatorController;
    protected bool[] CurrentState = { false, false, false, false };
    protected Rigidbody Rb;

    // Enemy Stuff
    private NavMeshAgent _agent;
    private float _detectionRadius;

    protected new void Start() {
        base.Start();
        Rb                 = GetComponent<Rigidbody>();
        AnimatorController = GetComponent<Animator>();
    }

    private void FixedUpdate() {
        Movement();
    }

    protected override void DecreaseHealth(int amount) {
        if (CurrentState[(int)StateOrder.Blocking]) return;
        health -= amount - defense;
        GotHit();
        if (health <= 0) TriggerDeath();
    }

    protected virtual void Movement() {
        //Move towards the player crudely and then switch to navmesh would be better
        //Animation for movement
        _agent.speed = 0;
        if (!CurrentState.Contains(true)) _agent.speed = speed;
        Debug.DrawCircle(transform.position, _detectionRadius, 32, Color.cyan);
        _agent.SetDestination(MainCharacter.Instance.transform.position);
        UnityEngine.Debug.DrawLine(transform.position, MainCharacter.Instance.transform.position, Color.red);
    }

    protected override void TriggerDeath() {
        Waves.Instance.EnemyDied();
        base.TriggerDeath();
    }

    protected void GotHit() {
        SetAnimationBool(true, Ht, (int)StateOrder.Hit);
        StartCoroutine(ResetFlag(Ht, (int)StateOrder.Hit, animations[(int)StateOrder.Hit].length));
    }

    //? Once the player is close enough stop moving and then attack towards the last known location.

    protected virtual void Attack() {
        var cState = AnimatorController.GetCurrentAnimatorStateInfo(0);
        if (cState.IsName("Attack") && cState.normalizedTime >= 1) return;
        AnimatorController.SetFloat(AtSp, attackSpeed);
        StartCoroutine(ResetFlag(At, (int)StateOrder.Attack,
                                 animations[(int)StateOrder.Attack].length * (1 + 1 / attackSpeed)));
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
        script.damage  = attack;
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