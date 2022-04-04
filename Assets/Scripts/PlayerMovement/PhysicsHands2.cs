using System;
using System.Collections;
using UnityEngine;

public class PhysicsHands2 : MonoBehaviour
{
    public Collider col;
    [Header("Grappling")]
    [SerializeField] private Transform grappleSpawnPoint;
    [Header("Climbing")]
    [SerializeField] private float minHeightFromTop = 0.3f;
    [SerializeField] private PhysicsHandsEvent onClimbStart;
    [SerializeField] private PhysicsHandsEvent onClimbEnd;
    [Header("PID Movement Values")]
    [SerializeField] private float frequency = 50f;
    [SerializeField] private float damping = 1f;
    [SerializeField] private float rotFrequency = 100f;
    [SerializeField] private float rotDamping = 0.9f;
    [Header("Rigidbody Movement Values")]
    [SerializeField] private Hand trackedHand;
    [SerializeField] private float maxDistanceFromTrackedHand = 2f;
    [SerializeField] private float physicsRange = 0.15f;
    [SerializeField] private float climbForce = 1000f;
    [SerializeField] private float climbDrag = 500f;
    [SerializeField] private LayerMask collisionLayers;
    [Header("Player References")]
    [SerializeField] private Rigidbody playerRb;

    private Rigidbody rb;
    private Transform targetTransform;
    private Vector3 previousPosition;
    private bool canClimb;
    private Collider currentLedge;
    private Collider fullyClimbable;
    private Collider closestCollider;
    private SphereCollider grabRange;
    private Vector3 grabPoint;

    private HandPresence handPresence;

