using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PhysicsHands2 : MonoBehaviour
{
    [SerializeField] private LayerMask defaultLayer;
    [SerializeField] private LayerMask playerLayers;
    [SerializeField] private ControllerSide side;
    [Header("Grappling")]
    [SerializeField] private Transform grappleSpawnPoint;
    [Header("Climbing")] 
    [SerializeField] private float ledgeGrabTimeDelay = 0.5f;
    [SerializeField] private float minHeightFromTop = 0.3f;
    [SerializeField] private Vector3 ledgeGrabRotation = new Vector3(282.059784f, 0, 192.092636f);
    [SerializeField] private Transform indexFingerTip;
    [SerializeField] private PhysicsHandsEvent onClimbStart;
    [SerializeField] private PhysicsHandsEvent onClimbEnd;
    [Header("PID Movement Values")]
    [SerializeField] private float frequency = 50f;
    [SerializeField] private float damping = 1f;
    [SerializeField] private float rotFrequency = 100f;
    [SerializeField] private float rotDamping = 0.9f;
    [Header("Rigidbody Movement Values")]
    [SerializeField] private float maxDistanceFromTrackedHand = 2f;
    [SerializeField] private float physicsRange = 0.15f;
    [SerializeField] private float climbForce = 1000f;
    [SerializeField] private float climbDrag = 500f;
    [SerializeField] private LayerMask collisionLayers;
    [Header("Player References")]
    [SerializeField] private Rigidbody playerRb;
    [Header("Shooting")] 
    [SerializeField] private PhysicsHandsEvent onGripBtnDown;
    [SerializeField] private PhysicsHandsEvent onGripBtnUp;
    [SerializeField] private GameObject handModel;
    [SerializeField] private GameObject gun;

    private Rigidbody rb;
    private Transform targetTransform;
    private Vector3 previousPosition;
    private Collider currentLedge;
    private SphereCollider grabRange;
    private Vector3 grabPoint;
    private List<Collider> closeColliders;
    private HandPresence handPresence;
    
    private XRDirectInteractor directInteractor;
    private GameObject grabbedObject;
    private bool canActivateHands;
    
    public bool CanClimb { get; private set; }
    public Transform GrappleSpawnPoint => grappleSpawnPoint;
    public Hand TrackedHand { get; private set; }

    public GameObject AttachedGun => gun;
    public PhysHandState State { get; private set; }

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        handPresence = GetComponent<HandPresence>();
        grabRange = GetComponent<SphereCollider>();
    }

    private void Start() {
        SetTrackedHand();
        
        rb.maxAngularVelocity = float.PositiveInfinity;
        previousPosition = transform.position;
        State = PhysHandState.Hand;
        closeColliders = new List<Collider>();
    }

    public void SetTrackedHand() {
        TrackedHand = HandInteractionManager.Instance.GetTrackedHand(side);
        
        targetTransform = TrackedHand.transform;
        directInteractor = targetTransform.GetComponent<XRDirectInteractor>();
        
        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }

    private void Update() {
        currentLedge = FindClosestLedge();
        var pos = transform.position;
        
        
        if (TrackedHand.GetHandButton(1) && CanClimb) {
            Climb();
            return;
        }

        // Teleport physics hands to tracked hand if too far 
        if (Vector3.Distance(transform.position, TrackedHand.transform.position) > maxDistanceFromTrackedHand)
            transform.position = TrackedHand.transform.position;

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
        Quaternion rot;
        //
        // if (IsGrabbingCorner(grabPoint)) {
        //     grabPoint = GetClosestPointOnEdge(grabPoint);
        //     rot = Quaternion.Euler(ledgeGrabRotation);
        //     handPresence.OnEdge(true);
        // } 
        // else {
            rot = Quaternion.Euler(0, transform.eulerAngles.y, -90);
            handPresence.OnFlatSurface(true);
        // }
        
        transform.position = grabPoint;
        transform.rotation = rot;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        CanClimb = true;
        onClimbStart.Raise(this);
    }

    private Vector3 GetClosestPointOnEdge(Vector3 point) {
        var curBounds = currentLedge.bounds;
        var gapToIndexFingerTip = indexFingerTip.position - transform.position;
        var yMax = curBounds.center.y + curBounds.extents.y - gapToIndexFingerTip.y;
        
        // Vector 3 -- Point on edge
        // float -- Y rotation associated with side of ledge
        var edgePoints = new Dictionary<Vector3, float>
        {
            [new Vector3(curBounds.center.x + curBounds.extents.x, yMax, point.z)] = 0f,
            [new Vector3(curBounds.center.x - curBounds.extents.x, yMax, point.z)] = 180f,
            [new Vector3(point.x, yMax, curBounds.center.z + curBounds.extents.z)] = -90f,
            [new Vector3(point.x, yMax, curBounds.center.z - curBounds.extents.z)] = 90f
        };
        
        var minDistance = float.PositiveInfinity;
        var closestPoint = Vector3.zero;

        foreach (var pair in edgePoints) {
            var edgePoint = pair.Key;
            var yRotation = pair.Value;
            
            var dist = Vector3.Distance(point, edgePoint);
            
            if (dist > minDistance) continue;
            
            minDistance = dist;
            closestPoint = edgePoint;
            ledgeGrabRotation.y = yRotation;
        }
        return closestPoint;
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
        Vector3 displacement = (transform.position - targetTransform.position);
        Vector3 force = displacement * climbForce;
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

    private IEnumerator CheckForLedge() {
        var time = Time.time;

        while (Time.time - time < ledgeGrabTimeDelay && TrackedHand.GetHandButton(1)) {
            // while (!currentLedge) yield return null;

            // le joueur ne doit pas grimper sur un objet qu'il peut attraper
            

            // grabPoint = GetClosestPoint();


            var pos = transform.position + transform.forward * 0.5f;

            if (Physics.Raycast(transform.position, Vector3.down, out var hit, 0.5f, defaultLayer)) {
                
                if (hit.collider.GetComponent<XRGrabInteractable>() != null)
                    yield break;
                
                if (hit.collider.CompareTag("NotClimbable"))
                    yield break;
                
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Default")) {
                    grabPoint = hit.point;
                    FixHandPosition();
                    yield break;
                }
            }
            
            if (Physics.Raycast(pos, Vector3.down, out var hit2, 0.8f, defaultLayer)) {
                
                if (hit.collider.GetComponent<XRGrabInteractable>() != null)
                    yield break;
                
                if (hit.collider.CompareTag("NotClimbable"))
                    yield break;
                
                if (hit2.collider.gameObject.layer == LayerMask.NameToLayer("Default")) {
                    grabPoint = hit2.point;
                    FixHandPosition();
                    yield break;
                }
            }
            
            // if (Vector3.Distance(grabPoint, transform.position) > grabRange.radius) yield break;
            // if (!currentLedge.CompareTag(Tags.Climbable) && !IsValidGrabPoint(grabPoint, currentLedge)) yield break;
            //
            // if (TrackedHand.GetHandButton(1) && IsValidGrabPoint(grabPoint, currentLedge))
            //     FixHandPosition();
                
            yield return null;
        }
    }

    private bool IsValidGrabPoint(Vector3 point, Collider ledge) {
        var dir = (point - transform.position).normalized;
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, 5f, ~playerLayers)) {
            return hit.normal.y > 0f;
        }
        
        return grabPoint.y >= ledge.bounds.max.y - minHeightFromTop;
    }

    public void OnGripBtnDown(Hand hand) {
        if (hand != TrackedHand) return;
        onGripBtnDown.Raise(this);

        if (!IsState(PhysHandState.Hand)) return;
        if (directInteractor.interactablesHovered.Count == 0)
            StartCoroutine(CheckForLedge());
    }
    
    public void OnGripBtnUp(Hand hand) {
        if (hand != TrackedHand) return;
        onGripBtnUp.Raise(this);

        if (!IsState(PhysHandState.Hand)) return;

        CanClimb = false;
        rb.constraints = RigidbodyConstraints.None;

        handPresence.OnEdge(false);
        handPresence.OnFlatSurface(false);
        onClimbEnd.Raise(this);
    }

    private void OnTriggerEnter(Collider other) {
        if (IsNotClimbableLayer(other.gameObject)) return;
        
        closeColliders.AddRange(other.GetComponents<Collider>());
    }

    private Collider FindClosestLedge() {
        if (closeColliders.Count == 0) return null;
        
        var handPos = transform.position;
        return closeColliders.OrderBy(x => Vector3.Distance(handPos, x.ClosestPoint(handPos))).First();
    }
    
    public void SetState(PhysHandState newState) {
        State = newState;
    }

    private void OnTriggerExit(Collider other) {
        var handPos = transform.position;
        closeColliders.RemoveAll(x => Vector3.Distance(handPos, x.ClosestPoint(handPos)) > grabRange.radius);
    }

    private static bool IsNotClimbableLayer(GameObject obj)
        => obj.CompareTag(Tags.NotClimbable) || obj.CompareTag(Tags.Player) || obj.layer == LayerMask.NameToLayer("UI");

    private bool IsState(PhysHandState checkState) => checkState == State;

    #region Toggle Hand Model On/Off
    public void OnSelectEntered() { 
        handModel.SetActive(false);
        canActivateHands = false;
        grabbedObject = directInteractor.firstInteractableSelected.transform.gameObject;
        grabbedObject.layer = LayerMask.NameToLayer("Grabbing");
    }

    public void OnSelectExited() {
        StartCoroutine(ActivateHandModel());
    }
    
    private IEnumerator ActivateHandModel() {
        while (!canActivateHands) {
            var colliders = Physics.OverlapSphere(transform.position, grabRange.radius);
            canActivateHands = colliders.All(c => c.gameObject != grabbedObject);

            yield return null;
        }
        
        handModel.SetActive(true);
        grabbedObject.layer = LayerMask.NameToLayer("Default");
    }
    
    public void ToggleHandModel(bool active) {
        handModel.SetActive(active);
    }

    private IEnumerator ToggleHandModelRoutine(bool active, float time) {
        yield return new WaitForSeconds(time);
        handModel.SetActive(active);
    }
    #endregion
}

public enum PhysHandState
{
    Hand,
    Gun
}