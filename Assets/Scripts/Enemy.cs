using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health;
    
    public void TakeDamage(float amount) {
        health -= amount;
        
        if (health <= 0)
            Die();
    }
    
    private void Die() {
        print("Dead");    
    }
}