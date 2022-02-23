using UnityEngine;

public class ClosestPoint : MonoBehaviour
{
    public Vector3 location;

    public void OnDrawGizmos()
    {
        var collider = GetComponent<Collider>();

        if (!collider) return;

        Vector3 closestPoint = collider.ClosestPoint(location);

        Gizmos.DrawSphere(location, 0.1f);
        Gizmos.DrawWireSphere(closestPoint, 0.1f);
    }
}
