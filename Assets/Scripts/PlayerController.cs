using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public Text txt;

    public GameObject LeftHandObj;
    public GameObject RightHandObj;
    public GameObject CenterEyeCamera;
    public GameObject ForwardDirection;
    public float MinRunSpeed = 0.8f;
    public float JumpSpeed = 0.5f;
    public float Speed = 70f;

    private float Gravity = 9.81f;

    private float[] VelocityArray;

    private int FrameCounter;
    private ControllerVelocity LeftController;
    private ControllerVelocity RightController;
    private Vector3 PlayerVelocity;
    private bool isJumping = false;

    private CharacterController playerController;

    public float AverageHandSpeed {
        get => VelocityArray.Average();
    }

    private void Awake() {
        LeftController = LeftHandObj.GetComponent<ControllerVelocity>();
        RightController = RightHandObj.GetComponent<ControllerVelocity>();
        playerController = GetComponent<CharacterController>();
    }

    private void Start() {
        VelocityArray = new float[30];
    }

    private void Update() {
        if (Time.timeSinceLevelLoad < 1f) return;
        
        TrackHandSpeed();
        SetForwardDirection();

        if (playerController.isGrounded) {
            ManageRun();
            ManageJump();
        }

        // Applying gravity to player
        // PlayerVelocity.y -= Gravity * Time.deltaTime;
        // playerController.Move(PlayerVelocity * Time.deltaTime);
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
            playerController.Move(ForwardDirection.transform.forward * AverageHandSpeed * Speed * Time.deltaTime);
        }
    }

    private void ManageJump() {
        if (!CanJump()) return;

        isJumping = JumpAction();
        if (isJumping) {
            txt.text = "jump";
            playerController.Move(Vector3.up * JumpSpeed * Gravity * Time.deltaTime);
            // StartCoroutine(EnableJump());
            // PlayerVelocity.y += Mathf.Sqrt(JumpSpeed * 3.0f * Gravity);
        }
        else {
            txt.text = "not jumping";
        }
    }

    IEnumerator EnableJump() {
        Gravity = -Gravity;
        yield return new WaitForSeconds(0.3f);
        Gravity = -Gravity;
    }

    // Checks if player has a speed greater than 1 (joystick movement);
    private bool IsMoving() => playerController.velocity.magnitude > 1f;

    // Can jump if both controllers are fast enough & player isn't already jumping
    private bool CanJump() => LeftController.Velocity > 1f && RightController.Velocity > 1f;

    // Checks if player does the jump action (raise both hands above head)
    private bool JumpAction() => LeftHandObj.transform.position.y > CenterEyeCamera.transform.position.y && RightHandObj.transform.position.y > CenterEyeCamera.transform.position.y;
}