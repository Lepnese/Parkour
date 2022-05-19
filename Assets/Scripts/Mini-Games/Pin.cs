using System.Collections;
using UnityEngine;

public class Pin : MonoBehaviour
{
    [SerializeField] private VoidEvent onKnocked;
    
    private Quaternion startRot;
    private Vector3 startPos;
    private float startTime;
    private float time;

    private void Start() {
        startPos = transform.position;
        startRot = transform.rotation;

        // StartCoroutine(CheckForKnock());
    }
    
    private IEnumerator CheckForKnock() {
        yield return new WaitForSeconds(1f);
        
        // yield return new WaitUntil(() =>
        //     Mathf.Abs(transform.rotation.eulerAngles.x) > 90f || Mathf.Abs(transform.rotation.eulerAngles.z) > 90f);

        // while (!(Mathf.Abs(transform.rotation.eulerAngles.x) > 30f || Mathf.Abs(transform.rotation.eulerAngles.z) > 30f)) {
        //     yield return null;
        // }

        startTime = Time.time;
        
        while (Time.time < startTime + 1 && (Mathf.Abs(transform.rotation.eulerAngles.x) > 30f || Mathf.Abs(transform.rotation.eulerAngles.z) > 30f)) {

            yield return null;
        }
        
        onKnocked.Raise();
    }

    public void Reset() {
        transform.position = startPos;
        transform.rotation = startRot;
        
        // StopAllCoroutines();
        // StartCoroutine(CheckForKnock());
    }
}
