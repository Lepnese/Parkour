using System.Collections.Generic;
using UnityEngine;

public class AISensor : MonoBehaviour
{
    [SerializeField] private Transform eyes;
    [SerializeField] private Transform feet;
    [Range(0, 50)] [SerializeField] private float distance = 10f;
    [Range(0, 180)] [SerializeField] private float angle = 30f;
    [Range(0, 15)] [SerializeField] private float height = 1f;
    [SerializeField] private Color meshColor = Color.red;
    [SerializeField] private LayerMask occlusionLayers;
    [SerializeField] private List<GameObject> objects = new List<GameObject>();

    private Mesh mesh;

    public bool IsInSight(Vector3 pos) {
        var origin = transform.position;
        var dest = pos;
        var dir = dest - origin;

        if (dir.y < 0 || dir.y > height) return false;
        if (dir.sqrMagnitude > distance * distance) return false;

        dir.y = 0;
        var deltaAngle = Vector3.Angle(dir, transform.forward);
        if (deltaAngle > angle) return false;

        origin.y += eyes.position.y - feet.position.y;
        Debug.DrawLine(origin, dest, Color.green);
        return !Physics.Linecast(origin, dest, occlusionLayers);
    }
    
    private Mesh CreateWedgeMesh() {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = segments * 4 + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];
        
        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;

        int vert = 0;
        
        // left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;
        
        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;
        
        // right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;
        
        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = angle * 2 / segments;
        for (int i = 0; i < segments; ++i) {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;
            
            topLeft = bottomLeft + Vector3.up * height;
            topRight = bottomRight + Vector3.up * height;
            
            // far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;
        
            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;
        
            // top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;
        
            // bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;
            
            currentAngle += deltaAngle;
        }

        for (int i = 0; i < numVertices; ++i) {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }

    private void OnValidate() {
        mesh = CreateWedgeMesh();
    }

    private void OnDrawGizmos() {
        if (mesh) {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        }

        Gizmos.color = Color.green;
        foreach (var obj in objects) {
            Gizmos.DrawSphere(obj.transform.position, 0.2f);
        }
    }
}
