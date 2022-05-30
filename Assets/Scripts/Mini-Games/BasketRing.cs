using UnityEngine;

public class BasketRing : MonoBehaviour
{
    [SerializeField] private VoidEvent onScoredEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BasketBall"))
            onScoredEvent.Raise();
    }
}
