using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private GameObject groundChecker;
    [SerializeField] private GameObject leftHandObj;
    [SerializeField] private GameObject rightHandObj;
    [SerializeField] private GameObject centerEyeCamera;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float a = 0.25f;
    [SerializeField] private float climbForce = 200f;

    [HideInInspector]
    public CharacterController controller;

    private Hand ClimbingHand { get; set; }
    
    private int frameCounter;
    private float playerMass = 3f;
    private const float GravityValue = 0f;
    private bool lastFrameHandsAboveJumpLimit = true;
    private bool isClimbing;

    private float[] velocityArray;
    private Vector3[] speedArray;
    
    private Hand leftController;
    private Hand rightController;
    
    private Vector3 playerVelocity;
    private Vector3 lastFramePosition;
    private Vector3 forwardDirection;
    private Vector3 averagePlayerSpeed;
    private Vector3 climbDirection = Vector3.zero;

    private Rigidbody rb;
    private Transform cameraTransform;

    private float AverageHandSpeed {
        get => velocityArray.Average();
    }

    private bool HandsAboveJumpLimit {
        get => leftHandObj.transform.position.y >  cameraTransform.position.y - a && rightHandObj.transform.position.y >  cameraTransform.position.y - a;
    }

    private bool IsGrounded {
        get => Physics.Raycast(groundChecker.transform.position, transform.TransformDirection(Vector3.down), 0.05f, ground);
    }

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        leftController = leftHandObj.GetComponent<Hand>();
        rightController = rightHandObj.GetComponent<Hand>();
        controller = GetComponent<CharacterController>();
    }

    private void Start() {
        velocityArray = new float[60];
        speedArray = new Vector3[30];
        cameraTransform = centerEyeCamera.transform;
        lastFramePosition = transform.position;
    }

    private void Update() {
        // if (Time.timeSinceLevelLoad < 1f) return;
        frameCounter++;

        averagePlayerSpeed = GetAveragePlayerSpeed();
        forwardDirection = cameraTransform.TransformDirection(Vector3.forward);
        
        TrackHandSpeed();
            
        if (IsGrounded && playerVelocity.y < 0.1f) {
            playerVelocity.y = 0f;
        }

        TrackPlayerSpeed();
        if (IsGrounded) {
            ManageRun();
            ManageJump();
        }

        // Applying gravity to player if not climbing
        if (!isClimbing) {
            playerVelocity.y -= GravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }

        playerVelocity.x = averagePlayerSpeed.x;
        playerVelocity.z = averagePlayerSpeed.z;
    }

    private void LateUpdate() {
        lastFramePosition = transform.position;
        lastFrameHandsAboveJumpLimit = !HandsAboveJumpLimit;
    }

    private void TrackPlayerSpeed() => speedArray[frameCounter % speedArray.Length] = (transform.position - lastFramePosition) / Time.deltaTime;

    private void TrackHandSpeed() => velocityArray[frameCounter % velocityArray.Length] = (leftController.Velocity.magnitude + rightController.Velocity.magnitude) / 2;

    private void ManageRun() {
        if (IsJoystickMoving()) {
            controller.Move(forwardDirection * AverageHandSpeed * speed * Time.deltaTime);
        }
    }

    private void ManageJump() {
        if (!CanJump()) return;
        playerVelocity.x = averagePlayerSpeed.x;
        playerVelocity.z = averagePlayerSpeed.z;
        playerVelocity.y += Mathf.Sqrt(jumpHeight * 2f * GravityValue);
    }

    // Returns the average velocity (from SpeedArray) of the player
    private Vector3 GetAveragePlayerSpeed() => speedArray.Aggregate(Vector3.zero, (acc, v) => acc + v) / speedArray.Length;

    // Checks if player has a speed greater than 1 (joystick movement);
    private bool IsJoystickMoving() => controller.velocity.magnitude > 1f;

    // Checks if player does the jump action (raise both hands above head)
    private bool CanJump() =>  HandsAboveJumpLimit && lastFrameHandsAboveJumpLimit && leftController.Velocity.magnitude > 1f && rightController.Velocity.magnitude > 1f;
}