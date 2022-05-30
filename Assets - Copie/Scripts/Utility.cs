using System;
using System.Data;
using System.Linq;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static void CreateClimbArea(GameObject climbableObj, float relativeHeight) {
        if (climbableObj.CompareTag(Tags.ClimbableSurface)) return;
        
        Transform climbableTransform = climbableObj.transform;
        
        bool climbAreaExists = (from Transform t in climbableTransform
                                    where t.CompareTag(Tags.ClimbableSurface)
                                    select t).Any();

        if (climbAreaExists) return;

        var ledgeObj = new GameObject {
            name = "Climbable Area",
            tag = Tags.ClimbableSurface,
        };
        
        var col = climbableObj.GetComponent<Collider>();
        if (col.GetType() == typeof(BoxCollider))
            CreateBoxColliderArea(climbableTransform, ledgeObj.transform, relativeHeight);
        else if (col.GetType() == typeof(MeshCollider))
            InstantiateClimbArea(climbableTransform, relativeHeight);
        else
            throw new NotSupportedException(
                $"GameObject {climbableObj} does not contain a BoxCollider or a MeshCollider");
    }

    private static void CreateBoxColliderArea(Transform parentTransform, Transform childTransform, float relativeHeight) {
        childTransform.parent = parentTransform;
        childTransform.localScale = new Vector3(1f, relativeHeight, 1f);

        var parentCollider = parentTransform.GetComponent<BoxCollider>();
        var col = childTransform.gameObject.AddComponent<BoxCollider>();
        col.size = parentCollider.size;
    }
    
    private static void InstantiateClimbArea(Transform climbableTransform, float relativeHeight) {
        var meshObj = climbableTransform.GetComponent<MeshCollider>();
        var renderer = climbableTransform.GetComponent<Renderer>();

        if (!renderer)
            throw new NoNullAllowedException($"GameObject {climbableTransform.name} does not contain a MeshRenderer");
        
        var parentMesh = meshObj ? meshObj.sharedMesh : climbableTransform.GetComponent<MeshFilter>().mesh;
        var parentSize = renderer.bounds.size;
        var parentCenter = renderer.bounds.center;
        
        var parentPos = climbableTransform.localPosition;
        var parentScale = climbableTransform.localScale;
        
        var newScale = parentScale;
        newScale.y = relativeHeight * parentScale.y / parentSize.y;
        
        var newPos = parentPos;
        newPos.y = parentCenter.y + parentSize.y / 2f - relativeHeight / 2f;
        
        var ledgeObj = new GameObject {
            name = "Climbable Area",
            tag = Tags.ClimbableSurface,
            transform = {
                position = newPos,
                localScale = newScale,
                parent = climbableTransform,
            }
        };

        var newCollider = ledgeObj.AddComponent<MeshCollider>();
        newCollider.sharedMesh = parentMesh;
        newCollider.convex = true;
    }

    public static void RemoveClimbArea(Collider ledgeObj) {
        Destroy(ledgeObj.gameObject);
    }
}
