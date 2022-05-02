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
        print("Dead");
        onEnemyKilled.Raise();
    }
    
    private void Update() {
        if (lastShootTime + shootDelay > Time.time) return;
        
        // Use an object pool instead for these! To keep this tutorial focused, we'll skip implementing one.
        // For more details you can see: https://youtu.be/fsDE_mO4RZM or if using Unity 2021+: https://youtu.be/zyzqA_CPz2E

        // Animator.SetBool("IsShooting", true);
        shootingSystem.Play();
        Vector3 direction = GetDirection();

        if (Physics.Raycast(bulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue)) {
            TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);

            StartCoroutine(SpawnTrail(trail, hit));

            lastShootTime = Time.time;
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
        Instantiate(impactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time);
    }
    
}