using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandPresence : MonoBehaviour
{
    public ActionBasedController controller;
    [SerializeField] private Hand hand;
    [SerializeField] private Animator animator;
    
    private bool isFlat;
    private bool isEdge;
    private const string AnimatorGripParam = "Grip";
    private const string AnimatorTriggerParam = "Trigger";
    private const string AnimatorFlatParam = "isFlat";
    private const string AnimatorEdgeParam = "isEdge";

    private void Awake() {
        controller = GetComponent<ActionBasedController>();
    }

    private void Update() {
        if (isFlat || isEdge) return;
        AnimateHand();
    }

    private void AnimateHand() {
        animator.SetFloat(AnimatorGripParam, hand.GripValue);
        animator.SetFloat(AnimatorTriggerParam, hand.TriggerValue);
    }

    public void OnFlatSurface(bool active) {
        isFlat = active;
        animator.SetBool(AnimatorFlatParam, active);
    }
    
    public void OnEdge(bool active) {
        isEdge = active;
        animator.SetBool(AnimatorEdgeParam, active);
    }
}
