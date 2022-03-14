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

    private PhysicsHands2 currentHandGrip = null;
    private CapsuleCollider col;
    private PlayerController2 playerController;
    private Vector3 targetPos;
    private Rigidbody rb;
    private ColliderFollow colFollow;
    private Camera cam;

    private List<BoxCollider> collisionList;
    public List<BoxCollider> CollisionList => collisionList;
    
    private void Awake() {
        cam = Camera.main;
        col = GetComponent<CapsuleCollider>();
        playerController = GetComponent<PlayerController2>();
        rb = GetComponent<Rigidbody>();
        colFollow = GetComponent<ColliderFollow>();
        collisionList = new List<BoxCollider>();
    }

    private void Update() {
        Debug.DrawRay(cam.transform.position, Vector3.down * rayLength);
    }

    public void OnClimbStart(PhysicsHands2 hand) {
        moveProvider.enabled = false;
        col.enabled = false;
        currentHandGrip = hand;
    }

    public void OnClimbEnd(PhysicsHands2 hand) {
        if (currentHandGrip != hand) return;
        
        moveProvider.enabled = true;
        col.enabled = true;
        currentHandGrip = null;
        FindLedge();
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
        
        print($"Enter {collisionList.Count}");
    }

    private void OnTriggerExit(Collider other) {
        if (!other.gameObject.CompareTag(Tags.DefaultClimbable)) return;
        
        var childColliders = other.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("ClimbArea")).ToArray();
        if (childColliders.Length != 1) return;
        
        BoxCollider childCollider = childColliders[0].GetComponent<BoxCollider>();
        collisionList.Remove(childCollider);
        Utility.RemoveClimbArea(childCollider);
        
        print($"Exit {collisionList.Count}");
    }

    // public void OnTriggerExited(BoxCollider currentCollider) {
    //     if (!leftPhysicsHand.CollisionList.Contains(currentCollider) && !rightPhysicsHand.CollisionList.Contains(currentCollider))
    //         Utility.RemoveClimbArea(currentCollider);
    //     else
    //         print("contained in one hand");
    // }
}
