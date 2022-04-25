using UnityEngine;
public class Gun : MonoBehaviour
{
    [SerializeField] private float damageAmount = 50f;
    [SerializeField] private float impactForce = 150f;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private GunInteraction gunInteraction;

    public void OnTriggerBtnDown(Hand hand) {
        if (hand != gunInteraction.CurrentHand) return;
        
        if (gunInteraction.CurrentAmmo > 0)
            Fire();
    }

    private void Fire() {
        AudioManager.instance.Play("GunShot");
        
        muzzleFlash.Play();
        gunInteraction.ReduceAmmo();
        if(Physics.Raycast(bulletSpawn.position, bulletSpawn.forward, out var hit)) {
            if(hit.rigidbody != null) {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            var enemy = hit.transform.GetComponent<Enemy>();
            if(enemy != null) {
                enemy.TakeDamage(damageAmount);
                return;
            }

            Quaternion impactRotation = Quaternion.LookRotation(hit.normal);
            GameObject impact = Instantiate(impactEffect, hit.point, impactRotation);
            impact.transform.parent = hit.transform;
            Destroy(impact, 5);
        }
    }
}
