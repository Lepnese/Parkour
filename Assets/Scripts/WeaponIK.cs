using UnityEngine;

[System.Serializable]
public class HumanBone
{
    public HumanBodyBones bone;
    public float weight;
}

public class WeaponIK : MonoBehaviour
{
    [SerializeField] private Transform aimTransform;
    [SerializeField] private Vector3 targetOffset;
    
    [SerializeField] private int iterations = 10;
    [Range(0, 1)] [SerializeField] private float weight = 1f;
    
    [SerializeField] private float angleLimit = 90f;
    [SerializeField] private float distanceLimit = 0.5f;

    [SerializeField] private HumanBone[] humanBones;
    private Transform[] boneTransforms;
    private AiAgent agent;
    public Vector3 AimPosition => aimTransform.position;
    public Vector3 AimDirection => aimTransform.forward;

    private void OnDrawGizmos() {
        Gizmos.DrawRay(aimTransform.position, aimTransform.forward * 10f);
    }

    private void Awake() {
        agent = GetComponent<AiAgent>();
    }

    private void Start() {
        boneTransforms = new Transform[humanBones.Length];
        for (int i = 0; i < boneTransforms.Length; i++) {
            boneTransforms[i] = agent.Animator.GetBoneTransform(humanBones[i].bone);
        }
    }

    private Vector3 GetTargetPosition() {
        Vector3 targetDirection = agent.TargetPosition - aimTransform.position;
        Vector3 aimDirection = aimTransform.forward;
        float blendOut = 0;

        float targetAngle = Vector3.Angle(targetDirection, aimDirection);
        if (targetAngle > angleLimit) {
            blendOut += (targetAngle - angleLimit) / 50f;
        }

        float targetDistance = targetDirection.magnitude;
        if (targetDistance < distanceLimit) {
            blendOut += distanceLimit - targetDistance;
        }

        Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, blendOut);
        return aimTransform.position + direction;
    }

    public void UpdateAimDirection() {
    }

    public void UpdateAimDireection() {
        if (aimTransform == null) return;
        if (!agent.Sensor.IsInSight(agent.PlayerTransform.position)) return;
        
        Vector3 targetPosition = GetTargetPosition();
        for (int i = 0; i < iterations; i++) {
            for (int j = 0; j < boneTransforms.Length; j++) {
                Transform bone = boneTransforms[j];
                float boneWeight = humanBones[j].weight * weight;
                AimAtTarget(bone, targetPosition, boneWeight);
            }
        }
    }

    private void AimAtTarget(Transform bone, Vector3 targetPosition, float weight) {
        Vector3 aimDirection = aimTransform.forward;
        Vector3 targetDirection = targetPosition - aimTransform.position;
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
        Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
        bone.rotation = blendedRotation * bone.rotation;
    }
}
