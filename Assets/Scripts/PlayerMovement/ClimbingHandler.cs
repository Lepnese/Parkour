using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClimbingHandler : MonoBehaviour
{
    [SerializeField] private Transform feet;
    [SerializeField] private ContinuousMoveProviderBase moveProvider;
    [SerializeField] private float teleportingSpeed = 5f;

    private PhysicsHands2 currentHandGrip = null;
    private Camera cam;
    private CapsuleCollider col;
    private PlayerController2 playerController;
    private bool isTeleporting;
    private bool finishedTeleporting;
    private Vector3 target;
    
    private void Awake() {
        cam = Camera.main;
        col = GetComponent<CapsuleCollider>();
        playerController = GetComponent<PlayerController2>();
    }

    private void Update() {
        if (!isTeleporting) return;
        feet.position = Vector3.MoveTowards(feet.position, target, 5f * Time.deltaTime);
        moveProvider.enabled = true;
        col.enabled = true;
        isTeleporting = feet.position != target;
    }

    public void OnClimb() {
        if (playerController.IsGrounded) return;
        feet.position = cam.transform.position;
        col.enabled = false;
        moveProvider.enabled = false;
        FindLedgeClimbPoint();
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
        FindLedgeClimbPoint();
    }

    private void FindLedgeClimbPoint() {
        if (Physics.Raycast(cam.transform.position, Vector3.down, out RaycastHit hit, col.height / 2f)) {
            target = new Vector3(hit.point.x, hit.point.y + 0.2f, hit.point.z);
            isTeleporting = true;
        }
    }
}
