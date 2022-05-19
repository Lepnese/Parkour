using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    
    private float startTime;
    private bool timerActive;

    public void HandleTimer(int i) {
        switch (i) {
            case 0:
                timerActive = false;
                break;
            case 1:
                if (!timerActive)
                    StartCoroutine(StartTimer());
                break;
            default:
                Debug.LogError("Received int other than 0 or 1");
                break;
        }
    }

    private IEnumerator StartTimer() {
        startTime = Time.time;
        timerActive = true;

        while (timerActive) {
            float t = Time.time - startTime;
            int minutes = Mathf.FloorToInt(t / 60F);
            int seconds = Mathf.FloorToInt(t - minutes * 60);
            int ms = Mathf.FloorToInt(t * 100) % 100;
            
            text.text = string.Format("{0:}:{1:00}:{2:00}", minutes, seconds, ms);

            yield return null;
        }
    }
}
