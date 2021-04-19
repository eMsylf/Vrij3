using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyrus.Combat
{
    public class CombatProperties
    {
        public interface IDamagable<T>
        {
            void TakeDamage(T damage);
        }

        public interface IDamageDealer<T>
        {
            void DealDamage(T damage);
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