    public Transform GrappleSpawnPoint => grappleSpawnPoint;
    public Hand TrackedHand => trackedHand;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        handPresence = GetComponent<HandPresence>();
        grabRange = GetComponent<SphereCollider>();
    }

    private void Start() {
        targetTransform = trackedHand.transform;
        rb.maxAngularVelocity = float.PositiveInfinity;
        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
        previousPosition = transform.position;
    }
    
    private void Update() {
        var pos = transform.position;
        
        if (trackedHand.GetHandButton(1) && canClimb) {
            Climb();
            return;
        }

        if (Vector3.Distance(transform.position, trackedHand.transform.position) > maxDistanceFromTrackedHand)
            transform.position = trackedHand.transform.position;
        
        bool isCloseToObject = Physics.CheckSphere(pos, physicsRange, collisionLayers);
        if (isCloseToObject) {
            // Controller moves using PID (rigidbody motion)
            PIDMovement();
            PIDRotation();
        }
        else {
            // Controller moves using transform (no rigidbody motion)
            TransformMovement();
            TransformRotation();
        }
    }

    private void FixHandPosition() {
        transform.position = grabPoint;
        
        if (IsGrabbingCorner(grabPoint))
            handPresence.OnEdge(true);
        else
            handPresence.OnFlatSurface(true);
        
        rb.constraints = RigidbodyConstraints.FreezeAll;
        canClimb = true;
        onClimbStart.Raise(this);
    }

    private void OnDrawGizmos() {
        var point = new Vector3(-0.5f, 13.64f, -0.3f);
        Gizmos.DrawSphere(point, 0.02f);

        var edgePoint = point;
        edgePoint.x = col.bounds.center.x - col.bounds.extents.x;
        Gizmos.DrawSphere(edgePoint, 0.02f);
    }

    // CETTE SECTION VIENT D'UNE SOURCE EXTÉRIEURE
    private bool IsGrabbingCorner(Vector3 grabPoint) {
        Vector3 localPoint = currentLedge.transform.InverseTransformPoint(grabPoint);
        Vector3 localDir = localPoint.normalized;

        float upDot = Vector3.Dot(localDir, Vector3.up);
        float fwdDot = Vector3.Dot(localDir, Vector3.forward);
        float rightDot = Vector3.Dot(localDir, Vector3.right);

        float topValue = Mathf.Abs(upDot);
        float sideValue = Mathf.Max(Mathf.Abs(fwdDot), Mathf.Abs(rightDot));
        
        return topValue <= sideValue + 0.1f;
    }
    
    // CETTE SECTION VIENT D'UNE SOURCE EXTÉRIEURE
    private void Climb() {
        Vector3 displacementFromResting = transform.position - targetTransform.position;
        Vector3 force = displacementFromResting * climbForce;
        float drag = GetDrag();
        
        playerRb.AddForce(force, ForceMode.Acceleration);
        playerRb.AddForce(drag * -playerRb.velocity * climbDrag, ForceMode.Acceleration);
    }
    
    // CETTE SECTION VIENT D'UNE SOURCE EXTÉRIEURE
    private float GetDrag() {
        Vector3 handVelocity = (targetTransform.localPosition - previousPosition) / Time.fixedDeltaTime;
        float drag = 1 / handVelocity.magnitude + 0.01f;
        drag = drag > 1 ? 1 : drag;
        drag = drag > 0.03f ? 0.03f : drag;
        previousPosition = transform.position;
        return drag;
    }

    private void TransformMovement() {
        rb.velocity = Vector3.zero;
        transform.localPosition = targetTransform.position;
    }

    private void TransformRotation() {
        rb.angularVelocity = Vector3.zero;
        transform.localRotation = targetTransform.rotation;
    }
    
    // CETTE SECTION VIENT D'UNE SOURCE EXTÉRIEURE
    private void PIDMovement() {
        float kp = (6f * frequency) * (6f * frequency) * 0.25f;
        float kd = 4.5f * frequency * damping;
        float g = 1 / (1 + kd * Time.fixedDeltaTime + kp * Time.fixedDeltaTime * Time.fixedDeltaTime);
        float ksg = kp * g;
        float kdg = (kd + kp * Time.fixedDeltaTime) * g;
        Vector3 force = (targetTransform.position - transform.position) * ksg +
                        (playerRb.velocity - rb.velocity) * kdg;
        rb.AddForce(force, ForceMode.Acceleration);
    }
    
    // CETTE SECTION VIENT D'UNE SOURCE EXTÉRIEURE
    private void PIDRotation() {
        float kp = (6f * rotFrequency) * (6f * rotFrequency) * 0.25f;
        float kd = 4.5f * rotFrequency * rotDamping;
        float g = 1 / (1 + kd * Time.fixedDeltaTime + kp * Time.fixedDeltaTime * Time.fixedDeltaTime);
        float ksg = kp * g;
        float kdg = (kd + kp * Time.fixedDeltaTime) * g;
        Quaternion q = targetTransform.rotation * Quaternion.Inverse(transform.rotation);
        if (q.w < 0) {
            q.x = -q.x;
            q.y = -q.y;
            q.z = -q.z;
            q.w = -q.w;
        }

        q.ToAngleAxis(out float angle, out Vector3 axis);
        axis.Normalize();
        axis *= Mathf.Deg2Rad;
        Vector3 torque = ksg * axis * angle - rb.angularVelocity * kdg;
        rb.AddTorque(torque, ForceMode.Acceleration);
    }
    
    public void OnGripBtnDown(Hand hand) {
        if (hand != trackedHand) return;

        StartCoroutine(CheckForLedge());
    }

    private IEnumerator CheckForLedge() {
        var time = Time.time;

        while (Time.time - time < 1f && trackedHand.GetHandButton(1)) {
            if (currentLedge || fullyClimbable) {
                closestCollider = FindClimbableCollider();
                grabPoint = closestCollider.ClosestPoint(transform.position);
        
                if (Vector3.Distance(grabPoint, transform.position) > grabRange.radius) yield break;
                if (closestCollider == currentLedge && !IsValidGrabPoint()) yield break;

                FixHandPosition();
            }
            yield return null;
        }
    }

    private Collider FindClimbableCollider() {
        if (currentLedge && fullyClimbable)
            return GetClosestCollider(transform.position, currentLedge, fullyClimbable);
        
        return currentLedge ? currentLedge : fullyClimbable;
    }

    private static Collider GetClosestCollider(Vector3 pos, Collider currentLedge, Collider fullyClimbable) {
        var ledgePoint = currentLedge.ClosestPoint(pos);
        var climbablePoint = fullyClimbable.ClosestPoint(pos);
        
        var ledgeDist = Vector3.Distance(pos, ledgePoint);
        var climbableDist = Vector3.Distance(pos, climbablePoint);

        return ledgeDist < climbableDist ? currentLedge : fullyClimbable;
    }
    
    private bool IsValidGrabPoint() {
        var bounds = currentLedge.bounds;
        var validHeight = bounds.center.y + bounds.extents.y - minHeightFromTop;

        return grabPoint.y >= validHeight;
    }

    public void OnGripBtnUp(Hand hand) {
        if (hand != trackedHand) return;
        
        canClimb = false;
        
        rb.constraints = RigidbodyConstraints.None;
        
        handPresence.OnEdge(false);
        handPresence.OnFlatSurface(false);
        onClimbEnd.Raise(this);
    }

    private void OnTriggerEnter(Collider other) {
        if (IsNotClimbableLayer(other.gameObject)) return;

        if (other.CompareTag(Tags.Climbable)) {
            fullyClimbable = other.GetComponent<Collider>();
            return;
        }
        
        currentLedge = other.GetComponent<Collider>();
    }

    private void OnTriggerExit(Collider other) {
        var col = other.GetComponent<Collider>();
        
        if (col == fullyClimbable) {
            fullyClimbable = null;
        }
        else if (col == currentLedge) {
            currentLedge = null;
        }
    }

    private static bool IsNotClimbableLayer(GameObject obj)
        => obj.CompareTag(Tags.NotClimbable) || obj.CompareTag(Tags.Player) || obj.layer == LayerMask.NameToLayer("UI");
}