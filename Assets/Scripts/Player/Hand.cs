using UnityEngine;
using UnityEngine.InputSystem;

public class Hand : MonoBehaviour
{
    [Header("Identification")]
    [SerializeField] private ControllerSide handName;
    [Header("Pressed Events")] 
    [SerializeField] private HandEvent onGripBtnDown;
    [SerializeField] private HandEvent onGripBtnUp;
    [SerializeField] private HandEvent onTriggerBtnDown;
    [SerializeField] private HandEvent onTriggerBtnUp;
    [SerializeField] private VoidEvent onJumpBtnDown;
    [SerializeField] private VoidEvent onJumpBtnUp;
    [Header("Values")]
    [SerializeField] private InputActionReference velocityReference = null;
    [SerializeField] private InputActionReference gripReference = null;
    [SerializeField] private InputActionReference triggerReference = null;
    [Header("Pressed")]
    [SerializeField] private InputActionReference gripPressedReference = null;
    [SerializeField] private InputActionReference triggerPressedReference = null;
    [SerializeField] private InputActionReference jumpBtnPressedReference = null;

    private bool isGripBtnDown;
    private bool isTriggerBtnDown;

    public ControllerSide Name => handName;
    public Vector3 Velocity => velocityReference.action.ReadValue<Vector3>();
    public float GripValue => gripReference.action.ReadValue<float>();
    public float TriggerValue => triggerReference.action.ReadValue<float>();

    protected virtual void Awake() {
        gripPressedReference.action.started += OnGripDown;
        gripPressedReference.action.canceled += OnGripUp;
        
        triggerPressedReference.action.started += OnTriggerDown;
        triggerReference.action.canceled += OnTriggerUp;
        
        jumpBtnPressedReference.action.started += OnJumpBtnDown;
        jumpBtnPressedReference.action.canceled += OnJumpBtnUp;
    }

    protected virtual void OnDestroy() {
        gripPressedReference.action.started -= OnGripDown;
        gripPressedReference.action.canceled -= OnGripUp;
        
        triggerPressedReference.action.started -= OnTriggerDown;
        triggerReference.action.canceled -= OnTriggerUp;
        
        jumpBtnPressedReference.action.started -= OnJumpBtnDown;
        jumpBtnPressedReference.action.canceled -= OnJumpBtnUp;
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
        onTriggerBtnDown.Raise(this);
    }
    
    private void OnTriggerUp(InputAction.CallbackContext ctx) {
        isTriggerBtnDown = false;
        onTriggerBtnUp.Raise(this);
    }
    
    private void OnJumpBtnDown(InputAction.CallbackContext obj) {
        if (handName == ControllerSide.Right)
            onJumpBtnDown.Raise();
    }
    
    private void OnJumpBtnUp(InputAction.CallbackContext obj) {
        if (handName == ControllerSide.Right)
            onJumpBtnUp.Raise();
    }
}

public enum ControllerSide
{
    Left,
    Right
}