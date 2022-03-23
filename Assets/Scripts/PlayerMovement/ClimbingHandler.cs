using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClimbingHandler : MonoBehaviour
{
    [Header("Climbing Values")]
    [SerializeField] private Vector3 relativeClimbAreaSize = new Vector3(1f, 0.1f, 1f);
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
    
    private void OnTriggerEnter(Collider other) {
        if (IsNotClimbableLayer(other.gameObject)) 
            return;

        if (other.gameObject.CompareTag(Tags.Climbable)) {
            other.gameObject.AddComponent<BoxCollider>();
            return;
        }
        
        Utility.CreateClimbArea(other.gameObject, relativeClimbAreaSize);
    }

    private static bool IsNotClimbableLayer(GameObject obj)
        => obj.CompareTag(Tags.NotClimbable) || obj.CompareTag(Tags.Player) || obj.layer == LayerMask.NameToLayer("UI");

    private void OnTriggerExit(Collider other) {
        if (other.GetType() != typeof(BoxCollider)) return;
        if (IsNotClimbableLayer(other.gameObject)) return;
        
        Transform[] ledgeColliders = other.GetComponentsInChildren<Transform>().Where(t => t.CompareTag(Tags.Climbable)).ToArray();
        
        if (ledgeColliders.Length != 1)
            throw new Exception("Ledge child object has more than 1 collider");

        BoxCollider ledgeCollider = ledgeColliders[0].GetComponent<BoxCollider>();
        Utility.RemoveClimbArea(ledgeCollider);
    }
}
