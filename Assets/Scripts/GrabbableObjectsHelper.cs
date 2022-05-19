using UnityEngine;

public class GrabbableObjectsHelper : MonoBehaviour
{
    public void ObjectGrabbed(GameObject obj) {
        obj.layer = LayerMask.NameToLayer("Grab");
    }

    public void ObjectDropped(GameObject obj) {
        obj.layer = LayerMask.NameToLayer("Default");
    }
}
