using System;
using UnityEngine;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine.Animations.Rigging;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController2 : MonoBehaviour
{
    [SerializeField] private GameObject groundChecker;
    [SerializeField] private GameObject leftHandObj;
    [SerializeField] private GameObject rightHandObj;
    [SerializeField] private GameObject centerEyeCamera;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float a = 0.25f;
    [SerializeField] private ActionBasedContinuousMoveProvider move;

    private int frameCounter;
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

    private bool isGrounded;
    private Transform cameraTransform;
    private Collider col;
    private float distToGround;

    public Rigidbody PlayerRigidbody { get; private set; }
    
    private float AverageHandSpeed {
        get => velocityArray.Average();
    }

    private bool HandsAboveJumpLimit {
        get => leftHandObj.transform.position.y >  cameraTransform.position.y - a && rightHandObj.transform.position.y >  cameraTransform.position.y - a;
    }

    private bool IsGrounded {
        get => Physics.Raycast(transform.position, Vector3.down, distToGround + 0.1f);
        // get => Physics.Raycast(groundChecker.transform.position, transform.TransformDirection(Vector3.down), 0.05f);
    }

    private void Awake() {
        col = GetComponent<Collider>();
        PlayerRigidbody = GetComponent<Rigidbody>();
        leftController = leftHandObj.GetComponent<Hand>();
        rightController = rightHandObj.GetComponent<Hand>();
    }

    private void Start() {
        distToGround = col.bounds.extents.y;
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
        
        //if (!isGrounded) return;
        ManageRun();
        ManageJump();
    }

    private void LateUpdate() {
        lastFramePosition = transform.position;
        lastFrameHandsAboveJumpLimit = !HandsAboveJumpLimit;
    }

    private void TrackPlayerSpeed() => speedArray[frameCounter % speedArray.Length] = (transform.position - lastFramePosition) / Time.deltaTime;
    private void TrackHandSpeed() => velocityArray[frameCounter % velocityArray.Length] = (leftController.Velocity.magnitude + rightController.Velocity.magnitude) / 2;

    private void ManageRun() {
        if (!IsJoystickMoving()) return;
        PlayerRigidbody.velocity = new Vector3(forwardDirection.x * speed * AverageHandSpeed,
            PlayerRigidbody.velocity.y, forwardDirection.z * speed * AverageHandSpeed);
        // PlayerRigidbody.AddForce(forwardDirection * speed * AverageHandSpeed, ForceMode.Acceleration);
        // PlayerRigidbody.AddForce(forwardDirection * speed * AverageHandSpeed * Time.deltaTime);
    }

    private void ManageJump() {
        if (!CanJump()) return;
        PlayerRigidbody.AddForce(jumpHeight * Vector3.up, ForceMode.Impulse);
    }
    
    public void Climb(Vector3 force) {
        PlayerRigidbody.AddForce(force, ForceMode.Acceleration);
    }

    // Returns the average velocity (from SpeedArray) of the player
    private Vector3 GetAveragePlayerSpeed() => speedArray.Aggregate(Vector3.zero, (acc, v) => acc + v) / speedArray.Length;

    // Checks if player has a speed greater than 1 (joystick movement);
    private bool IsJoystickMoving() => move.leftHandMoveAction.action.ReadValue<Vector2>() != Vector2.zero && AverageHandSpeed > 3f;

    // Checks if player does the jump action (raise both hands above head with speed)
    private bool CanJump() => HandsAboveJumpLimit && lastFrameHandsAboveJumpLimit && leftController.Velocity.magnitude > 1f && rightController.Velocity.magnitude > 1f;
}