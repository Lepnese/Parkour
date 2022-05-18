using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTest : MonoBehaviour
{
    private Vector3 minScale;
    public Vector3 maxScale;
    public bool repeatable;
    public float speed = 2f;
    public float duration = 5f;

    public void StartLerp() => StartCoroutine(RepeatLerp(minScale, maxScale, duration));
    
    private IEnumerator Start() {
        minScale = transform.localScale;
        while (repeatable) {
            yield return RepeatLerp(minScale, maxScale, duration);
            yield return RepeatLerp(maxScale, minScale, duration);
        }
    }
    
    private IEnumerator RepeatLerp(Vector3 a, Vector3 b, float time) {
        float i = 0f;
        float rate = speed / time;
        while (i < 1f) {
            i += Time.fixedDeltaTime * rate;
            transform.localScale = Vector3.Lerp(a, b, i);
            yield return null;
        }
    }
}
