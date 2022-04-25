﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PhysicsHands2 : MonoBehaviour
{
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

    private Camera cam;
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
    public Hand TrackedHand { get; private set; }

    public GameObject AttachedGun => gun;
    public PhysHandState State { get; private set; }

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        handPresence = GetComponent<HandPresence>();
        grabRange = GetComponent<SphereCollider>();
    }

    private void Start() {
        cam = Camera.main;
        SetTrackedHand();
        
        rb.maxAngularVelocity = float.PositiveInfinity;
        previousPosition = transform.position;
        State = PhysHandState.Hand;
    }

    public void SetTrackedHand() {
        TrackedHand = HandInteractionManager.Instance.GetTrackedHand(side);
        
        targetTransform = TrackedHand.transform;
        
        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }

    private void Update() {
        var pos = transform.position;
        
        if (TrackedHand.GetHandButton(1) && canClimb) {
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
        
        if (IsGrabbingCorner(grabPoint)) {
            grabPoint = GetClosestPointOnEdge(grabPoint);
            rot = Quaternion.Euler(ledgeGrabRotation);
            handPresence.OnEdge(true);
        } 
        else {
            rot = Quaternion.Euler(0, transform.eulerAngles.y, -90);
            handPresence.OnFlatSurface(true);
        }
        
        transform.position = grabPoint;
        transform.rotation = rot;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        canClimb = true;
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

    private IEnumerator CheckForLedge() {
        var time = Time.time;

        while (Time.time - time < ledgeGrabTimeDelay && TrackedHand.GetHandButton(1)) {
            if (currentLedge || fullyClimbable) {
                closestCollider = FindClimbableCollider();
                
                if (closestCollider.GetComponent<XRGrabInteractable>() != null)
                    yield break;

                grabPoint = GetClosestPoint();

                if (Vector3.Distance(grabPoint, transform.position) > grabRange.radius) yield break;
                if (closestCollider == currentLedge && !IsValidGrabPoint()) yield break;

                FixHandPosition();
            }
            yield return null;
        }
    }

    private Vector3 GetClosestPoint() {
        Vector3 point;
        var meshCollider = closestCollider.GetComponent<MeshCollider>();
        
        if (!meshCollider) {
            point = closestCollider.ClosestPoint(transform.position);
            return point;
        }
        
        meshCollider.convex = true;
        point = closestCollider.ClosestPoint(transform.position);
        meshCollider.convex = false;
    
        return point;
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

    public void OnGripBtnDown(Hand hand) {
        if (hand != TrackedHand) return;
        onGripBtnDown.Raise(this);

        if (!IsState(PhysHandState.Hand)) return;
        StartCoroutine(CheckForLedge());
    }
    
    public void OnGripBtnUp(Hand hand) {
        if (hand != TrackedHand) return;
        onGripBtnUp.Raise(this);

        if (!IsState(PhysHandState.Hand)) return;

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

    public void SetState(PhysHandState newState) {
        State = newState;
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

    private bool IsState(PhysHandState checkState) => checkState == State;

    #region Toggle Hand Model On/Off
    public void OnSelectEntered(float time) {
        ToggleHandModel(false, time);
    }

    public void OnSelectExited(float time) {
        ToggleHandModel(true, time);
    }
    
    public void ToggleHandModel(bool active, float time) {
        if (time == 0) {
            handModel.SetActive(active);
            return;
        }
        
        StartCoroutine(ToggleHandModelRoutine(active, time));
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