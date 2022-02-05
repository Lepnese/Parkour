using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerVelocity : MonoBehaviour
{
    public InputActionReference velocityReference = null;
    
    public float Velocity { 
        get {
            Vector3 velocity = velocityReference.action.ReadValue<Vector3>();
            return velocity.magnitude;
        }
    }
}