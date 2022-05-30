using UnityEngine;

public class PlayerFootStepsSound : MonoBehaviour
{
    private AudioSource footstep_Sound;

    [SerializeField] private AudioClip[] footstep_Clip;
    private CharacterController character_Controller;
    public float step_Distance;
    public float step_Speed;
    public float volume_Min, volume_Max;
    public float accumulated_Distance;

    private void Awake()
    {
        footstep_Sound = GetComponent<AudioSource>();

        character_Controller = GetComponentInParent<CharacterController>();
    }


    private void Update()
    {
        CheckToPlayFootStepSound();
    }

    private void CheckToPlayFootStepSound()
    {
        if (!character_Controller.isGrounded)
            return;

        if(character_Controller.velocity.sqrMagnitude > 0)
        {
            accumulated_Distance += Time.deltaTime;

            if (accumulated_Distance < step_Distance) return;
            
            footstep_Sound.volume = Random.Range(volume_Min, volume_Max);
            footstep_Sound.clip = footstep_Clip[Random.Range(0, footstep_Clip.Length)];
            footstep_Sound.Play();

            accumulated_Distance = 0f;
        }
        else
        {
            accumulated_Distance = 0f;
        }
    }
}
