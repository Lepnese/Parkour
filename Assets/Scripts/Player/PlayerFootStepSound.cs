using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerFootStepSound : MonoBehaviour
{

    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private AudioClip landingClip;
    [SerializeField] private float stepDistance;
    
    private AudioSource audioSource;
    private PlayerController2 playerController;
    private float accumulatedDistance;

    private float speed;
    private Vector3 posLastFrame;
    private bool groundedLastFrame;
    private float yVelLastFrame;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playerController = GetComponentInParent<PlayerController2>();
    }


    private void Update() {
        speed = GetSpeed();

        
        CheckToPlayFootStepSound();
    }

    private void CheckForLanding() {
        if (!groundedLastFrame && playerController.IsGrounded) {
        // if (Mathf.Abs(yVelLastFrame) > 0.01f && Mathf.Abs(playerController.Rigidbody.velocity.y) < 0.01f) {
            audioSource.clip = landingClip;
            audioSource.Play();
        }
    }

    private void CheckToPlayFootStepSound()
    {
        if (!playerController.IsGrounded) {
            accumulatedDistance = 0;
            return;
        }

        accumulatedDistance += Time.deltaTime * speed;
        stepDistance = playerController.Rigidbody.velocity.sqrMagnitude > 0.01f ? 2f : 1f;
        
        if (accumulatedDistance < stepDistance) return;
        
        audioSource.clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.Play();
        accumulatedDistance = 0f;
    }

    private float GetSpeed() {
        return (transform.position - posLastFrame).sqrMagnitude * 200f;
    }

    private void LateUpdate() {
        CheckForLanding();
        
        posLastFrame = transform.position;
        groundedLastFrame = playerController.IsGrounded;
        yVelLastFrame = playerController.Rigidbody.velocity.y;
    }
}
