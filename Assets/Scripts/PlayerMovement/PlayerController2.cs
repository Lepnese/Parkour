using System;
using UnityEngine;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController2 : MonoBehaviour
{
    [Header("Interactable Events")]
    [SerializeField] private BoolEvent onAreaEnter;
    
    [Header("Player")]
    [SerializeField] private GameObject leftHandObj;
    [SerializeField] private GameObject rightHandObj;
    [SerializeField] private GameObject centerEyeCamera;
    [SerializeField] private ActionBasedContinuousMoveProvider moveProvider;
    [SerializeField] private LayerMask playerLayers;

    [Header("Sprint")]
    [SerializeField] private float sprintSpeed = 200f;
    [SerializeField] private float maxSpeed = 16f;
    [SerializeField] private float sprintMultiplier = 10f;
    [SerializeField] private float airMultiplier = 0.4f;

    [Header("Jump")]
    [SerializeField] private float maxHandSpeed;
    [SerializeField] private float jumpVelocity = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    
    [Header("Drag")] 
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 2f;

    private int frameCounter;
    private bool isGrounded;
    private bool isJumpBtnDown;

    private float[] handVelocityArray;
    
    private Hand leftHand;
    private Hand rightHand;
    
    private Vector3 forwardDirection;

    private CapsuleCollider col;
    private Transform cameraTransform;
    private Rigidbody rb;

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

        isGrounded = Physics.Raycast(
            cameraTransform.position,
            Vector3.down, col.height + 0.05f, ~playerLayers);
        
        Debug.DrawRay(cameraTransform.position,
                    Vector3.down * (col.height + 0.05f));
        
        forwardDirection = cameraTransform.TransformDirection(Vector3.forward).normalized;
        
        TrackHandSpeed();
        ControlDrag();
    }

    private void FixedUpdate() {
        if (!isGrounded) return;
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
        rb.drag = isGrounded ? groundDrag : airDrag;
    }

    private void Run() {
        var controlledSpeed = Mathf.Clamp(AverageHandSpeed, 0f, maxSpeed);
        rb.AddForce(forwardDirection * sprintSpeed * controlledSpeed);
    }

    private void Jump() {
        var handSpeed = (leftHand.Velocity.magnitude + rightHand.Velocity.magnitude) / 2f;
        var controlledSpeed = Mathf.Clamp(handSpeed, 0f, maxHandSpeed);

        rb.AddForce(jumpVelocity * Vector3.up * controlledSpeed, ForceMode.Impulse);
    }
    
    private bool IsMovingForward() => moveProvider.leftHandMoveAction.action.ReadValue<Vector2>().y > 0f;

    public void OnJumpBtnUp() {
        if (isGrounded)
            Jump();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Tags.InteractableArea))
            onAreaEnter.Raise(true);
    }
    
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(Tags.InteractableArea))
            onAreaEnter.Raise(false);
    }
}
