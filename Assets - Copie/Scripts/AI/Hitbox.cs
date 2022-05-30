using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public AiHealth health;

    public void OnRaycastHit(float damage, Vector3 direction, Vector3 point) {
        health.TakeDamage(damage, direction, point);
    }
}