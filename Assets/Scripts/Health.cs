using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;

    private void Start() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage) {
        currentHealth -= damage;
        if (IsDead()) {
            Die();
        }
    }

    private void Die() {
        print("Dead");
    }

    public bool IsDead() => currentHealth <= 0f;
}