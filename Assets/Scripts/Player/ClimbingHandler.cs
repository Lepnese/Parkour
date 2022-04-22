using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClimbingHandler : MonoBehaviour
{
    [Header("Climbing Values")] 
    [SerializeField] private float vaultForce = 5f;
    [SerializeField] private float rayLength = 0.5f;
    [Space]
    [SerializeField] private ContinuousMoveProviderBase moveProvider;

    private PhysicsHands2 currentClimbingHand;
    private Rigidbody rb;
    private ColliderFollow colFollow;
    private Camera cam;

    private void Awake() {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        colFollow = GetComponent<ColliderFollow>();
    }
    
    public void OnClimbStart(PhysicsHands2 hand) {
        moveProvider.enabled = false;
        colFollow.SetIsClimbing(true);
        currentClimbingHand = hand;
        rb.useGravity = false;
    }

    public void OnClimbEnd(PhysicsHands2 hand) {
        if (currentClimbingHand != hand) return;

        currentClimbingHand = null;
        rb.useGravity = true;
        
        StartCoroutine(FindLedge());
    }

    private IEnumerator FindLedge() {
        while (!Physics.Raycast(cam.transform.position, Vector3.down, rayLength)) {
            Debug.DrawRay(cam.transform.position, Vector3.down * rayLength);
            yield return null;
        }

        colFollow.SetIsClimbing(false);
        rb.AddForce(vaultForce * Vector3.up, ForceMode.Impulse);
        moveProvider.enabled = true;
    }
}
