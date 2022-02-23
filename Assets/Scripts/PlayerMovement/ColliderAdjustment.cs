using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using UnityEngine.PlayerLoop;

public class ColliderAdjustment : MonoBehaviour
{
    // private XROrigin XROrigin;
    // private CapsuleCollider col;
    // private void Awake() {
    //     XROrigin = GetComponentInParent<XROrigin>();
    //     col = GetComponent<CapsuleCollider>();
    // }
    //
    // private void Update() {
    //     var height = Mathf.Clamp(XROrigin.CameraInOriginSpaceHeight, 0, float.PositiveInfinity);
    //
    //     Vector3 center = XROrigin.CameraInOriginSpacePos;
    //     center.y = height / 2f;
    //
    //     col.height = height;
    //     col.center = center;
    // }

    private CapsuleCollider col;
    private XROrigin xrOrigin;

    private void Awake() {
        col = GetComponent<CapsuleCollider>();
        xrOrigin = GetComponent<XROrigin>();
    }

    private void Update() {
        Vector3 center = xrOrigin.CameraInOriginSpacePos;
        col.center = new Vector3(center.x, col.center.y, center.z);
        col.height = xrOrigin.CameraInOriginSpaceHeight;
    }
}
