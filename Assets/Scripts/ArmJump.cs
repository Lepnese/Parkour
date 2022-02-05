using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmJump : MonoBehaviour
{
    public InputActionReference primaryReference = null;
    public GameObject LeftHand;
    public GameObject RightHand;

    private bool isPressed = false;

    // private void Awake() {
    //     primaryReference.action.started += Jump;
    // }

    // private void OnDestroy() {
    //     primaryReference.action.started -= Jump;
    // }

    private void Update() {
        isPressed = primaryReference.action.ReadValue<bool>();
        if (isPressed)
            print(isPressed);
    }

    // private void Jump(InputAction.CallbackContext ctx) {
    //     print(2)
    //     isPressed = !isPressed;
    //     print(isPressed);
    // }
}
