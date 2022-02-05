using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class SwingingArms : MonoBehaviour
{
    public GameObject LeftHand;
    public GameObject RightHand;
    public GameObject CenterEyeCamera;
    public GameObject ForwardDirection;

    public float Speed = 70f;
    private float HandSpeed;

    private XRBaseController controller;

    private Vector3 PreviousFrameLeft;
    private Vector3 PreviousFrameRight;
    private Vector3 PreviousFramePosition;
    private Vector3 ThisFramePosition;
    private Vector3 ThisFrameLeft;
    private Vector3 ThisFrameRight;

    private void Awake() {
        controller = LeftHand.GetComponent<XRBaseController>();
    }

    private void Start() {
        PreviousFrameLeft = LeftHand.transform.position;
        PreviousFrameRight = RightHand.transform.position;
        PreviousFramePosition = transform.position;
    }

    private void Update() {
        float yRotation = CenterEyeCamera.transform.eulerAngles.y;
        ForwardDirection.transform.eulerAngles = new Vector3(0, yRotation, 0);

        ThisFrameLeft = LeftHand.transform.position;
        ThisFrameRight = RightHand.transform.position;
        ThisFramePosition = transform.position;

        var playerDistanceMoved = Vector3.Distance(ThisFramePosition, PreviousFramePosition);
        var leftDistanceMoved = Vector3.Distance(ThisFrameLeft, PreviousFrameLeft);
        var rightDistance = Vector3.Distance(ThisFrameRight, PreviousFrameRight);

        HandSpeed = (leftDistanceMoved - playerDistanceMoved) + (rightDistance - playerDistanceMoved);

        if (Time.timeSinceLevelLoad > 1f)
            transform.position += ForwardDirection.transform.forward * HandSpeed * Speed * Time.deltaTime;

        PreviousFrameLeft = ThisFrameLeft;
        PreviousFrameRight = ThisFrameRight;
        PreviousFramePosition = ThisFramePosition;
    }
}