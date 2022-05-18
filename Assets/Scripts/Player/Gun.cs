using System;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float damageAmount = 50f;
    [SerializeField] private float impactForce = 150f;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private GunInteraction gunInteraction;

    private bool rayFromBullet;
    private RaycastHit rayHit;
    private AudioSource audioSource;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnTriggerBtnDown(Hand hand) {
        if (hand != gunInteraction.CurrentHand) return;
        
        if (gunInteraction.AmmoCount > 0)
            Fire();
    }

    private void Fire() {
        audioSource.Play();
        muzzleFlash.Play();
        gunInteraction.ReduceAmmo();
        
        if (!rayFromBullet) return;
        if (rayHit.transform.CompareTag(Tags.Player)) return;

        if (rayHit.rigidbody != null) {
            rayHit.rigidbody.AddForce(-rayHit.normal * impactForce);
        }

        var hitBox = rayHit.collider.GetComponent<Hitbox>();
        if (hitBox) {
            hitBox.OnRaycastHit(damageAmount, bulletSpawn.forward, rayHit.point);
            return;
        }

        Quaternion impactRotation = Quaternion.LookRotation(rayHit.normal);
        GameObject impact = Instantiate(impactEffect, rayHit.point, impactRotation);
        impact.transform.parent = rayHit.transform;
        Destroy(impact, 5);
    }

    private void Update() {
        rayFromBullet = Physics.Raycast(bulletSpawn.position, bulletSpawn.forward, out rayHit);
        CheckForReload();
    }

    private void CheckForReload() {
        if (!rayFromBullet) return;
        if(rayHit.normal != Vector3.up) return;
        
        float dot = Vector3.Dot(bulletSpawn.forward, rayHit.normal);
        if (Mathf.Abs(dot) > 0.9f)
            gunInteraction.Reload();
    }
}
