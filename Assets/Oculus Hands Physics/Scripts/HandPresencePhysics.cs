using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class HandPresencePhysics : HandPresence
{
    [Space]
    [SerializeField] private float followSpeed = 30f;
    [SerializeField] private float rotateSpeed = 100f;
    [Space]
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;
    [Space]
    [SerializeField] private Transform palm;
    [SerializeField] private float reachDistance = 0.1f;
    [SerializeField] private float joinDistance = 0.05f;
    [SerializeField] private LayerMask grabbableLayer;

    private Rigidbody rb;
    private Transform target;
    
    private bool isGrabbing;
    private GameObject heldObject;
    private Transform grabPoint;
    private FixedJoint joint1, joint2;
    
    private void Awake() {
        target = controller.gameObject.transform;
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.mass = 20f;
        rb.maxAngularVelocity = 20f;
    }

    private void Start() {
        controller.selectAction.action.started += Grab;
        controller.selectAction.action.canceled += Release;

        rb.position = target.position;
        rb.rotation = target.rotation;
    }

    private void OnDestroy() {
        controller.selectAction.action.started -= Grab;
        controller.selectAction.action.canceled -= Release;
    }

    private void Update() {
        PhysicsMove();
    }

    public void printValue(InputActionReference reference) {
        print(reference.action.ReadValue<float>());
    }

    private void PhysicsMove() {
        // Position
        var positionWidthOffset = target.TransformPoint(positionOffset);
        var distance = Vector3.Distance(positionWidthOffset, transform.position);
        rb.velocity = (positionWidthOffset - transform.position).normalized * (followSpeed * distance);

        // Rotation
        var rotationWidthOffset = target.rotation * Quaternion.Euler(rotationOffset);
        var q = rotationWidthOffset * Quaternion.Inverse(rb.rotation);
        q.ToAngleAxis(out float angle, out Vector3 axis);
        rb.angularVelocity = axis * (angle * Mathf.Deg2Rad * rotateSpeed);
    }

    private void Grab(InputAction.CallbackContext ctx) {
        if (isGrabbing || heldObject) return;
        
        Collider[] grabbableColliders = Physics.OverlapSphere(palm.position, reachDistance, grabbableLayer);
        if (grabbableColliders.Length < 1) return;

        var objectToGrab = grabbableColliders[0].transform.gameObject;
        var objectBody = objectToGrab.GetComponent<Rigidbody>();

        if (objectBody != null) {
            heldObject = objectBody.gameObject;
        }
        else {
            objectBody = objectToGrab.GetComponentInParent<Rigidbody>();
            if (objectBody == null) return;

            heldObject = objectBody.gameObject;
        }

        StartCoroutine(GrabObject(grabbableColliders[0], objectBody));
    }

    private IEnumerator GrabObject(Collider collider, Rigidbody targetRb) {
        isGrabbing = true;

        // Create grab point
        grabPoint = new GameObject().transform;
        grabPoint.position = collider.ClosestPoint(palm.position);
        grabPoint.parent = heldObject.transform;

        // Move hand to grab point  
        target = grabPoint;

        // Wait for hand to reach grab point
        while (grabPoint != null && Vector3.Distance(grabPoint.position, palm.position) > joinDistance && isGrabbing) {
            yield return new WaitForEndOfFrame();
        }
        
        // Freeze hand and object motion
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        targetRb.velocity = Vector3.zero;
        targetRb.angularVelocity = Vector3.zero;

        targetRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        targetRb.interpolation = RigidbodyInterpolation.Interpolate;

        // Attach joints
        joint1 = gameObject.AddComponent<FixedJoint>();
        joint1.connectedBody = targetRb;
        joint1.breakForce = float.PositiveInfinity;
        joint1.breakTorque = float.PositiveInfinity;

        joint1.connectedMassScale = 1;
        joint1.massScale = 1;
        joint1.enableCollision = false;
        joint1.enablePreprocessing = false;

        joint2 = heldObject.AddComponent<FixedJoint>();
        joint2.connectedBody = rb;
        joint2.breakForce = float.PositiveInfinity;
        joint2.breakTorque = float.PositiveInfinity;

        joint2.connectedMassScale = 1;
        joint2.massScale = 1;
        joint2.enableCollision = false;
        joint2.enablePreprocessing = false;

        // Reset target
        target = controller.gameObject.transform;
    }

    private void Release(InputAction.CallbackContext ctx) {
        if (joint1 != null)
            Destroy(joint1);
        if (joint2 != null)
            Destroy(joint2);
        if (grabPoint != null)
            Destroy(grabPoint.gameObject);
        
        if (heldObject != null) {
            var targetRb = heldObject.GetComponent<Rigidbody>();
            targetRb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            targetRb.interpolation = RigidbodyInterpolation.None;
            heldObject = null;
        }

        isGrabbing = false;
        target = controller.gameObject.transform;
    }
}
