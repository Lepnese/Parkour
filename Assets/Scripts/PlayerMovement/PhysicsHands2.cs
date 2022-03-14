using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PhysicsHands2 : MonoBehaviour
{
    [SerializeField] private ClimbingHandler player;

    [Header("Climbing")] 
    [SerializeField] private float ledgeGrabMaxDistance = 1f;
    [SerializeField] private LayerMask climbLayer;
    [SerializeField] private PhysicsHandsEvent onClimbStart;
    [SerializeField] private PhysicsHandsEvent onClimbEnd;
    [Header("PID Movement Values")]
    [SerializeField] private float frequency = 50f;
    [SerializeField] private float damping = 1f;
    [SerializeField] private float rotFrequency = 100f;
    [SerializeField] private float rotDamping = 0.9f;
    [Header("Rigidbody Movement Values")]
    public Hand targetController;
    [SerializeField] private float physicsRange = 0.15f;
    [SerializeField] private float ledgeGrabRange = 0.35f;
    [SerializeField] private float climbForce = 1000f;
    [SerializeField] private float climbDrag = 500f;
    [SerializeField] private LayerMask interactableLayers;
    [Header("Player References")]
    [SerializeField] private Rigidbody playerRb;

    private Rigidbody rb;
    private Transform targetTransform;
    private Vector3 previousPosition;
    private bool isHoldingGripBtn;
    private bool posIsFixed;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        targetTransform = targetController.transform;
        rb.maxAngularVelocity = float.PositiveInfinity;
        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
        previousPosition = transform.position;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(transform.position, ledgeGrabRange);
    }

    private void Update() {
        var pos = transform.position;
        
        if (isHoldingGripBtn && posIsFixed) {
            Climb();
            return;
        }
        
        bool isCloseToObject = Physics.CheckSphere(pos, physicsRange, interactableLayers);
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
        if (player.CollisionList.Count == 0) return;
        
        BoxCollider closestLedge = player.CollisionList.Count == 1 ? 
            player.CollisionList[0] :
            player.CollisionList.OrderBy(col => (transform.position - col.transform.position).sqrMagnitude).First();
        
        if (!closestLedge) return;
        ToggleHandColliders();
        
        var closestPoint = closestLedge.ClosestPoint(transform.position);
        transform.position = closestPoint;

        var localPoint = closestLedge.transform.InverseTransformPoint(closestPoint);
        var localDir = localPoint.normalized;

        float upDot = Vector3.Dot(localDir, Vector3.up);
        float fwdDot = Vector3.Dot(localDir, Vector3.forward);
        float rightDot = Vector3.Dot(localDir, Vector3.right);
        
        float upPower = Mathf.Abs(upDot);
        float fwdPower = Mathf.Abs(fwdDot);
        float rightPower = Mathf.Abs(rightDot);
        
        print($"{upPower}, {fwdPower}, {rightPower}");
        
        rb.constraints = RigidbodyConstraints.FreezeAll;
        posIsFixed = true;
    }
    
    private void Climb() {
        Vector3 displacementFromResting = transform.position - targetTransform.position;
        Vector3 force = displacementFromResting * climbForce;
        float drag = GetDrag();
        
        playerRb.AddForce(force, ForceMode.Acceleration);
        playerRb.AddForce(drag * -playerRb.velocity * climbDrag, ForceMode.Acceleration);
    }

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
        if (hand != targetController) return;
        
        isHoldingGripBtn = true;
        
        bool isNearLedge = Physics.CheckSphere(transform.position, ledgeGrabMaxDistance, (int)Layers.ClimbPoint);
        if (!isNearLedge) return;
        
        FixHandPosition();
        onClimbStart.Raise(this);
    }
    
    public void OnGripBtnUp(Hand hand) {
        if (hand != targetController) return;
        onClimbEnd.Raise(this);
        
        isHoldingGripBtn = false;
        posIsFixed = false;
        rb.constraints = RigidbodyConstraints.None;
        
        ToggleHandColliders();
    }

    private void ToggleHandColliders() {
        
    }
}