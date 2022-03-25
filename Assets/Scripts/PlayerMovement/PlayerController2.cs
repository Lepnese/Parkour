using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController2 : MonoBehaviour
{
    public bool isTrue;
    [Header("Player")]
    [SerializeField] private GameObject leftHandObj;
    [SerializeField] private GameObject rightHandObj;
    [SerializeField] private GameObject centerEyeCamera;
    [SerializeField] private ActionBasedContinuousMoveProvider moveProvider;
    [SerializeField] private LayerMask groundLayers;

    [Header("Sprint")]
    [SerializeField] private float sprintSpeed = 200f;
    [SerializeField] private float maxSpeed = 16f;
    [SerializeField] private float sprintMultiplier = 10f;
    [SerializeField] private float airMultiplier = 0.4f;

    [Header("Jump")]
    [SerializeField] private float jumpVelocity = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    
    [Header("Drag")] 
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 2f;

    private int frameCounter;
    private bool isGrounded;
    private bool isJumpBtnDown;
    private bool handVelocityTrackingActive;

    private float[] velocityArray;
    private Vector3[] speedArray;
    private List<float> jumpingHandVelocity;
    
    private Hand leftHand;
    private Hand rightHand;
    
    private Vector3 lastFramePosition;
    private Vector3 forwardDirection;

    private CapsuleCollider col;
    private Transform cameraTransform;
    private Rigidbody rb;
    


    private float AverageHandSpeed => velocityArray.Average();

    private float AverageJumpingHandSpeed => jumpingHandVelocity.Average();
    
    private void Awake() {
        jumpingHandVelocity = new List<float>();
        
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        leftHand = leftHandObj.GetComponent<Hand>();
        rightHand = rightHandObj.GetComponent<Hand>();
    }

    private void Start() {
        velocityArray = new float[30];
        speedArray = new Vector3[30];
        cameraTransform = centerEyeCamera.transform;
        lastFramePosition = transform.position;
        handVelocityTrackingActive = true;
    }

    private void Update() {
        frameCounter++;

        isGrounded = Physics.Raycast(
            cameraTransform.position,
            Vector3.down, col.height + 0.05f, groundLayers);
        
        Debug.DrawRay(cameraTransform.position,
                    Vector3.down * (col.height + 0.05f));
        
        forwardDirection = cameraTransform.TransformDirection(Vector3.forward).normalized;
        
        TrackPlayerSpeed();
        TrackHandSpeed();
        ControlDrag();
        if (isTrue)
            AdjustGravity();
    }

    private void AdjustGravity() {
        if (rb.velocity.y < 0)
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if (rb.velocity.y > 0 && !isJumpBtnDown)
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
    }

    private IEnumerator TrackJumpingHandSpeed() {
        jumpingHandVelocity.Clear();
        
        while (isJumpBtnDown) {
            jumpingHandVelocity.Add((leftHand.Velocity.magnitude + rightHand.Velocity.magnitude) / 2);
            yield return null;
        }
    }

    private void FixedUpdate() {
        if (!isGrounded) return;
        if (!IsJoystickMoving()) return;
        Run();
        // ManageRun();
    }

    private void LateUpdate() {
        lastFramePosition = transform.position;
    }

    private void TrackPlayerSpeed() => speedArray[frameCounter % speedArray.Length] = (transform.position - lastFramePosition) / Time.deltaTime;

    private void TrackHandSpeed() {
        if (leftHand.Velocity.magnitude > 0.7f && rightHand.Velocity.magnitude > 0.7f) {
            velocityArray[frameCounter % velocityArray.Length] = (leftHand.Velocity.magnitude + rightHand.Velocity.magnitude) / 2;
        }
        else {
            velocityArray[frameCounter % velocityArray.Length] = 0f;
        }
    } 

    private void ControlDrag() {
        rb.drag = isGrounded ? groundDrag : airDrag;
    }

    private void Run() {
        rb.AddForce(forwardDirection * sprintSpeed * AverageHandSpeed);

        // if (rb.velocity.magnitude < maxSpeed) return;
        //
        // var normalized = Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2));
        // var xNorm = rb.velocity.x / normalized;
        // var zNorm = rb.velocity.z / normalized;
        //
        // rb.velocity = new Vector3(xNorm * maxSpeed, rb.velocity.y, zNorm * maxSpeed);
        // rb.velocity = rb.velocity.normalized * maxSpeed;
    }
    
    private void ManageRun() {
        if (isGrounded)
            rb.AddForce(forwardDirection.normalized * sprintSpeed * sprintMultiplier * AverageHandSpeed,
                ForceMode.Acceleration);
        else
            rb.AddForce(forwardDirection.normalized * sprintSpeed * sprintMultiplier * airMultiplier * AverageHandSpeed,
                ForceMode.Acceleration);
    }

    private void Jump() {
        var handSpeed = (leftHand.Velocity.magnitude + rightHand.Velocity.magnitude) / 2f;
        var vel = rb.velocity;
        rb.velocity = new Vector3(vel.x, handSpeed * jumpVelocity, vel.z);

        // rb.AddForce(jumpForce * Vector3.up * AverageJumpingHandSpeed, ForceMode.Impulse);
    }

    // Checks if player is moving with the joystick
    private bool IsJoystickMoving() => moveProvider.leftHandMoveAction.action.ReadValue<Vector2>() != Vector2.zero;

    public void OnJumpBtnDown() {
        isJumpBtnDown = true;
        handVelocityTrackingActive = false; 
        
        StartCoroutine(TrackJumpingHandSpeed());
    }

    public void OnJumpBtnUp() {
        isJumpBtnDown = false;
        handVelocityTrackingActive = true;
    
        if (isGrounded)
            Jump();
    }
}
