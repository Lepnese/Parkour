using UnityEngine;

public class Grappling : MonoBehaviour {

    [SerializeField] private Hand hand;
    [SerializeField] private LayerMask whatIsGrappleable;
    [SerializeField] private Transform grappleSpawnPoint, playerCamera, player;
    
    private LineRenderer lr;
    private Vector3 grapplePoint;
    private float maxDistance = 100f;
    private SpringJoint joint;
    private bool pressing;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    private void Update() {
        bool grappleActivated = hand.TriggerValue > 0.1f && hand.GripValue > 0.1f;
        if (grappleActivated) {
            pressing = true;
            StartGrapple();
        }
        else if (pressing) {
            pressing = false;
            StopGrapple();
        }
    }

    private void LateUpdate() {
        DrawRope();
    }

    private void StartGrapple() {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance, whatIsGrappleable)) {
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
            currentGrapplePosition = grappleSpawnPoint.position;
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
        
        lr.SetPosition(0, grappleSpawnPoint.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling() {
        return joint != null;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }
}