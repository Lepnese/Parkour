using UnityEngine;

public class ResetPin : MonoBehaviour
{
    [SerializeField] private VoidEvent onPinReset;
    private bool isReset;
    
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("ButtonPusher")) return;

        if (!isReset)
            onPinReset.Raise();
    
        isReset = true;
    }

    private void OnTriggerExit(Collider other) {
        isReset = false;
    }
}
