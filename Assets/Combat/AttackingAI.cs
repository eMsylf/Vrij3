using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gyrus.Combat
{
    public class AttackingAI : MonoBehaviour
    {
        public Attack Attack;

        //public AnimationCurve AttackCurve;

        public float AttackTime;
        [SerializeField]
        private float attackTimeCurrent;

        void Update()
        {
            if (Attack.gameObject.activeSelf)
            {
                if (attackTimeCurrent > 0f)
                {
                    attackTimeCurrent -= Time.deltaTime;
                }
                else
                {
                    Attack.gameObject.SetActive(false);
                }
            }
        }



        public void DoAttack()
        {
            StartCoroutine(ManageAttack());

            // De enemy...
            // - Schreeuwt in vaste intervallen(als het niet veel werk is dan in random intervallen) voor x seconden.
            // - Implementatie van tenminste 1 idle frame, de attack announcement en 1 att frame(als het kan meteen alles dan is dat alvast klaar maar dit hangt er vanaf hoeveel tijd dit kost kwa implementatie van de sprites / animaties)
        }

        public IEnumerator ManageAttack()
        {
            Attack.gameObject.SetActive(true);
            yield return new WaitForSeconds(AttackTime);
            Attack.gameObject.SetActive(false);
        }
    }
}