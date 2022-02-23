using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hand : MonoBehaviour
{
    [SerializeField] private InputActionReference velocityReference = null;
    [SerializeField] private InputActionReference gripReference = null;
    [SerializeField] private InputActionReference triggerReference = null;

    public Vector3 Velocity { 
        get => velocityReference.action.ReadValue<Vector3>();
    }

    public float GripValue {
        get => gripReference.action.ReadValue<float>();
    }

    public float TriggerValue {
        get => triggerReference.action.ReadValue<float>();
    }

}