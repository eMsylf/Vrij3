using BobJeltes;
using Combat;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Enemy))]
//[RequireComponent(typeof(Pathfinding))]
public partial class EnemyAIStationary : MonoBehaviour
{
    Enemy enemy;
    Enemy Enemy
    {
        get
        {
            if (enemy == null)
            {
                enemy = GetComponent<Enemy>();
            }
            return enemy;
        }
    }

    public Animator animator;
    [Tooltip("The amount of time the AI will spend in the Idle state, randomly picked between these values. X = min, y = max")]
    public States state;
    [Header("Idle")]
    public Vector2 IdleTime = new Vector2(1f, 5f);
    private float idleTimeCurrent = 0f;
    public UnityEvent OnIdle;
    [Header("Attack announcement")]
    [Min(0)]
    public float attackAnnouncementTime = .5f;
    private float attackAnnouncementTimeCurrent;
    public UnityEvent OnAttackAnnouncement;
    [Header("Attack")]
    public float attackTime = 5f;
    private float attackTimeCurrent = 0f;
    public UnityEvent OnAttack;


    void Update()
    {
        AnyState();
        switch (state)
        {
            case States.Idle:
                Idle();
                break;
            case States.Attack:
                Attack();
                break;
            case States.AttackAnnouncement:
                AttackAnnouncement();
                break;
        }
    }

    public void GoToState(int _state)
    {
        state = (States)_state;
    }
    public void GoToState(States _state)
    {
        state = _state;
    }

    private float CheckPlayerDistance()
    {
        return Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
    }

    private void AnyState()
    {
    }


    public void ToIdle()
    {
        if (state == States.Idle)
        {
            Debug.Log("Already in idle state");
            return;
        }

        idleTimeCurrent = Random.Range(IdleTime.x, IdleTime.y);
        Debug.Log("To Idle for " + idleTimeCurrent);
        state = States.Idle;
        OnIdle.Invoke();
        animator.SetBool("Scream", false);
    }
    private void Idle()
    {
        if (idleTimeCurrent <= 0f)
        {
            ToAttackAnnouncement();
            return;
        }
        idleTimeCurrent -= Time.deltaTime;
    }

    public void ToAttackAnnouncement()
    {
        if (state == States.AttackAnnouncement)
        {
            Debug.Log("Already in attack announcement state");
            return;
        }
        OnAttackAnnouncement.Invoke();
        attackAnnouncementTimeCurrent = attackAnnouncementTime;
    }
    private void AttackAnnouncement()
    {
        if (attackAnnouncementTimeCurrent <= 0f)
        {
            ToAttack();
            return;
        }
        attackAnnouncementTimeCurrent -= Time.deltaTime;
    }

    public void ToAttack()
    {
        if (state == States.Attack)
        {
            Debug.Log("Already in attack state");
            return;
        }
        Debug.Log("Transition to Attack");
        state = States.Attack;
        OnAttack.Invoke();
        attackTimeCurrent = attackTime;
        animator.SetBool("Scream", true);
    }
    private void Attack()
    {
        if (attackTimeCurrent <= 0f)
        {
            ToAttack();
            return;
        }
        if (!PlayerController.Instance.enabled)
            ToIdle();
        attackTimeCurrent -= Time.deltaTime;
    }
}
