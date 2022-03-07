using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController2 : MonoBehaviour
{
    [Header("Player Objects")]
    [SerializeField] private GameObject leftHandObj;
    [SerializeField] private GameObject rightHandObj;
    [SerializeField] private GameObject centerEyeCamera;
    [SerializeField] private float a = 0.25f;
    [SerializeField] private ActionBasedContinuousMoveProvider move;

    [Header("Movement")]
    [SerializeField] private float sprintSpeed = 2f;
    [SerializeField] private float sprintMultiplier = 10f;
    [SerializeField] private float airMultiplier = 0.4f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Drag")] 
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 2f;

    private int frameCounter;
    private bool lastFrameHandsAboveJumpLimit = true;

    private float[] velocityArray;
    private Vector3[] speedArray;
    
    private Hand leftController;
    private Hand rightController;
    
    private Vector3 playerVelocity;
    private Vector3 lastFramePosition;
    private Vector3 forwardDirection;
    private Vector3 averagePlayerSpeed;

    private CapsuleCollider col;
    private Transform cameraTransform;
    private Rigidbody rb;
    
    public bool IsGrounded => Physics.Raycast(
        new Vector2(transform.position.x, transform.position.y + col.height),
        Vector3.down, col.height + 0.1f, ~gameObject.layer);
    
    private float AverageHandSpeed {
        get => velocityArray.Average();
    }

    private bool HandsAboveJumpLimit {
        get => leftHandObj.transform.position.y >  cameraTransform.position.y - a && rightHandObj.transform.position.y >  cameraTransform.position.y - a;
    }

    private void Awake() {
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        leftController = leftHandObj.GetComponent<Hand>();
        rightController = rightHandObj.GetComponent<Hand>();
    }

    private void Start() {
        velocityArray = new float[60];
        speedArray = new Vector3[30];
        cameraTransform = centerEyeCamera.transform;
        lastFramePosition = transform.position;
    }

    private void Update() {
        if (Time.timeSinceLevelLoad < 1f) return;
        frameCounter++;
        
        averagePlayerSpeed = GetAveragePlayerSpeed();
        forwardDirection = cameraTransform.TransformDirection(Vector3.forward);
        
        TrackHandSpeed();
        TrackPlayerSpeed();
        
        ControlDrag();
        
        if (IsGrounded && JumpAction())
            Jump();
    }

    private void FixedUpdate() {
        if (!IsJoystickMoving()) return;
        ManageRun();
    }

    private void LateUpdate() {
        lastFramePosition = transform.position;
        lastFrameHandsAboveJumpLimit = !HandsAboveJumpLimit;
    }

    private void TrackPlayerSpeed() => speedArray[frameCounter % speedArray.Length] = (transform.position - lastFramePosition) / Time.deltaTime;
    private void TrackHandSpeed() => velocityArray[frameCounter % velocityArray.Length] = (leftController.Velocity.magnitude + rightController.Velocity.magnitude) / 2;

    private void ControlDrag() {
        rb.drag = IsGrounded ? groundDrag : airDrag;
    }
    
    private void ManageRun() {
        if (IsGrounded) {
            rb.AddForce(forwardDirection.normalized * sprintSpeed * sprintMultiplier * AverageHandSpeed, ForceMode.Acceleration);
        }
        else {
            rb.AddForce(forwardDirection.normalized * sprintSpeed * sprintMultiplier * airMultiplier * AverageHandSpeed, ForceMode.Acceleration);
        }
    }

    private void Jump() {
        rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
    }

    // Returns the average velocity (from SpeedArray) of the player
    private Vector3 GetAveragePlayerSpeed() => speedArray.Aggregate(Vector3.zero, (acc, v) => acc + v) / speedArray.Length;

    // Checks if player has a speed greater than 1 (joystick movement);
    private bool IsJoystickMoving() => move.leftHandMoveAction.action.ReadValue<Vector2>() != Vector2.zero;

    // Checks if player does the jump action (raise both hands above head with speed)
    private bool JumpAction() => HandsAboveJumpLimit && lastFrameHandsAboveJumpLimit && leftController.Velocity.magnitude > 1f && rightController.Velocity.magnitude > 1f;
}