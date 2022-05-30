using UnityEngine;

public class HandPresence : MonoBehaviour
{
    [SerializeField] private ControllerSide side;
    [SerializeField] private Animator animator;

    private Hand hand;
    private bool isFlat;
    private bool isEdge;

    private void Start() {
        SetHand();
    }

    public void SetHand() {
        hand = HandInteractionManager.Instance.GetTrackedHand(side);
    }

    private void Update() {
        if (isFlat || isEdge) return;
        AnimateHand();
    }

    private void AnimateHand() {
        animator.SetFloat("Grip", hand.GripValue);
        animator.SetFloat("Trigger", hand.TriggerValue);
    }

    public void OnFlatSurface(bool active) {
        isFlat = active;
        animator.SetBool("isFlat", active);
    }
    
    public void OnEdge(bool active) {
        isEdge = active;
        animator.SetBool("isEdge", active);
    }
}
