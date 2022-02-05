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
    public float JumpSpeed = 20f;

    public float Speed = 70f;
    private float HandSpeed;

    private float[] VelocityArray;

    private int FrameCounter;
    private ControllerVelocity LeftController;
    private ControllerVelocity RightController;
    private Vector3 PlayerVelocity;

    private CharacterController playerController;

    private void Awake() {
        LeftController = LeftHandObj.GetComponent<ControllerVelocity>();
        RightController = RightHandObj.GetComponent<ControllerVelocity>();
        playerController = GetComponent<CharacterController>();
    }

    private void Start() {
        VelocityArray = new float[20];
    }

    private void FixedUpdate() {
        FrameCounter++;
        if (FrameCounter >= VelocityArray.Length - 1) {
            FrameCounter = 0;
        }

        SetForwardDirection();
        HandSpeed = GetAverageHandSpeed();

        if (playerController.isGrounded) {
            ManageRun();
            ManageJump();
        }

        PlayerVelocity.y += -9.81f * Time.deltaTime;
        playerController.Move(PlayerVelocity * Time.deltaTime);
    }

    private void SetForwardDirection() {
        float yRotation = CenterEyeCamera.transform.eulerAngles.y;  
        ForwardDirection.transform.eulerAngles = new Vector3(0, yRotation, 0);
    }

    private void ManageRun() {
        if (playerController.velocity.magnitude > 1f) {
            playerController.Move(ForwardDirection.transform.forward * HandSpeed * Speed * Time.deltaTime);
        }
    }

    private void ManageJump() {
        if (LeftController.Velocity < 1f || RightController.Velocity < 1f) return;

        if (LeftHandObj.transform.position.y > CenterEyeCamera.transform.position.y && RightHandObj.transform.position.y > CenterEyeCamera.transform.position.y) {
            PlayerVelocity.y += Mathf.Sqrt(1f * -3.0f * -9.81f);
        }
    }

    private float GetAverageHandSpeed() {
        VelocityArray[FrameCounter] = (LeftController.Velocity + RightController.Velocity) / 2;
        return VelocityArray.Average();
    }
}