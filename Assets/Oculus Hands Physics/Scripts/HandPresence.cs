using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class HandPresence : MonoBehaviour
{
    public ActionBasedController controller;
    [SerializeField] private Animator handAnimator;

    private Hand hand;
    private float gripValue;
    private float triggerValue;

    private void Awake() {
        hand = controller.GetComponent<Hand>();
    }

    private void Update() {
        handAnimator.SetFloat("Grip", hand.GripValue);
        handAnimator.SetFloat("Trigger", hand.TriggerValue);
    }
}
