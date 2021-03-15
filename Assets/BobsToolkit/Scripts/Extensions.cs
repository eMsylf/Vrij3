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
            Vector3 referenceRelativeVector3 = reference.forward * vector3.z + reference.right * vector3.x + reference.up * vector3.y;
            return referenceRelativeVector3;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A vector3 between 0 and 1</returns>
        public static Vector3 RandomVector301()
        {
            return RandomVector3(Vector3.zero, Vector3.one);
        }

        public static Vector3 RandomVector3(Vector3 min, Vector3 max)
        {
            return new Vector3(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y),
                Random.Range(min.z, max.z)
                );
        }

        public static Vector3 RandomVector3()
        {
            return RandomVector3(1f);
        }

        public static Vector3 RandomVector3(float multiplier)
        {
            return RandomVector3(multiplier, multiplier, multiplier);
        }

        public static Vector3 RandomVector3(Vector3 multiplier)
        {
            return RandomVector3(multiplier.x, multiplier.y, multiplier.z);
        }

        public static Vector3 RandomVector3(float x, float y, float z)
        {
            return new Vector3(
                Random.Range(-x, x),
                Random.Range(-y, y),
                Random.Range(-z, z));
        }

        /// <summary>
        /// Returns a random number based on a roll between 0 and 100.
        /// </summary>
        /// <param name="trueChance">A number between 0 and 100. Lower = more chance at FALSE. Higher = more chance at TRUE.</param>
        /// <returns></returns>
        public static bool RandomTrueFalse(int trueChance = 50)
        {
            return Random.Range(0, 100) < trueChance ? true : false;
        }
    }
}