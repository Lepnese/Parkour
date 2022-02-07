using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public Text txt;

    public GameObject _groundChecker;
    public GameObject LeftHandObj;
    public GameObject RightHandObj;
    public GameObject CenterEyeCamera;
    public GameObject ForwardDirection;
    public float MinRunSpeed = 0.8f;
    public float Speed = 70f;
    public float JumpHeight = 2f;
    public LayerMask Ground;
    public float SphereSize;

    private float Gravity = 9.81f;
    private bool _isGrounded;

    private float[] VelocityArray;
    private Vector3[] SpeedArray;

    private int FrameCounter;
    private int FrameCounter2;
    private ControllerVelocity LeftController;
    private ControllerVelocity RightController;
    private Vector3 PlayerVelocity;

    private Vector3 LastFramePosition;

    private CharacterController playerController;

    public float AverageHandSpeed {
        get => VelocityArray.Average();
    }

    public Vector3 AverageCurSpeed {
        get => SpeedArray.Aggregate(Vector3.zero, (acc, v) => acc + v) / SpeedArray.Length;
    }

    private void Awake() {
        LeftController = LeftHandObj.GetComponent<ControllerVelocity>();
        RightController = RightHandObj.GetComponent<ControllerVelocity>();
        playerController = GetComponent<CharacterController>();
        
    }

    private void Start() {
        VelocityArray = new float[60];
        SpeedArray = new Vector3[30];
        LastFramePosition = transform.position;
    }

    private void Update() {
        if (Time.timeSinceLevelLoad < 1f) return;        

        txt.text = AverageCurSpeed.magnitude.ToString();

        TrackHandSpeed();
        SetForwardDirection();

        _isGrounded = Physics.CheckSphere(_groundChecker.transform.position, SphereSize, Ground, QueryTriggerInteraction.Ignore);
        if (_isGrounded && PlayerVelocity.y < 0)
            PlayerVelocity.y = 0f;

        if (_isGrounded) {
            ManageRun();
            TrackPlayerSpeed();
            ManageJump();
        }


        // Applying gravity to player
        PlayerVelocity.y -= Gravity * Time.deltaTime;
        playerController.Move(PlayerVelocity * Time.deltaTime);
        
        PlayerVelocity.x = AverageCurSpeed.x;
        PlayerVelocity.z = AverageCurSpeed.z;

        LastFramePosition = transform.position;
    }

    private void TrackPlayerSpeed() {
        FrameCounter2++;
        if (FrameCounter2 >= SpeedArray.Length - 1) {
            FrameCounter2 = 0;
        }
        SpeedArray[FrameCounter2] = (transform.position - LastFramePosition) / Time.smoothDeltaTime;
    }

    private void TrackHandSpeed() {
        FrameCounter++;
        if (FrameCounter >= VelocityArray.Length - 1) {
            FrameCounter = 0;
        }
        VelocityArray[FrameCounter] = (LeftController.Velocity + RightController.Velocity) / 2;
    }

    private void SetForwardDirection() {
        float yRotation = CenterEyeCamera.transform.eulerAngles.y;  
        ForwardDirection.transform.eulerAngles = new Vector3(0, yRotation, 0);
    }

    private void ManageRun() {
        if (IsMoving()) {
            playerController.SimpleMove(ForwardDirection.transform.forward * AverageHandSpeed * Speed);
            // playerController.Move(ForwardDirection.transform.forward * AverageHandSpeed * Speed * Time.smoothDeltaTime);
        }
    }

    private void ManageJump() {        
        if (JumpAction()) {
            PlayerVelocity.x = AverageCurSpeed.x;
            PlayerVelocity.z = AverageCurSpeed.z;
            PlayerVelocity.y += Mathf.Sqrt(JumpHeight * 2f * Gravity);
        }
    }

    // Checks if player has a speed greater than 1 (joystick movement);
    private bool IsMoving() => playerController.velocity.magnitude > 1f;

    // Checks if player does the jump action (raise both hands above head)
    private bool JumpAction() => LeftController.Velocity > 1f && RightController.Velocity > 1f && LeftHandObj.transform.position.y > CenterEyeCamera.transform.position.y && RightHandObj.transform.position.y > CenterEyeCamera.transform.position.y;
}