using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.Extensions
{
    public static class Extensions
    {
        public static IEnumerator SetActive(this GameObject gameObject, bool value, float t)
        {
            yield return new WaitForSeconds(t);
            gameObject.SetActive(value);
        }

        public static Vector3 ConvertToObjectRelative(this Vector3 vector3, Transform reference, bool flatten = false, bool normalize = false)
        {
            Vector3 referenceForward = reference.forward;
            if (flatten)
                referenceForward.Scale(new Vector3(1, 0, 1));
            if (normalize)
                referenceForward.Normalize();
            Vector3 referenceRight = reference.right;
            Vector3 referenceRelativeVector3 = referenceForward * vector3.z + referenceRight * vector3.x;
            return referenceRelativeVector3;
        }
    }

    
    
}