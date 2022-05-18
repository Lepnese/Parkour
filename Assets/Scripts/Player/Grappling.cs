using UnityEngine;

public class Grappling : MonoBehaviour {

    [SerializeField] private PhysicsHands2 leftHandPhysics;
    [SerializeField] private PhysicsHands2 rightHandPhysics;
    [SerializeField] private LayerMask grappableLayer;
    [SerializeField] private Transform player;
    [SerializeField] private float sphereCastRadius = 1f;

    [Header("Joint Values")] 
    [SerializeField] private float spring = 4.5f;
    [SerializeField] private float damper = 7f;
    [SerializeField] private float massScale = 4.5f;

    private PhysicsHands2 grappleHand;

    private Transform spawnPoint;
    private LineRenderer lr;

    private Vector3 grapplePoint;
    private Vector3 currentGrapplePosition;

    private SpringJoint joint;
    private float maxDistance = 100f;

    // CETTE SECTION VIENT D'UNE SOURCE EXTÉRIEURE
    private void Update() {
        grappleHand = GetGrappleHand();
        if (grappleHand && !joint) {
            lr = grappleHand.GetComponentInChildren<LineRenderer>();
            spawnPoint = grappleHand.GrappleSpawnPoint;
            StartGrapple();
        }
        else if (!grappleHand && joint) {
            StopGrapple();
        }
    }

    // Checks which hand is pressing both trigger and grip buttons
    private PhysicsHands2 GetGrappleHand() {
        var left = HandInteractionManager.Instance.GetTrackedHand(ControllerSide.Left);
        var right = HandInteractionManager.Instance.GetTrackedHand(ControllerSide.Right);
        
        if (leftHandPhysics.State == PhysHandState.Hand) {
            if (left.GetHandButton(0) && left.GetHandButton(1))
                return leftHandPhysics;
        }

        if (rightHandPhysics.State == PhysHandState.Hand) {
            if (right.GetHandButton(0) && right.GetHandButton(1))
                return rightHandPhysics;
        }
        
        return null;
    }
    
    private void LateUpdate() {
        DrawRope();
    }

    // CETTE SECTION VIENT D'UNE SOURCE EXTÉRIEURE
    private void StartGrapple() {
        Vector3 origin = grappleHand.transform.position;
        Vector3 direction = grappleHand.transform.TransformDirection(Vector3.forward);
        
        if (Physics.SphereCast(origin, sphereCastRadius, direction, out RaycastHit hit, maxDistance)) {
            if (1 << hit.collider.gameObject.layer != grappableLayer.value) return;
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point.
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = spring;
            joint.damper = damper;
            joint.massScale = massScale;

            lr.positionCount = 2;
            currentGrapplePosition = spawnPoint.position;
        }
    }
    
    // CETTE SECTION VIENT D'UNE SOURCE EXTÉRIEURE
    private void StopGrapple() {
        lr.positionCount = 0;
        Destroy(joint);
    }
    
    // CETTE SECTION VIENT D'UNE SOURCE EXTÉRIEURE
    private void DrawRope() {
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
        
        lr.SetPosition(0, spawnPoint.position);
        lr.SetPosition(1, currentGrapplePosition);
    }
}