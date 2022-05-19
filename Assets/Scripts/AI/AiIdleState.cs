public class AiIdleState : AiState
{
    public AiStateId GetId() {
        return AiStateId.Idle;
    }

    public void Enter(AiAgent agent) {
        agent.Ragdoll.DeactivateRagdoll();
    }

    public void Update(AiAgent agent) {
        if (agent.Sensor.IsInSight(agent.PlayerTransform.position)) {
            agent.StateMachine.ChangeState(AiStateId.Shoot);
        }
    }

    public void Exit(AiAgent agent) {
    }
}