using System;
using UnityEngine;

public class PhysicalHands : MonoBehaviour
{
    [SerializeField] private Transform handTarget;
    [SerializeField] private float forceAmount;
    
    public Vector3 Position { get; private set; }

    private Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        Vector3 dir = handTarget.position - transform.position;
        Position = dir.normalized * dir.magnitude * forceAmount;
        
        rb.AddForce(Position);
    }
}
