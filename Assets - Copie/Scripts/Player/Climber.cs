using System;
using System.Collections;
using UnityEngine;

public class Climber : MonoBehaviour
{
    public Transform sphere;
    [SerializeField] private ConfigurableJoint climberHandler;

    private bool isClimbing;
    private PhysicsHands2 activeHand;
    private Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        if (isClimbing) {
            climberHandler.targetPosition = activeHand.TrackedHand.transform.position;
            sphere.position = climberHandler.targetPosition;
        }
    }

    public void OnClimbEnd(PhysicsHands2 hand) {
        if (hand != activeHand) return;
        
        climberHandler.connectedBody = null;
        isClimbing = false;
        rb.useGravity = true;
    }

    public void OnClimbStart(PhysicsHands2 hand) {
        // if (isClimbing && hand == activeHand) return;
        if (!hand.CanClimb) return; 
        
        StartCoroutine(UpdateHand(hand));
    }

    private IEnumerator UpdateHand(PhysicsHands2 hand) {
        while (hand.TrackedHand.GetHandButton(1)) {
            activeHand = hand;
            isClimbing = true;
            climberHandler.transform.position = hand.transform.position;
            rb.useGravity = false;
            climberHandler.connectedBody = rb;
            
            yield return null;
        }
    }
}