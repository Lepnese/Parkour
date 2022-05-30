using UnityEngine;

public class ActivateRespawn : MonoBehaviour
{
    [SerializeField] private VoidEvent onPlayerFall;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Tags.Player)) {
            onPlayerFall.Raise();
        }
    }
}
