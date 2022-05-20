using System;
using UnityEngine;

[ExecuteInEditMode]
public class Trigger : MonoBehaviour
{
    private void OnTriggerExit(Collider other) {
        if (other.GetComponents<Collider>().Length != 0 && other is BoxCollider) return;
        print(other.name);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
