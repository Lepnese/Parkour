using UnityEngine;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController2 : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject leftHandObj;
    [SerializeField] private GameObject rightHandObj;
    [SerializeField] private GameObject centerEyeCamera;
    [SerializeField] private ActionBasedContinuousMoveProvider moveProvider;
    [SerializeField] private LayerMask playerLayers;

    [Header("Sprint")]
    [SerializeField] private float sprintSpeed = 200f;
    [SerializeField] private float maxSpeed = 16f;
    
    [Header("Jump")]
    [SerializeField] private float maxHandSpeed;
    [SerializeField] private float jumpVelocity = 5f;
    
    [Header("Drag")] 
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 2f;

    private int frameCounter;

    private float[] handVelocityArray;
    
    private Hand leftHand;
    private Hand rightHand;
    
    private Vector3 forwardDirection;

    private CapsuleCollider col;
    private Transform cameraTransform;
    private Rigidbody rb;
    private Collider lastCollider;

    public bool IsGrounded { get; private set; }
    public Rigidbody Rigidbody => rb;
    private float AverageHandSpeed => handVelocityArray.Average();
    
    private void Awake() {
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        leftHand = leftHandObj.GetComponent<Hand>();
        rightHand = rightHandObj.GetComponent<Hand>();
    }

    private void Start() {
        handVelocityArray = new float[30];
        cameraTransform = centerEyeCamera.transform;
    }

    private void Update() {
        frameCounter++;

        // IsGrounded = Physics.Raycast(
        //     cameraTransform.position,
        //     Vector3.down, col.height + 0.05f, ~playerLayers);

        IsGrounded = Mathf.Abs(rb.velocity.y) < 0.01f;
        
        forwardDirection = cameraTransform.TransformDirection(Vector3.forward).normalized;

        TrackHandSpeed();
        ControlDrag();
    }

    private void FixedUpdate() {
        if (!IsGrounded) return;
        if (!IsMovingForward()) return;
        Run();
        // ManageRun();
    }
    
    private void TrackHandSpeed() {
        if (leftHand.Velocity.magnitude > 0.7f && rightHand.Velocity.magnitude > 0.7f) {
            handVelocityArray[frameCounter % handVelocityArray.Length] = (leftHand.Velocity.magnitude + rightHand.Velocity.magnitude) / 2;
        }
        else {
            handVelocityArray[frameCounter % handVelocityArray.Length] = 0f;
        }
    } 

    private void ControlDrag() {
        rb.drag = IsGrounded ? groundDrag : airDrag;
    }

    private void Run() {
        var controlledSpeed = Mathf.Clamp(AverageHandSpeed, 0f, maxSpeed);
        rb.AddForce(forwardDirection * sprintSpeed * controlledSpeed);
    }

    private void Jump() {
        var handSpeed = (leftHand.Velocity.magnitude + rightHand.Velocity.magnitude) / 2f;
        var controlledSpeed = Mathf.Clamp(handSpeed, 0f, maxHandSpeed);
        var vel = rb.velocity.magnitude > 2f ? 6f : 4.5f;
        
        rb.AddForce(vel * controlledSpeed * Vector3.up, ForceMode.Impulse);
    }
    
    private bool IsMovingForward() => moveProvider.leftHandMoveAction.action.ReadValue<Vector2>().y > 0f;
    
    public void OnJumpBtnUp() {
        if (IsGrounded)
            Jump();
    }

    public void OnPlayerFall() {
        var playerPos = transform.position;
        
        var closestPoint = lastCollider.ClosestPoint(playerPos);
        var dir = (closestPoint - playerPos).normalized;

        closestPoint.x += dir.x;
        closestPoint.y = lastCollider.bounds.max.y + 0.2f;
        closestPoint.z += dir.z;
        
        // var spawnPoint = closestPoint;
        // spawnPoint.y += lastCollider.bounds.extents.y + 0.2f;

        rb.velocity = Vector3.zero;
        transform.position = closestPoint;
    }

    private void OnCollisionStay(Collision collision) {
        // var col = collision.collider;

        if (Mathf.Abs(rb.velocity.y) < 0.01f)
            lastCollider = collision.collider;
    }
}
