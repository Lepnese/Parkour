using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Climber : MonoBehaviour
{
    public static Hand climbingHand;
    private PlayerController player;
    private ContinuousMoveProviderBase joystickMovement;
    private CharacterController controller;

    private void Awake() {
        player = GetComponent<PlayerController>();
        controller = GetComponent<CharacterController>();
        joystickMovement = GetComponent<ContinuousMoveProviderBase>();
    }

    private void FixedUpdate() {
        if (climbingHand) {
            // player.StartClimb(climbingHand);
            joystickMovement.enabled = false;
        }
        else {
            // player.StopClimb(climbingHand);
            joystickMovement.enabled = true;
        }
    }
}