using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunicSounds {
    public class AudioVolumeCube : AudioVolume 
    {

        public override Vector3 GetClosestVolumePoint(Vector3 point) 
        {
            point = transform.InverseTransformPoint(point);
            var result = new Vector3(
                Nearest(point.x, -0.5f, 0.5f),
                Nearest(point.y, -0.5f, 0.5f),
                Nearest(point.z, -0.5f, 0.5f));
            result = transform.TransformPoint(result);
            //result.y = transform.position.y;
            return result;
        }

        private float Nearest(float value, float min, float max) 
        {
            if (min <= value && value <= max) {
                return value;
            }
            else if (Mathf.Abs(min - value) < Mathf.Abs(max - value)) {
                return min;
            }
            else {
                return max;
            }
        }

        private void OnDrawGizmos() 
        {
            var originalColor = Gizmos.color;

            Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            var originalGizmoMatrix = Gizmos.matrix;
            Gizmos.matrix *= cubeTransform;

            var newColor = Color.magenta;
            newColor.a = 0.3f;
            Gizmos.color = newColor;
            Gizmos.DrawCube(Vector3.zero, Vector3.one);

            Gizmos.matrix = originalGizmoMatrix;

            Gizmos.color = originalColor;
        }
    }
}
