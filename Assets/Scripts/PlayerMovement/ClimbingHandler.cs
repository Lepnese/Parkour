using System.Collections.Generic;
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
    private CapsuleCollider col;
    private Rigidbody rb;
    private ColliderFollow colFollow;
    private Camera cam;

    private List<BoxCollider> collisionList;
    public List<BoxCollider> CollisionList => collisionList;
    
    private void Awake() {
        cam = Camera.main;
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        colFollow = GetComponent<ColliderFollow>();
        collisionList = new List<BoxCollider>();
    }

    private void Update() {
        Debug.DrawRay(cam.transform.position, Vector3.down * rayLength);
    }

    public void OnClimbStart(PhysicsHands2 hand) {
        moveProvider.enabled = false;
        colFollow.SetIsClimbing(true);
        currentClimbingHand = hand;
    }

    public void OnClimbEnd(PhysicsHands2 hand) {
        if (currentClimbingHand != hand) return;

        bool ledgeFound = Physics.Raycast(cam.transform.position, Vector3.down, rayLength);
        if (!ledgeFound) return;
        
        rb.AddForce(vaultForce * Vector3.up, ForceMode.Impulse);
        colFollow.SetIsClimbing(false);
        moveProvider.enabled = true;
        currentClimbingHand = null;
        // FindLedge();
    }

    private void FindLedge() {
        if (Physics.Raycast(cam.transform.position, Vector3.down, rayLength)) {
            rb.AddForce(vaultForce * Vector3.up, ForceMode.Impulse);
            colFollow.SetIsClimbing(false);
            moveProvider.enabled = true;
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag(Tags.DefaultClimbable)) return;
        
        var newLedge = Utility.CreateClimbArea(other.gameObject, relativeClimbAreaSize);
        
        if (!collisionList.Contains(newLedge))
            collisionList.Add(newLedge);
        
        print($"Exited entered - " +
              $"collisionList.Count = {collisionList.Count}");
    }

    private void OnTriggerExit(Collider other) {
        if (!other.gameObject.CompareTag(Tags.DefaultClimbable)) return;
        
        var childColliders = other.GetComponentsInChildren<Transform>().Where(t => t.CompareTag(Tags.ClimbArea)).ToArray();
        if (childColliders.Length != 1) return;
        
        BoxCollider childCollider = childColliders[0].GetComponent<BoxCollider>();
        collisionList.Remove(childCollider);
        Utility.RemoveClimbArea(childCollider);
        
        print($"Exited trigger - " +
              $"collisionList.Count = {collisionList.Count}");
    }
}
