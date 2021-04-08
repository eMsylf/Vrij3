using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RanchyRats.Gyrus
{
    public class Targeting : CharacterComponent
    {
        public GameObject DebugTarget;
        public GameObject GetTarget(Vector3 position)
        {
            if (DebugTarget != null)
                return DebugTarget;
            Collider[] colliders = Physics.OverlapSphere(position, Radius, Targetable);
            if (colliders.Length > 0)
                return colliders[0].gameObject;
            return null;
        }
        public float Radius = 3f;
        public Color RadiusColor = Color.white;
        public LayerMask Targetable;

        internal void LockOn(Vector3 position)
        {
            Debug.Log("Lock on");
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            Handles.color = RadiusColor;
            Handles.DrawWireDisc(transform.position, transform.up, Radius);
        }
#endif
    }
}
