using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ObjectSize : MonoBehaviour
{
    [SerializeField] private Transform point;
    [SerializeField] private Collider col;

    private Mouse mouse;
    private Vector3 closestPoint;
    private void Start() {
        mouse = Mouse.current;
    }
    private void Update() {
        if (mouse.leftButton.wasPressedThisFrame) {
            closestPoint = col.ClosestPoint(point.position);
            IsGrabbingCorner();
        }
    }

    private void IsGrabbingCorner() {
        var minHeight = col.bounds.center;
        minHeight.x += col.bounds.extents.x;
        minHeight.y += col.bounds.extents.y - 0.1f;
        
        if (closestPoint.y < minHeight.y) return;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(closestPoint, 0.05f);
        
        var dest = col.bounds.center;
        dest.y += col.bounds.extents.y;
        
        Gizmos.DrawLine(col.bounds.center, dest);

        var corner = col.bounds.center;
        corner.x += col.bounds.extents.x;
        corner.y += col.bounds.extents.y - 0.1f;
        Gizmos.DrawSphere(corner, 0.01f);
        Gizmos.DrawLine(col.bounds.center, corner);
    }
}
