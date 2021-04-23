using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BobJeltes.Extensions;
using Gyrus.Combat;
using Gyrus;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RanchyRats.Gyrus
{
    public class Character : MonoBehaviour, CombatProperties.IKillable
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
        [Tooltip("Adds an amount to the associated property in the character profile")]
        public Modifiers modifiers;

        [Header("Optional components")]
        public CharacterStatistic health;
        public CharacterStatistic stamina;

        [SerializeField]
        private CharacterController controller;
        public CharacterController Controller
        {
            get
            {
                if (controller == null)
                    controller = GetComponent<CharacterController>();
                return controller;
            }
        }

        [SerializeField]
        private Animator animator;
        public Animator Animator
        {
            get
            {
                if (animator == null)
                    animator = GetComponent<Animator>();
                return animator;
            }
        }

        [System.Serializable]
        public struct CharacterEvent
        {
            public FMODUnity.StudioEventEmitter sound;
            [Tooltip("All objects are spawned from this list.")]
            public List<GameObject> AllObjectsList;
            [Tooltip("A random object is spawned from this list.")]
            public List<GameObject> RandomObjectList;
            public UnityEvent unityEvent;

            /// <summary>
            /// Starts the event's sound, spawns all objects and a random object, and invokes the unity event
            /// </summary>
            /// <param name="position">The position at which the objects should be spawned</param>
            /// <param name="scale">The scale at which the objects should be spawned</param>
            public void FireEverything(Vector3 position, Vector3 scale)
            {
                if (sound != null) sound.Play();
                SpawnCollection(position, scale);
                SpawnRandom(position, scale);
                unityEvent.Invoke();
            }

            public void SpawnCollection(Vector3 position, Vector3 scale)
            {
                foreach (GameObject obj in AllObjectsList)
                {
                    Instantiate(obj, position, Quaternion.identity).transform.localScale = scale;
                }
            }

            public void SpawnRandom(Vector3 position, Vector3 scale)
            {
                if (RandomObjectList.Count == 0) return;
                Instantiate(
                    RandomObjectList[Random.Range(0, RandomObjectList.Count)],
                    position,
                    Quaternion.identity).
                    transform.localScale = scale;
            }
        }
        [Header("Character events")]
        public CharacterEvent GetHit = new CharacterEvent();
        public CharacterEvent Death = new CharacterEvent();
        public CharacterEvent Revival = new CharacterEvent();
        [System.Serializable]
        public class TouchDamage
        {
            public bool enabled;
            public int damage = 0;
            public float invincibilityTime = 1f;
            public LayerMask layers;
            public float force = 1f;
        }
        [Space]
        public TouchDamage touchDamage;

        private float InvincibilityTime = 0f;
        public bool Invincible
        {
            get
            {
                return InvincibilityTime > 0f;
            }
        }

        public virtual void OnEnable()
        {
            if (health != null)
            {
                health.SetValueWithoutEvent(health.MaxValue);
            }
            if (stamina != null)
            {
                stamina.SetValueWithoutEvent(stamina.MaxValue);
            }
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

        private void OnCollisionEnter(Collision collision)
        {
            Character otherCharacter = collision.gameObject.GetComponent<Character>();
            if (otherCharacter == null)
            {
                return;
            }

            if (touchDamage.layers != (touchDamage.layers.value | (1 << otherCharacter.gameObject.layer)))
            {
                return;
            }

            if (otherCharacter.Invincible)
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
                    otherCharacter.TakeDamage(touchDamage.damage, touchDamage.invincibilityTime, this);
                Debug.Log(otherCharacter + " takes touch damage from " + name);
            }
        }

        public void SpawnObjectAtOwnHeight(GameObject prefab)
        {
            if (prefab != null)
                Instantiate(prefab, new Vector3(transform.position.x, prefab.transform.position.y, transform.position.z), prefab.transform.rotation).transform.localScale = transform.localScale;
        }

        public void TakeDamage(float damageTaken, float invincibilityTime = 0f, Character damageSource = null)
        {
            health.Value = Mathf.Clamp(health.Value - damageTaken, 0, health.Value);
            InvincibilityTime += invincibilityTime;
            if (damageSource != null)
            {
                Debug.Log(damageSource.name + " hit enemy " + name + " for " + damageTaken + " damage.");
            }

            GetHit.unityEvent.Invoke();

            GetHit.SpawnRandom(transform.position, transform.lossyScale);

            //Debug.Log(name + " took " + damageTaken + " damage", this);
            if (health.Value <= 0)
            {
                //Debug.Log(name + " should die now", this);
                Die();
            }
            else if (GetHit.sound != null)
                GetHit.sound.Play();
        }

        public virtual void Die()
        {
            Debug.Log(name + " died", this);

            Death.SpawnCollection(transform.position, transform.lossyScale);
            Death.SpawnRandom(transform.position, transform.lossyScale);
            Death.unityEvent.Invoke();

            if (Controller.PlayerController != null)
                GameManager.Instance.PlayerDeath(Controller.Character);
            //----------------------------------------------------------- Character dies
            if (Death.sound != null)
                Death.sound.Play();

            gameObject.SetActive(false);
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