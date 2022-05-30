using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform targetCamera;

    public Transform TargetTransform => targetCamera;
    public Health Health { get; private set; }

    private bool followTarget;

    private void Awake() {
        Health = target.GetComponent<Health>();
    }

    private void Update() {
        if (followTarget)
            transform.position = targetCamera.position - Vector3.up * 0.2f; // plus bas sinon on vise directement dans les yeux
    }

    public void FollowTarget(bool active) => followTarget = active;
}
