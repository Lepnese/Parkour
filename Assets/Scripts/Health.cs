using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Image redSplatter;
    [SerializeField] private Image whiteRadial;
    [SerializeField] private float hurtTimer = 0.1f;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private VoidEvent playerDiedEvent;

    private AudioSource audioSource;
    private float currentHealth;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage) {
        currentHealth -= damage;

        StartCoroutine(HurtFlash());
        UpdateSplatter();
        
        if (IsDead()) {
            Die();
        }
    }

    private void UpdateSplatter() {
        Color splatterAlpha = redSplatter.color;
        splatterAlpha.a = 1 - currentHealth / maxHealth;
        redSplatter.color = splatterAlpha;
    }

    private IEnumerator HurtFlash() {
        whiteRadial.enabled = true;
        audioSource.PlayOneShot(hurtClip);
        yield return new WaitForSeconds(hurtTimer);
        whiteRadial.enabled = false;
    }


    private void Die() {
        playerDiedEvent.Raise();
        currentHealth = maxHealth;
        UpdateSplatter();
    }

    public bool IsDead() => currentHealth <= 0f;
}