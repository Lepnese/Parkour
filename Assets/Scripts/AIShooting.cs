using System.Collections;
using UnityEngine;

public class AIShooting : MonoBehaviour
{
    [SerializeField] private Transform aimTransform;
    [SerializeField] private float timeBetweenShots = 0.5f;
    [SerializeField] private float aimLockTime = 0.2f;
    [SerializeField] private AudioSource audioSource;
    
    private IEnumerator startFiring;
    
    private AiAgent agent;
    private bool isFiring;
    private float lastShotTime;
    private static readonly int IsShooting = Animator.StringToHash("isShooting");

    private Color aimColor;
    private Color shootColor;

    private void Awake() {
        agent = GetComponent<AiAgent>();
    }

    private void Start() {
        startFiring = Fire();
        
        aimColor = shootColor = agent.LineRenderer.startColor;
        aimColor.a = 0.5f;
        shootColor.a = 1f;

        lastShotTime = Time.time;
    }

    private IEnumerator Fire() {
        lastShotTime = Time.time;
        while (isFiring) {
            agent.Target.FollowTarget(true);
            
            agent.Animator.SetBool(IsShooting, false);
            agent.LineRenderer.startColor = aimColor;
            agent.LineRenderer.endColor = aimColor;

            // follow player while cannot shoot
            while (!CanShoot()) {
                EnableLineRenderer(true);
                yield return null;
            }
            
            // change line color to shootColor, then wait for aimLockTime before shooting
            agent.LineRenderer.startColor = shootColor;
            agent.LineRenderer.endColor = shootColor;

            agent.Target.FollowTarget(false);

            yield return new WaitForSeconds(aimLockTime);
            
            Shoot();

            lastShotTime = Time.time;
            yield return null;
        }
    }

    private void Shoot() {
        audioSource.Play();
        agent.Animator.SetBool(IsShooting, true);
        
        if (Physics.Raycast(aimTransform.position, aimTransform.forward, out var hit)) {
            var health = hit.collider.GetComponent<Health>();
            var hitBox = hit.collider.GetComponent<HandHitbox>();
            
            if (health) {
                health.TakeDamage(agent.config.shootingDamage);
            }
            else if (hitBox) {
                hitBox.TakeDamage(agent.config.shootingDamage);
            }
        }
    }

    private void EnableLineRenderer(bool active) {
        if (active) {
            agent.LineRenderer.positionCount = 2;
            agent.LineRenderer.SetPosition(0, aimTransform.position);
            agent.LineRenderer.SetPosition(1, aimTransform.position + aimTransform.forward * 50f);
        }
        else {
            agent.LineRenderer.positionCount = 0;
        }
    }

    private bool CanShoot() => lastShotTime + timeBetweenShots < Time.time;

    public void SetFiring(bool active) {
        isFiring = active;
        
        if (active)
            StartCoroutine(startFiring);
        else {
            StopCoroutine(startFiring);
            EnableLineRenderer(false);
        }
    }
}