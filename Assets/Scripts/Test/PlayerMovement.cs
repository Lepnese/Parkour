using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rightHand;
    [SerializeField] private Rigidbody leftHand;

    private Rigidbody rb;
    private PhysicalHands rightPhysicalHand;
    private PhysicalHands leftPhysicalHand;
    
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        rightPhysicalHand = rightHand.GetComponent<PhysicalHands>();
        leftPhysicalHand = leftHand.GetComponent<PhysicalHands>();
    }

    private void FixedUpdate() {
        Climb();
    }

    private void Climb() {
        rb.AddForce(-rightPhysicalHand.Position);
        rb.AddForce(-leftPhysicalHand.Position);
    }
}
