using BobJeltes;
using Combat;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Enemy))]
//[RequireComponent(typeof(Pathfinding))]
public class EnemyAIStationary : MonoBehaviour
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

    [Tooltip("The amount of time the AI will spend in the Idle state, randomly picked between these values. X = min, y = max")]
    public Vector2 IdleTime = new Vector2(1f, 5f);
    private float idleTimeCurrent = 0f;

    public SpriteFlash AttackAnnouncement = new SpriteFlash();

    public enum States
    {
        Idle,
        Attack
    }
    public States state;

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
        }
    }


    public UnityEvent onPlayerClose;

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

    #region States
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
    }
    private void Idle()
    {
        if (idleTimeCurrent > 0f)
        {
            idleTimeCurrent -= Time.deltaTime;
        }
        else
        {
            ToAttack();
        }
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
    }
    private void Attack()
    {
        if (!PlayerController.Instance.enabled)
            ToIdle();
    }
    #endregion

//#if UNITY_EDITOR
//    public Color TargetingColor = Color.yellow;
//    public Color AttackColor = Color.red;
//    private void OnDrawGizmosSelected()
//    {
//        Handles.color = TargetingColor;
//        Handles.DrawWireDisc(transform.position, transform.up, TargetingProximity);
//        Handles.color = AttackColor;
//        Handles.DrawWireDisc(transform.position, transform.up, AttackingProximity);
//    }
//#endif
}
