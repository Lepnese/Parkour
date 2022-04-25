using System.Collections;
using UnityEngine;

public class Watch : MonoBehaviour
{
    private PhysicsHands2 attachedHand;
    private MeshRenderer watchMesh;
    private MeshRenderer[] childMeshes;
    private Canvas canvas;
    
    private void Awake() {
        attachedHand = GetComponentInParent<PhysicsHands2>();
        watchMesh = GetComponent<MeshRenderer>();
        childMeshes = GetComponentsInChildren<MeshRenderer>();
        canvas = GetComponentInChildren<Canvas>();
    }

    private void Start() {
        StartCoroutine(WaitUntil(PhysHandState.Gun, false));
    }

    private IEnumerator WaitUntil(PhysHandState targetState, bool activeOnTargetState) {
        while (attachedHand.State != targetState) {
            yield return null;
        }

        ToggleMeshes(activeOnTargetState);

        bool newStateIsHand = attachedHand.State == PhysHandState.Hand;
        StartCoroutine(WaitUntil(newStateIsHand ? PhysHandState.Gun : PhysHandState.Hand, !newStateIsHand));
    }

    private void ToggleMeshes(bool active) {
        watchMesh.enabled = active;
        canvas.enabled = active;

        foreach (var mesh in childMeshes) {
            mesh.enabled = active;
        }
    }
}
