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

    private bool triggerActive;
    
    public Vector3 Velocity {
        get => velocityReference.action.ReadValue<Vector3>();
    }

    public float GripValue {
        get => gripReference.action.ReadValue<float>();
    }

    public float TriggerValue {
        get => triggerReference.action.ReadValue<float>();
    }

    public bool TriggerPressed { get; private set; }

    public bool GripPressed { get; private set; }

    private void Update() {
        if (TriggerValue < 0.1f) return;

        if (!TriggerPressed)
            TriggerPressed = true;
        
        else if (TriggerPressed)
            TriggerPressed = false;
    }
}