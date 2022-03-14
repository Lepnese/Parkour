using System.Collections;
using TMPro;
using UnityEngine;
using Unity.XR.CoreUtils;

public class ColliderAdjustment : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float climbingColliderHeight;
    
    private CapsuleCollider col;
    private XROrigin xrOrigin;
    private bool isClimbing;
    private float yCenterAtStart;
    private ClimbState currentClimbState;

    private void Awake() {
        col = GetComponent<CapsuleCollider>();
        xrOrigin = GetComponent<XROrigin>();
    }

    private void Start() {
        yCenterAtStart = col.center.y;
        StartCoroutine(NormalCollider());
    }

    private IEnumerator NormalCollider() {
        currentClimbState = ClimbState.NotClimbing;
        while (!isClimbing) {
            Vector3 cameraCenter = xrOrigin.CameraInOriginSpacePos;
            col.center = new Vector3(cameraCenter.x, yCenterAtStart, cameraCenter.z);
            col.height = xrOrigin.CameraInOriginSpaceHeight;
            yield return null;
        }
    }

    private IEnumerator ClimbingCollider() {
        currentClimbState = ClimbState.Climbing;
        while (isClimbing) {
            col.center = xrOrigin.CameraInOriginSpacePos;
            col.height = climbingColliderHeight;
            yield return null;
        }
    }

    private IEnumerator StopClimbing() {
        if (col.height == climbingColliderHeight) {
            StartCoroutine(NormalCollider());
            yield return null;
        }
        
        currentClimbState = ClimbState.Transition;
            
        Vector3 curCenter = col.center;
        Vector3 targetCenter = xrOrigin.CameraInOriginSpacePos;

        float i = 0f;
        while (i < 1f) {
            i += Time.fixedDeltaTime * 5f;
            
            float curHeight = col.height;
            float targetHeight = xrOrigin.CameraInOriginSpaceHeight;
            
            col.height = Mathf.Lerp(curHeight, targetHeight, i);
            // col.center = Vector3.Lerp(curCenter, targetCenter, i);
            yield return null;
        }

        isClimbing = false;
        StartCoroutine(NormalCollider());
    }

    public void SetIsClimbing(bool active) {
        text.text = currentClimbState.ToString();
        
        isClimbing = active && currentClimbState != ClimbState.Transition;
        StartCoroutine(isClimbing ? ClimbingCollider() : StopClimbing());
    }
}

public enum ClimbState
{
    Climbing,
    NotClimbing,
    Transition
}
