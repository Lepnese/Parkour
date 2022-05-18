using UnityEngine;

public class AiAgent : MonoBehaviour
{
    [SerializeField] private AiStateId initialState;
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody weapon;
    
    public AiAgentConfig config;
    public SkinnedMeshRenderer mesh;

    public Rigidbody Weapon => weapon;
    public EnemyTarget EnemyTarget { get; private set; }
    public AiAnimation Animation { get; private set; }
    public Ragdoll Ragdoll { get; private set; }
    public AiStateMachine StateMachine { get; private set; }
    public AISensor Sensor { get; private set; }
    public Animator Animator { get; private set; }
    public LineRenderer LineRenderer { get; private set; }
    public AIShooting Shooting { get; private set; }
    public Vector3 TargetPosition => target.position + Vector3.down * 0.3f;
    public Health TargetHealth { get; private set; }

    private void Awake() {
        Animation = GetComponent<AiAnimation>();
        Shooting = GetComponent<AIShooting>();
        LineRenderer = GetComponent<LineRenderer>();
        Animator = GetComponent<Animator>();
        Sensor = GetComponent<AISensor>();
        Ragdoll = GetComponent<Ragdoll>();
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void Start() {
        StateMachine = new AiStateMachine(this);
        StateMachine.RegisterState(new AiIdleState());
        StateMachine.RegisterState(new AiShootState());
        StateMachine.RegisterState(new AiDeathState());
        StateMachine.RegisterState(new AiPauseState());
        StateMachine.ChangeState(initialState);
        
        EnemyTarget = target.GetComponent<EnemyTarget>();
        TargetHealth = EnemyTarget.Health;
        EnemyTarget.FollowTarget(true);
    }

    private void Update() {
        StateMachine.Update();
    }
}