using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hand : MonoBehaviour
{
    [Header("Pressed Events")] 
    [SerializeField] private HandEvent onGripBtnDown;
    [SerializeField] private HandEvent onGripBtnUp;
    [Header("Values")]
    [SerializeField] private InputActionReference velocityReference = null;
    [SerializeField] private InputActionReference gripReference = null;
    [SerializeField] private InputActionReference triggerReference = null;
    [Header("Pressed")]
    [SerializeField] private InputActionReference gripPressedReference = null;
    [SerializeField] private InputActionReference triggerPressedReference = null;

    private bool isGripBtnDown;
    private bool isTriggerBtnDown;
    
    public Vector3 Velocity => velocityReference.action.ReadValue<Vector3>();
    public float GripValue => gripReference.action.ReadValue<float>();
    public float TriggerValue => triggerReference.action.ReadValue<float>();

    private void Awake() {
        gripPressedReference.action.started += OnGripDown;
        gripPressedReference.action.canceled += OnGripUp;

        triggerPressedReference.action.started += OnTriggerDown;
        triggerReference.action.canceled += OnTriggerUp;
    }

    private void OnDestroy() {
        gripPressedReference.action.started -= OnGripDown;
        gripPressedReference.action.canceled -= OnGripUp;
        
        triggerPressedReference.action.started -= OnTriggerDown;
        triggerReference.action.canceled -= OnTriggerUp;
    }

    public bool GetHandButton(int btn) {
        return btn switch {
            0 => isTriggerBtnDown,
            1 => isGripBtnDown,
            _ => false
        };
    }

    private void OnGripDown(InputAction.CallbackContext ctx) {
        isGripBtnDown = true;
        onGripBtnDown.Raise(this);
    }
    
    private void OnGripUp(InputAction.CallbackContext ctx) {
        isGripBtnDown = false;
        onGripBtnUp.Raise(this);
    }

    private void OnTriggerDown(InputAction.CallbackContext ctx) {
        isTriggerBtnDown = true;
    }
    
    private void OnTriggerUp(InputAction.CallbackContext ctx) {
        isTriggerBtnDown = false;
    }
}