using UnityEngine;

public class GunInteraction : MonoBehaviour
{
    [SerializeField] private float magCapacity = 16f;
    
    private Camera cam;
    private PhysicsHands2 currentHand;
    private GameObject gun;

    private float currentAmmo;
    public float CurrentAmmo => currentAmmo;
    public Hand CurrentHand => currentHand.TrackedHand;
    
    private void Awake() {
        cam = Camera.main;
    }

    private void Start() {
        currentAmmo = magCapacity;
    }

    public void OnGripBtnDown(PhysicsHands2 hand) {
        if (currentHand || HandInFOV(hand)) return;

        currentHand = hand;
        gun = hand.AttachedGun;
        ToggleGunActive(true);
    }

    private void ToggleGunActive(bool active) {
        gun.SetActive(active);
        currentHand.ToggleHandModel(!active, 0f);
        currentHand.SetState(active ? PhysHandState.Gun : PhysHandState.Hand);
    }

    public void OnGripBtnUp(PhysicsHands2 hand) {
        if (hand != currentHand) return;

        ToggleGunActive(false);
        currentHand = null;
    }
    
    private bool HandInFOV(PhysicsHands2 hand) {
        Vector3 screenPoint = cam.WorldToViewportPoint(hand.transform.position);
        bool inFOV = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        
        return inFOV;
    }

    public void ReduceAmmo() {
        currentAmmo--;
    }
}