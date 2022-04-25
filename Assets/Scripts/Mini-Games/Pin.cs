using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    [SerializeField] private VoidEvent OnKnockedEvent;
    private Quaternion RotationAtStart;

    bool isKnocked;
    public bool IsKnocked => isKnocked;
    public float time;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        RotationAtStart = transform.rotation;
        while(transform.rotation == RotationAtStart)
        {
            yield return null;
        }
        OnKnockedEvent.Raise();
    }

}
