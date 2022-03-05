using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grappling : MonoBehaviour {

    [SerializeField] private PhysicsHands2 leftHandPhysics;
    [SerializeField] private PhysicsHands2 rightHandPhysics;
    [SerializeField] private LayerMask whatIsGrappable;
    [SerializeField] private Transform player;
    [SerializeField] private float sphereCastRadius = 1f;

    private PhysicsHands2 grappleHand;
    private Hand leftHand;
    private Hand rightHand;
    
    private Transform spawnPoint;
    private LineRenderer lr;
    private Vector3 grapplePoint;
    private float maxDistance = 100f;
    private SpringJoint joint;
    private bool pressing;

    private void Awake() {
        leftHand = leftHandPhysics.targetController;
        rightHand = rightHandPhysics.targetController;
    }

    private void Update() {
        Debug.DrawLine(leftHand.transform.position, leftHand.transform.TransformDirection(Vector3.forward) * 100f);

        grappleHand = GetGrappleHand();
        if (grappleHand && !joint) {
            lr = grappleHand.GetComponentInChildren<LineRenderer>();
            spawnPoint = grappleHand.transform.Find("Grapple Spawn Point");
            StartGrapple();
        }
        else if (!grappleHand && joint) {
            StopGrapple();
        }
    }

    private PhysicsHands2 GetGrappleHand() {
        if (leftHand.TriggerValue > 0.1f && leftHand.GripValue > 0.1f)
            return leftHandPhysics;
        if (rightHand.TriggerValue > 0.1f && rightHand.GripValue > 0.1f)
            return rightHandPhysics;
        return null;
    }
    
    private void LateUpdate() {
        DrawRope();
    }

    private void StartGrapple() {
        Vector3 origin = grappleHand.transform.position;
        Vector3 direction = grappleHand.transform.TransformDirection(Vector3.forward);
        if (Physics.SphereCast(origin, sphereCastRadius, direction, out RaycastHit hit, maxDistance, whatIsGrappable)) {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point.
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            //Adjust these values to fit your game.
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = spawnPoint.position;
        }
    }
    
    private void StopGrapple() {
        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;
    
    private void DrawRope() {
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
        
        lr.SetPosition(0, spawnPoint.position);
        lr.SetPosition(1, currentGrapplePosition);
    }
}