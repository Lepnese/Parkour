using TMPro;
using UnityEngine;

public class PhysicsHands2 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [Header("Grappling")]
    [SerializeField] private Transform grappleSpawnPoint;
    [Header("Climbing")]
    [SerializeField] private PhysicsHandsEvent onClimbStart;
    [SerializeField] private PhysicsHandsEvent onClimbEnd;
    [Header("PID Movement Values")]
    [SerializeField] private float frequency = 50f;
    [SerializeField] private float damping = 1f;
    [SerializeField] private float rotFrequency = 100f;
    [SerializeField] private float rotDamping = 0.9f;
    [Header("Rigidbody Movement Values")]
    [SerializeField] private Hand trackedHand;
    [SerializeField] private float physicsRange = 0.15f;
    [SerializeField] private float climbForce = 1000f;
    [SerializeField] private float climbDrag = 500f;
    [SerializeField] private LayerMask collisionLayers;
    [Header("Player References")]
    [SerializeField] private Rigidbody playerRb;

    private Rigidbody rb;
    private Transform targetTransform;
    private Vector3 previousPosition;
    private bool isHoldingGripBtn;
    private bool canClimb;
    private BoxCollider closestLedge;

    private HandPresence handPresence;

    public Transform GrappleSpawnPoint => grappleSpawnPoint;
    public Hand TrackedHand => trackedHand;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        handPresence = GetComponent<HandPresence>();
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
        
        if (isHoldingGripBtn && canClimb) {
            Climb();
            return;
        }
        
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
        var grabPoint = closestLedge.ClosestPoint(transform.position);
        transform.position = grabPoint;
        
        if (IsGrabbingCorner(grabPoint))
            handPresence.OnEdge(true);
        else
            handPresence.OnFlatSurface(true);
        
        rb.constraints = RigidbodyConstraints.FreezeAll;
        canClimb = true;
    }

    // CETTE SECTION VIENT D'UNE SOURCE EXTÉRIEURE
    private bool IsGrabbingCorner(Vector3 grabPoint) {
        Vector3 localPoint = closestLedge.transform.InverseTransformPoint(grabPoint);
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
        
        isHoldingGripBtn = true;
        
        if (!closestLedge) return;
        
        FixHandPosition();
        onClimbStart.Raise(this);
    }
    
    public void OnGripBtnUp(Hand hand) {
        if (hand != trackedHand) return;

        isHoldingGripBtn = false;
        canClimb = false;
        rb.constraints = RigidbodyConstraints.None;
        
        handPresence.OnEdge(false);
        handPresence.OnFlatSurface(false);
        onClimbEnd.Raise(this);
    }

    private void OnTriggerEnter(Collider other) {
        if (!(other.CompareTag(Tags.Climbable) || other.CompareTag(Tags.ClimbableSurface)))
            return;
        
        closestLedge = other.GetComponent<BoxCollider>();
        
        if (!closestLedge)
            Debug.LogWarning($"GameObject {other} does not contain a collider but is tagged: {other.tag}");
    }

    private void OnTriggerExit(Collider other) {
        if (!(other.CompareTag(Tags.Climbable) || other.CompareTag(Tags.ClimbableSurface))) 
            return;
        
        closestLedge = null;
    }
}