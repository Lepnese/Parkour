using System.Collections;
using UnityEngine;
using Unity.XR.CoreUtils;

public class ColliderFollow : MonoBehaviour
{
    [SerializeField] private float adjustmentSpeed = 5f;
    [SerializeField] private float adjustmentThreshold = 0.1f;

    private CapsuleCollider col;
    private XROrigin xrOrigin;
    private bool isClimbing;
    private float yCenterAtStart;
    private Vector3 cameraCenter;
    private float cameraHeight;

    private void Awake() {
        col = GetComponent<CapsuleCollider>();
        xrOrigin = GetComponent<XROrigin>();
    }
    
    private void Update() {
        cameraCenter = xrOrigin.CameraInOriginSpacePos;
        cameraHeight = xrOrigin.CameraInOriginSpaceHeight;
        
        if (Time.timeSinceLevelLoad < 0.3f) {
            yCenterAtStart = col.center.y;
            col.height = cameraHeight;
            col.center = cameraCenter;
            return;
        }
        
        if (isClimbing)
            SetClimbingCollider();
        else
            SetNormalCollider();
    }

    private void SetNormalCollider() {
        col.center = new Vector3(cameraCenter.x, cameraCenter.y / 2, cameraCenter.z);
        
        if (ColliderHeightBelowThreshold()) return;
        col.height = cameraHeight;
    }

    private IEnumerator AdjustCollider() {
        while (ColliderHeightBelowThreshold()) {
            col.height += adjustmentSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private void SetClimbingCollider() {
        col.center = cameraCenter;
        col.height = 0f;
    }

    private bool ColliderHeightBelowThreshold() => col.height < cameraHeight - adjustmentThreshold;

    public void SetIsClimbing(bool active) {
        isClimbing = active;
        
        if (active) return;
        
        if (ColliderHeightBelowThreshold())
            StartCoroutine(AdjustCollider());
    }
    
}
