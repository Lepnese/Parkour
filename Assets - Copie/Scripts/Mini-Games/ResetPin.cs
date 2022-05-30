using UnityEngine;

public class ResetPin : MonoBehaviour
{
    [SerializeField] private Pin[] pins;

    private bool isReset;
    
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("ButtonPusher")) return;

        if (!isReset)
            ResetPins();
    
        isReset = true;
    }

    private void OnTriggerExit(Collider other) {
        isReset = false;
    }

    private void ResetPins() {
        foreach (var pin in pins) {
            pin.Reset();
        }        
    }
}
