using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Ragdoll : MonoBehaviour
{
    private Rigidbody[] rigidbodies;
    private Animator animator;

    private void Awake() {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public void DeactivateRagdoll() {
        animator.enabled = true;
        
        foreach (var rb in rigidbodies) {
            rb.isKinematic = true;
        }
    }
    
    public void ActivateRagdoll() {
        animator.enabled = false;
        
        foreach (var rb in rigidbodies) {
            rb.isKinematic = false;

            var interactable = rb.gameObject.GetComponent<XRGrabInteractable>();
            if (!interactable) {
                var grab = rb.gameObject.AddComponent<XRGrabInteractable>();
                grab.throwVelocityScale = 3f;
            }
        }
    }

    public void ApplyForce(Vector3 force) {
        var rb = animator.GetBoneTransform(HumanBodyBones.Hips).GetComponent<Rigidbody>();
        rb.AddForce(force, ForceMode.VelocityChange);
    }
}
