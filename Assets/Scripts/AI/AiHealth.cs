using UnityEngine;

public class AiHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private ParticleSystem bloodParticles;
    private AiAgent agent;
    private float currentHealth;

    private void Awake() {
        agent = GetComponent<AiAgent>();
        
        var rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies) {
            Hitbox hitBox = rb.gameObject.AddComponent<Hitbox>();
            hitBox.health = this;
        }
    }

    private void Start() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount, Vector3 direction, Vector3 point) {
        currentHealth -= amount;
        
        bloodParticles.transform.position = point;
        bloodParticles.Play();
        
        if (IsDead()) {
            Die(direction);
        }
    }

    private void Die(Vector3 direction) {
        AiDeathState deathState = agent.StateMachine.GetState(AiStateId.Death) as AiDeathState;
        deathState.direction = direction;
        agent.StateMachine.ChangeState(AiStateId.Death);
    }

    public bool IsDead() => currentHealth <= 0;
}