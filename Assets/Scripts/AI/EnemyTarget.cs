using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    [SerializeField] private Transform target;

    public Transform TargetTransform => target;
    public Health Health { get; private set; }

    private bool followTarget;

    private void Awake() {
        Health = target.GetComponent<Health>();
    }

    private void Update() {
        if (followTarget)
            transform.position = target.position + Vector3.up * 1.2f; // plus haut sinon on vise les pieds
    }

    public void FollowTarget(bool active) => followTarget = active;
}
