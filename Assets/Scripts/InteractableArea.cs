using UnityEngine;

public class InteractableArea : MonoBehaviour
{
    [SerializeField] private BoolEvent toggleHandInteraction;

    private float exitTime;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Tags.Player) && Time.time > exitTime + 0.2f) {
            print(true);
            toggleHandInteraction.Raise(true);
        }
    }
    
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(Tags.Player)) {
            print(false);
            toggleHandInteraction.Raise(false);
            exitTime = Time.time;
        }
    }
}
