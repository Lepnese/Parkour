using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private VoidEvent onEnemyKilled;
    [SerializeField] private float health;
    [Header("Shooting")]
    [SerializeField] private bool addBulletSpread = true;
    [SerializeField] private Vector3 bulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] private ParticleSystem shootingSystem;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private ParticleSystem impactParticleSystem;
    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private float shootDelay = 0.5f;
    [SerializeField] private Transform player;

    private Animator animator;
    private float lastShootTime;
    
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float amount) {
        health -= amount;
        
        if (health <= 0)
            Die();
    }
    
    private void Die() {
        animator.SetBool("Die", true);
        onEnemyKilled.Raise();
    }
    
    private void Update() {
        if (lastShootTime + shootDelay > Time.time) return;
        
        shootingSystem.Play();
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        Vector3 direction = GetDirection();
        if (Physics.Raycast(bulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue)) {
            animator.SetBool("IsShooting", true);
            TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit));
            lastShootTime = Time.time;
        }
        else {
            animator.SetBool("IsShooting", false);
        }
    }
    
    private Vector3 GetDirection() {
        Vector3 direction = (player.transform.position - transform.position).normalized;

        if (!addBulletSpread) return direction;
        
        direction += new Vector3(
            Random.Range(-bulletSpreadVariance.x, bulletSpreadVariance.x),
            Random.Range(-bulletSpreadVariance.y, bulletSpreadVariance.y),
            Random.Range(-bulletSpreadVariance.z, bulletSpreadVariance.z)
        );

        direction.Normalize();

        return direction;
    }
    
    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit) {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        // animator.SetBool("IsShooting", false);
        trail.transform.position = hit.point;
        var particle = Instantiate(impactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(particle, trail.time);
        Destroy(trail.gameObject, trail.time);
    }
    
}