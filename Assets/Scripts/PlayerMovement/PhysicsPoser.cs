using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(XRController))]
public class PhysicsPoser : MonoBehaviour
{
    [Space]
    [SerializeField] private float physicsRange = 0.1f;
    [SerializeField] private LayerMask physicsMask = 0;
    
    [Space]
    [Range(0, 1)] [SerializeField] private float slowDownVelocity = 0.75f;
    [Range(0, 1)] [SerializeField] private float slowDownAngularVelocity = 0.75f;

    [Space]
    [Range(0, 100)] [SerializeField] private float maxPositionChange = 75f;
    [Range(0, 100)] [SerializeField] private float maxRotationChange = 75f;

    private Rigidbody rb = null;
    private ActionBasedController controller = null;
    private XRBaseInteractor interactor = null;

    private Vector3 targetPosition = Vector3.zero;
    private Quaternion targetRotation = Quaternion.identity;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<ActionBasedController>();
        interactor = GetComponent<XRBaseInteractor>();
    }

    private void Start() {
        UpdateTracking();
        MoveUsingTransform();
        RotateUsingTransform();
    }

    private void Update() {
        UpdateTracking();
    }

    private void UpdateTracking() {
        targetPosition = controller.positionAction.action.ReadValue<Vector3>();
        targetRotation = controller.rotationAction.action.ReadValue<Quaternion>();
        // device.TryGetFeatureValue(CommonUsages.devicePosition, out targetPosition);
        // device.TryGetFeatureValue(CommonUsages.deviceRotation, out targetRotation);
    }

    private void FixedUpdate() {
        if (IsHoldingObject() || !WithinPhysicsRange()) {
            MoveUsingTransform();
            RotateUsingTransform();
        }
        else {
            MoveUsingPhysics();
            RotateUsingPhysics();
        }
    }

    private bool IsHoldingObject() => true;
    private bool WithinPhysicsRange() => Physics.CheckSphere(transform.position, physicsRange, physicsMask, QueryTriggerInteraction.Ignore);

    private void MoveUsingPhysics() {
        rb.velocity *= slowDownVelocity;

        Vector3 velocity = FindNewVelocity();
        if (IsValidVelocity(velocity.x)) {
            float maxChange = maxPositionChange * Time.deltaTime;
            rb.velocity = Vector3.MoveTowards(rb.velocity, velocity, maxChange);
        }
    }

    private Vector3 FindNewVelocity() => (targetPosition - rb.position) / Time.deltaTime;

    private void RotateUsingPhysics() {
        rb.angularVelocity *= slowDownAngularVelocity;

        Vector3 angularVelocity = FindNewAngularVelocity();
        if (IsValidVelocity(angularVelocity.x)) {
            float maxChange = maxRotationChange * Time.deltaTime;
            rb.angularVelocity = Vector3.MoveTowards(rb.angularVelocity, angularVelocity, maxChange);
        }
    }

    private Vector3 FindNewAngularVelocity() {
        Quaternion difference = targetRotation * Quaternion.Inverse(rb.rotation);
        difference.ToAngleAxis(out float angleInDeg, out Vector3 rotationAxis);
        
        if (angleInDeg > 180)
            angleInDeg -= 360;
        
        return (rotationAxis * angleInDeg * Mathf.Deg2Rad) / Time.deltaTime;
    }

    private bool IsValidVelocity(float value) => !float.IsNaN(value) && !float.IsInfinity(value);

    private void MoveUsingTransform() {
        rb.velocity = Vector3.zero;
        transform.localPosition = targetPosition;
    }

    private void RotateUsingTransform() {
        rb.angularVelocity = Vector3.zero;
        transform.localRotation = targetRotation;
    }

    private void OnDrawGizmos() => Gizmos.DrawWireSphere(transform.position, physicsRange);

    private void OnValidate() {
        if (TryGetComponent(out Rigidbody rigidbody))
            rigidbody.useGravity = false;
    }
}
