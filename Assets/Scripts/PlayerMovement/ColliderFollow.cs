using System.Collections;
using TMPro;
using UnityEngine;
using Unity.XR.CoreUtils;

public class ColliderFollow : MonoBehaviour
{
    private CapsuleCollider col;
    private XROrigin xrOrigin;
    private bool isClimbing;
    private float yCenterAtStart;
    private Vector3 cameraCenter;
    private float cameraHeight;
    private bool isGrounded;

    private void Awake() {
        col = GetComponent<CapsuleCollider>();
        xrOrigin = GetComponent<XROrigin>();
    }

    private void Start() {
        yCenterAtStart = col.center.y;
    }

    private void Update() {
        cameraCenter = xrOrigin.CameraInOriginSpacePos;
        cameraHeight = xrOrigin.CameraInOriginSpaceHeight;

        if (isClimbing)
            SetClimbingCollider();
        else
            SetNormalCollider();
    }

    private void SetNormalCollider() {
        col.center = new Vector3(cameraCenter.x, yCenterAtStart, cameraCenter.z);
        col.height = cameraHeight;
    }

    private void SetClimbingCollider() {
        col.center = cameraCenter;
        col.height = 0f;
    }

    public void SetIsClimbing(bool active) {
        isClimbing = active;
        // StartCoroutine(T(active));
    }

    private IEnumerator T(bool active) {
        yield return new WaitForSeconds(0.1f);
        isClimbing = active;
    }
}
