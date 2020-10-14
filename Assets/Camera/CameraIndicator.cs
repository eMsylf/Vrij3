using UnityEngine;

public class CameraIndicator : MonoBehaviour {
    public bool ShowIndicator;
    [Range(.2f, 1f)]
    public float IndicatorSize = .5f;

    public Camera MainCamera;
    public CameraFollow CameraFollow;
    public Transform InnerMount;

    //private void OnDrawGizmos() {
    //    if (!ShowIndicator) {
    //        return;
    //    }
    //    Vector3 cubeSize = new Vector3(IndicatorSize, IndicatorSize, IndicatorSize);
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireCube(InnerMount.position, cubeSize);
    //    //Gizmos.DrawLine(InnerMount.position, InnerMount.position + InnerMount.up * CameraFollow.CameraDistance);
    //    //Gizmos.DrawWireSphere(InnerMount.position + InnerMount.up * CameraFollow.CameraDistance, IndicatorSize);
    //}
}
