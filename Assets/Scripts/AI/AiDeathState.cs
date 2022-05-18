using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AiDeathState : AiState
{
    public Vector3 direction;
    private static readonly int isDeadId = Animator.StringToHash("isDead");

    public AiStateId GetId() {
        return AiStateId.Death;
    }

    public void Enter(AiAgent agent) {
        agent.GetComponent<RigBuilder>().enabled = false;
        agent.Weapon.isKinematic = false;
        agent.Animator.SetBool(isDeadId, true);
        agent.Ragdoll.ActivateRagdoll();
        direction.y = 1;
        agent.Ragdoll.ApplyForce(direction * agent.config.dieForce);
        agent.mesh.updateWhenOffscreen = true;
    }

    public void Update(AiAgent agent) { }

    public void Exit(AiAgent agent) { }
}