using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;

public class ArmSwing : MonoBehaviour
{
    public InputActionReference moveReference = null;
    public GameObject LeftHand;
    public GameObject RightHand;
    public GameObject CenterEyeCamera;
    public GameObject ForwardDirection;
    public float MinRunSpeed = 0.8f;

    public Text text;

    public float Speed = 70f;
    private float HandSpeed;

    private float[] VelocityArray;
    private Vector3[] LeftHandPositions;
    private Vector3[] RightHandPositions;

    private int FrameCounter;
    private ControllerVelocity leftHandVelocity;
    private ControllerVelocity rightHandVelocity;
    private Vector2 joystickMovement;

    private CharacterController playerController;

    private void Awake() {
        leftHandVelocity = LeftHand.GetComponent<ControllerVelocity>();
        rightHandVelocity = RightHand.GetComponent<ControllerVelocity>();
        playerController = GetComponent<CharacterController>();
    }

    private void Start() {
        VelocityArray = new float[20];
        LeftHandPositions = new Vector3[300];
        RightHandPositions = new Vector3[300];
    }

    private void Update() {
        FrameCounter++;
        if (FrameCounter >= VelocityArray.Length - 1) {
            FrameCounter = 0;
        }
    
        HandSpeed = GetAverageHandSpeed();
        text.text = HandSpeed.ToString();



        float yRotation = CenterEyeCamera.transform.eulerAngles.y;  
        ForwardDirection.transform.eulerAngles = new Vector3(0, yRotation, 0);

        if (CanMove()) {
            playerController.Move(ForwardDirection.transform.forward * HandSpeed * Speed * Time.deltaTime);
        }
    }

    private float GetAverageHandSpeed() {
        VelocityArray[FrameCounter] = (leftHandVelocity.Velocity + rightHandVelocity.Velocity) / 2;
        return VelocityArray.Average();
    }

    private bool CanMove()
        => playerController.velocity.magnitude > 1f && playerController.isGrounded && HandSpeed > MinRunSpeed;
}