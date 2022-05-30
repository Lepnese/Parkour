using UnityEngine;

public class HandHitbox : MonoBehaviour
{
    [SerializeField] private Health health;

    public void TakeDamage(float i) {
        health.TakeDamage(i);
    }
}