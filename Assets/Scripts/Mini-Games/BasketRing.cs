using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketRing : MonoBehaviour
{
    [SerializeField] private VoidEvent OnScoredEvent;
    private Quaternion RotationAtStart;

    bool isKnocked;
    public bool IsKnocked => isKnocked;
    public float time;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BasketBall"))
            OnScoredEvent.Raise();
    }
}
