using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BobJeltes.Extensions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RanchyRats.Gyrus
{
    public class Character : MonoBehaviour, 
        CombatProperties.IKillable, 
        //CombatProperties.IDamagable<float>, 
        CombatProperties.ICanAttack
    {
        public CharacterProfile profile;

        [System.Serializable]
        public struct Modifiers
        {
            public float energy;
            public float health;
            public float speed;
            public float attackFrequency;
            public float size;
        }
        [Tooltip("Adds an amount to the associated statistic in the character profile")]
        public Modifiers modifiers;

        [Header("Optional components")]
        public Stat health;
        public Stat stamina;
        public Movement movement;
        [System.Serializable]
        public struct Events
        {
            public UnityEvent onHit;
            public UnityEvent onDeath;
            public UnityEvent onRevival;
        }
        public Events events;
        public List<GameObject> HitObjects = new List<GameObject>();
        [Tooltip("If disabled, every hit object in the list is spawned upon hit.")]
        public bool PickRandomHitObject;
        public List<GameObject> DeathObjects = new List<GameObject>();
        [Tooltip("If disabled, every death object in the list is spawned upon death.")]
        public bool PickRandomDeathObject;
        public List<GameObject> BloodSplatters = new List<GameObject>();
        [Tooltip("If enabled, only one bloodsplatter is instantiated upon death. If disabled, every bloodsplatter in the list is spawned upon death.")]
        public bool PickRandomBloodSplatter;

        public CharacterController Controller;
        public Animator Animator;

        public virtual void Die()
        {
            Debug.Log(name + " died", this);
            if (PickRandomDeathObject)
            {
                SpawnObjectAtOwnHeight(DeathObjects[Random.Range(0, DeathObjects.Count)]);
            }
            else
            {
                foreach (GameObject obj in DeathObjects)
                {
                    SpawnObjectAtOwnHeight(obj);
                }
            }
            if (PickRandomBloodSplatter)
            {
                SpawnObjectAtOwnHeight(BloodSplatters[Random.Range(0, BloodSplatters.Count)]);
            }
            else
            {
                foreach (GameObject obj in BloodSplatters)
                {
                    SpawnObjectAtOwnHeight(obj);
                }
            }

            if (events.onDeath != null)
                events.onDeath.Invoke();

            if (Controller != null)
            {
                Controller.attacking.InterruptCharge();
            }
            if (this is PlayerCharacter)
                GameManager.Instance.PlayerDeath(this as PlayerCharacter);
            //----------------------------------------------------------- Character dies
            if (sounds.death != null)
                sounds.death.Play();

            gameObject.SetActive(false);
        }
        [System.Serializable]
        public struct Sounds
        {
            public FMODUnity.StudioEventEmitter death;
            public FMODUnity.StudioEventEmitter getHit;
        }
        public Sounds sounds;
        private float InvincibilityTime = 0f;
        public bool Invincible
        {
            get
            {
                return InvincibilityTime > 0f;
            }
        }
        [System.Serializable]
        public class TouchDamage
        {
            public bool enabled;
            public int damage = 0;
            public float invincibilityTime = 1f;
            public LayerMask layers;
            public float force = 1f;
        }
        public TouchDamage touchDamage;

        public virtual void OnEnable()
        {
            OnEnableTasks();
        }

        internal void OnEnableTasks()
        {
            //Debug.Log("Set current health and stamina of " + name + " to max", this);
            if (health != null)
            {
                health.SetCurrent(health.MaxValue);
            }
            if (stamina != null)
            {
                stamina.SetCurrent(stamina.MaxValue);
            }
        }

        private void OnDisable()
        {
            OnDisableTasks();
        }

        internal void OnDisableTasks()
        {
            //Debug.Log(name + " disabled", this);
        }


        public virtual void Update()
        {
            ManageInvincibility();
        }

        void ManageInvincibility()
        {
            if (Invincible)
            {
                InvincibilityTime -= Time.deltaTime;
            }
        }

        public UnityEvent OnAttackAnnouncement;
        public void AnnounceAttack()
        {
            OnAttackAnnouncement.Invoke();
        }

        public void Attack()
        {
            throw new System.NotImplementedException();
        }

        public void SpawnObjectAtOwnHeight(GameObject prefab)
        {
            if (prefab != null)
                Instantiate(prefab, new Vector3(transform.position.x, prefab.transform.position.y, transform.position.z), prefab.transform.rotation).transform.localScale = transform.localScale;
        }

        public void TakeDamage(float damageTaken, float invincibilityTime = 0f, Character damageSource = null)
        {
            health.SetCurrent(Mathf.Clamp(health.Value - damageTaken, 0, health.Value));
            InvincibilityTime += invincibilityTime;
            if (damageSource != null)
            {
                Debug.Log(damageSource.name + " hit enemy " + name + " for " + damageTaken + " damage.");
            }

            events.onHit.Invoke();

            if (PickRandomHitObject)
            {
                Instantiate(HitObjects[Random.Range(0, HitObjects.Count)]);
            }
            else
            {
                foreach (GameObject obj in HitObjects)
                {
                    Instantiate(obj, new Vector3(transform.position.x, obj.transform.position.y, transform.position.z), obj.transform.rotation);
                }
            }
            //Debug.Log(name + " took " + damageTaken + " damage", this);
            if (health.Value <= 0)
            {
                //Debug.Log(name + " should die now", this);
                Die();
            }
            else if (sounds.getHit != null)
                sounds.getHit.Play();
        }

        private void OnCollisionEnter(Collision collision)
        {
            Character otherFighter = collision.gameObject.GetComponent<Character>();
            if (otherFighter == null)
            {
                return;
            }

            if (touchDamage.layers != (touchDamage.layers.value | (1 << otherFighter.gameObject.layer)))
            {
                return;
            }

            if (otherFighter.Invincible)
            {
                return;
            }

            if (touchDamage.enabled)
            {
                if (collision.rigidbody != null)
                {
                    Vector3 forceVector = collision.rigidbody.position - transform.position;
                    forceVector.Normalize();
                    collision.rigidbody.AddForce(touchDamage.force * forceVector, ForceMode.Impulse);
                }
                if (touchDamage.damage != 0)
                    otherFighter.TakeDamage(touchDamage.damage, touchDamage.invincibilityTime, this);
                Debug.Log(otherFighter + " takes touch damage from " + name);
            }
        }

        [SerializeField] private Vector3 respawnPoint = new Vector3();
        public Vector3 RespawnPoint
        {
            get
            {
                if (RespawnOverride == null)
                    return respawnPoint;
                return RespawnOverride.position;
            }
        }
        public Transform RespawnOverride;

        // TODO: Verander dit naar een GameManager functie die de juiste instance van de character respawnt
        public void Respawn()
        {
            transform.position = RespawnPoint;
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.white;
            Handles.DrawLine(transform.position, RespawnPoint);
            Gizmos.DrawIcon(RespawnPoint, "Player Spawn Pos");
        }
#endif
    }
}