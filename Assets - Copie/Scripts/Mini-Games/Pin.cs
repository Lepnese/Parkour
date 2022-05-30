using UnityEngine;

public class Pin : MonoBehaviour
{
    private Quaternion startRot;
    private Vector3 startPos;

    private void Start() {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    public void Reset() {
        transform.position = startPos;
        transform.rotation = startRot;
    }
}
