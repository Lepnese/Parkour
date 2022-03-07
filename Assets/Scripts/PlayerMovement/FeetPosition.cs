using UnityEngine;

public class FeetPosition : MonoBehaviour
{
    private Transform camTransform;
    private Transform _transform;

    private void Awake() {
        camTransform = Camera.main.transform;
        _transform = transform;
    }

    private void Update() {
        _transform.position = new Vector3(camTransform.position.x, _transform.position.y, camTransform.position.z);
    }
}
