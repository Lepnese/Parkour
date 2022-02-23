using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public Text txt;
    //test

    #region Public Fields
    public GameObject _groundChecker;
    public GameObject LeftHandObj;
    public GameObject RightHandObj;
    public GameObject CenterEyeCamera;
    public float Speed = 2f;
    public float JumpHeight = 2f;
    public LayerMask Ground;
    #endregion

    private CharacterController playerController;
    
    private int FrameCounter;
    private const float Gravity = 9.81f;
    private bool _isGrounded;

    private float[] VelocityArray;
    private Vector3[] SpeedArray;
    
    private ControllerVelocity LeftController;
    private ControllerVelocity RightController;
    
    private Vector3 PlayerVelocity;
    private Vector3 LastFramePosition;
    private Vector3 ForwardDirection;
    private Vector3 AveragePlayerSpeed;

    private Transform _transform;
    private Transform CameraTransform;


    public float AverageHandSpeed {
        get => VelocityArray.Average();
    }

    private void Awake() {
        LeftController = LeftHandObj.GetComponent<ControllerVelocity>();
        RightController = RightHandObj.GetComponent<ControllerVelocity>();
        playerController = GetComponent<CharacterController>();
    }

    private void Start() {
        VelocityArray = new float[60];
        SpeedArray = new Vector3[30];
        _transform = transform;
        CameraTransform = CenterEyeCamera.transform;
        LastFramePosition = _transform.position;
    }

    private void Update() {
        if (Time.timeSinceLevelLoad < 1f) return;
        FrameCounter++;

        AveragePlayerSpeed = GetAveragePlayerSpeed();
        ForwardDirection = CameraTransform.TransformDirection(Vector3.forward);

        txt.text = AveragePlayerSpeed.magnitude.ToString();

        TrackHandSpeed();

        // _isGrounded = Physics.CheckSphere(_groundChecker.transform.position, SphereSize, Ground, QueryTriggerInteraction.Ignore);
        _isGrounded = Physics.Raycast(_groundChecker.transform.position, transform.TransformDirection(Vector3.down), 0.05f, Ground);
        if (_isGrounded && PlayerVelocity.y < 0.1f) {
            PlayerVelocity.y = 0f;
        }

        if (_isGrounded) {
            ManageRun();
            TrackPlayerSpeed();
            ManageJump();
        }


        // Applying gravity to player
        PlayerVelocity.y -= Gravity * Time.deltaTime;
        playerController.Move(PlayerVelocity * Time.deltaTime);
        
        PlayerVelocity.x = AveragePlayerSpeed.x;
        PlayerVelocity.z = AveragePlayerSpeed.z;

        LastFramePosition = _transform.position;
    }

    private void TrackPlayerSpeed() {
        SpeedArray[FrameCounter % SpeedArray.Length] = (_transform.position - LastFramePosition) / Time.smoothDeltaTime;
    }

    private void TrackHandSpeed() {
        VelocityArray[FrameCounter % VelocityArray.Length] = (LeftController.Velocity + RightController.Velocity) / 2;
    }

    private void ManageRun() {
        if (IsMoving()) {
            playerController.SimpleMove(ForwardDirection * AverageHandSpeed * Speed);
            // playerController.Move(ForwardDirection.transform.forward * AverageHandSpeed * Speed * Time.smoothDeltaTime);
        }
    }

    private void ManageJump() {        
        if (JumpAction()) {
            PlayerVelocity.x = AveragePlayerSpeed.x;
            PlayerVelocity.z = AveragePlayerSpeed.z;
            PlayerVelocity.y += Mathf.Sqrt(JumpHeight * 2f * Gravity);
        }
    }

    // Returns the average velocity (from SpeedArray) of the player
    private Vector3 GetAveragePlayerSpeed() => SpeedArray.Aggregate(Vector3.zero, (acc, v) => acc + v) / SpeedArray.Length;

    // Checks if player has a speed greater than 1 (joystick movement);
    private bool IsMoving() => playerController.velocity.magnitude > 1f;

    // Checks if player does the jump action (raise both hands above head)
    private bool JumpAction() => LeftController.Velocity > 1f && RightController.Velocity > 1f && LeftHandObj.transform.position.y > CenterEyeCamera.transform.position.y && RightHandObj.transform.position.y > CenterEyeCamera.transform.position.y;
}