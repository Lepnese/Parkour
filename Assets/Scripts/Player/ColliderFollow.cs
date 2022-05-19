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
    private Vector3 cameraCenter;
    private float cameraHeight;

    private void Awake() {
        col = GetComponent<CapsuleCollider>();
        xrOrigin = GetComponent<XROrigin>();
    }

    private IEnumerator Start() {
        yield return new WaitWhile(() => Time.timeSinceLevelLoad < 0.3f);
        
        // while (Time.timeSinceLevelLoad < 0.3f) {
        //     yield return null;
        // }
        //
        ResetCamera();
        
        col.height = cameraHeight;
        col.center = cameraCenter;
    }

    private void Update() {
        ResetCamera();
        
        if (isClimbing)
            SetClimbingCollider();
        else
            SetNormalCollider();
    }

    private void SetNormalCollider() {
        if (ColliderHeightBelowThreshold()) return;
        
        col.center = new Vector3(cameraCenter.x, cameraCenter.y / 2, cameraCenter.z);
        col.height = cameraHeight;
    }

    private void AdjustCollider() {
        StartCoroutine(AdjustCenter());
        StartCoroutine(AdjustHeight());
    }

    private IEnumerator AdjustCenter() {
        float y = col.center.y;
        while (col.center.y > cameraCenter.y / 2f) {
            y -= adjustmentSpeed / 2f * Time.fixedDeltaTime;
            col.center = new Vector3(col.center.x, y, col.center.z);
            
            yield return null;
        }
    }

    private IEnumerator AdjustHeight() {
       while (ColliderHeightBelowThreshold()) { 
           col.height += adjustmentSpeed * Time.fixedDeltaTime;
           yield return null;
       }
    }

    private void ResetCamera() {
        cameraCenter = xrOrigin.CameraInOriginSpacePos;
        cameraHeight = xrOrigin.CameraInOriginSpaceHeight;
    }

    private void SetClimbingCollider() {
        col.center = cameraCenter;
        col.height = 0f;
    }

    private bool ColliderHeightBelowThreshold() => col.height < cameraHeight - adjustmentThreshold;

    public void SetIsClimbing(bool active) {
        isClimbing = active;

        if (!active && ColliderHeightBelowThreshold()) {
            AdjustCollider();
        }
    }
}