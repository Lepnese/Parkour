using UnityEngine;

public class PlayerSoundWhileSprinting : MonoBehaviour
{

    private PlayerMovement playerMovement; // jsp il sert a quoi tho

    public float player_Speed;
    public float sprint_speed = 10f;
    public float move_speed = 1f;

    private Transform Look_Root; // jsp il sert a quoi tho
   
    private PlayerFootStepsSound player_Footsteps;
    private float sprint_volume = 1f;
    private float walk_volume_Min = 8.2f;
    private float walk_volume_Max = 0.6f;
    private float walk_Step_Distance = 0.4f;
    private float sprint_Step_Distance = 0.25f;
     
    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        Look_Root = transform.GetChild(0);

        player_Footsteps = GetComponentInChildren<PlayerFootStepsSound>();
    } 
    void Start()
    {
        player_Footsteps.volume_Min = walk_volume_Min;
        player_Footsteps.volume_Max = walk_volume_Max;
        player_Footsteps.step_Distance = walk_Step_Distance;
    }
        


    void Update()
    {
        Sprint();
    }

    void Sprint()
    {
        if (player_Speed >= sprint_speed)
        {
            //playerMovement.speed = sprint_speed;

            player_Footsteps.step_Distance = sprint_Step_Distance;
            player_Footsteps.volume_Min = sprint_volume;
            player_Footsteps.volume_Max = sprint_volume;
        }
        else if(player_Speed >= move_speed && player_Speed < sprint_speed)
        {
            player_Footsteps.step_Distance = walk_Step_Distance;
            player_Footsteps.volume_Min = walk_volume_Min;
            player_Footsteps.volume_Max = walk_volume_Max;
        }
    }
}
