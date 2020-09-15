using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class CombatProperties
    {
        public interface IDamagable<T>
        {
            void TakeDamage(T damageTaken);
        }

        public interface IKillable
        {
            void Die();
        }

        public interface ICanAttack
        {
            void Attack();
        }
    }
}
