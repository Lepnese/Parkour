public class AiShootState : AiState
{
    public AiStateId GetId() {
        return AiStateId.Shoot;
    }

    public void Enter(AiAgent agent) {
        agent.Ragdoll.DeactivateRagdoll();
        agent.Animation.StartAiming();
        UpdateFiring(agent);
    }

    public void Update(AiAgent agent) {
        if (!agent.Sensor.IsInSight(agent.TargetPosition) || agent.TargetHealth.IsDead()) {
            agent.StateMachine.ChangeState(AiStateId.Idle);
        }
    }

    private static void UpdateFiring(AiAgent agent) {
        bool isInSight = agent.Sensor.IsInSight(agent.TargetPosition);
        agent.Shooting.SetFiring(isInSight);
    }

    public void Exit(AiAgent agent) {
        agent.Animation.StopAiming();
        agent.Shooting.SetFiring(false);
    }
}