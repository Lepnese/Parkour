using Unity.XR.CoreUtils;
using UnityEngine;

public class PhysicsHands2 : MonoBehaviour
{
    [Space]
    [SerializeField] private float frequency = 50f;
    [SerializeField] private float damping = 1f;
    [SerializeField] private float rotFrequency = 100f;
    [SerializeField] private float rotDamping = 0.9f;
    [Space]
    [SerializeField] private Hand targetController;
    [SerializeField] private float physicsRange = 0.1f;
    [SerializeField] private float climbForce = 1000f;
    [SerializeField] private float climbDrag = 500f;
    [SerializeField] private LayerMask interactableLayers;
    [Space]
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private CapsuleCollider playerCollider;
    
    private Rigidbody rb;
    private Transform targetTransform;
    private Vector3 position;
    private Vector3 previousPosition;
    private bool isCloseToObject;
    private bool isClimbing;

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

    private void Update() {
        isCloseToObject = Physics.CheckSphere(transform.position, physicsRange, interactableLayers);
        if (isCloseToObject) {
            PIDMovement();
            PIDRotation();
        }
        else {
            TransformMovement();
            TransformRotation();
        }

        playerCollider.enabled = !isClimbing;
        if (!isClimbing) return;
        Climb();
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

    private void OnCollisionEnter(Collision collision) {
        isClimbing = true;
    }

    private void OnCollisionExit(Collision other) {
        isClimbing = false;
    }
}