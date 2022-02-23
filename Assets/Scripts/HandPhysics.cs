using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPhysics : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        // Position
        // rb.position = target.position;
        rb.velocity = (target.position - transform.position) / Time.deltaTime;

        // Rotation
        rb.rotation = target.rotation;
        // Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        // rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        // Vector3 rotationDifferenceInDegree = angleInDegree * rotationAxis;
        // rb.angularVelocity = rotationDifferenceInDegree * Mathf.Deg2Rad / Time.deltaTime;
    }
}