using UnityEngine;

public class GrabbableObjectsHelper : MonoBehaviour
{
    public void ObjectGrabbed(GameObject obj) {
        obj.layer = LayerMask.NameToLayer("Grabbing");
    }

    public void ObjectDropped(GameObject obj) {
        obj.layer = LayerMask.NameToLayer("GrabbableObject");
    }
}
