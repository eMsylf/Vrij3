using RanchyRats.Gyrus;
using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnxietyBoss : Enemy
{
    public void PlayIdleSound()
    {
        Debug.Log("Play idle sound");
    }

    public void PlayTeethClackSound()
    {
        Debug.Log("Play teeth clack sound");
    }

    public void PlayAttAnnounceSound()
    {
        Debug.Log("Play attack announce sound");
    }

    public void PlayScreamAttackSound()
    {
        Debug.Log("Play scream attack sound");
    }

    public void PlayEyePopSound()
    {
        Debug.Log("Play eye pop sound");
    }

    public void PlayDeathSound()
    {
        Debug.Log("Play death sound");
    }

    public override void Die()
    {
        base.Die();
        if (Animator == null)
        {
            Debug.LogError("Animator not assigned", gameObject);
            return;
        }

        Animator.SetTrigger("Death");
    }

    public void StartAttack(string name)
    {
        Attack attack = attacks.Find(x => x.name == name);
        if (attack == null)
        {
            Debug.LogError("No attack with name " + name + " found in attack list of " + gameObject.name, this);
            return;
        }
        attack.Start();
    }

    public List<Attack> attacks = new List<Attack>();
    [System.Serializable]
    public class Attack
    {
        public string name = "";
        public UnityEvent OnAttack = new UnityEvent();

        public void Start()
        {
            OnAttack.Invoke();
        }
    }
}
