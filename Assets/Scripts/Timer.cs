using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    private float startTime;
    private bool timerStopped;

    private void Awake() {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    public void HandleTimer(int i) {
        switch (i) {
            case 0:
                timerStopped = true;
                break;
            case 1:
                StartCoroutine(StartTimer());
                break;
            default:
                Debug.LogWarning("Received int other than 0 or 1");
                break;
        }
    }

    private IEnumerator StartTimer() {
        startTime = Time.time;
        timerStopped = false;

        while (!timerStopped) {
            float t = Time.time - startTime;
            int minutes = Mathf.FloorToInt(t / 60F);
            int seconds = Mathf.FloorToInt(t - minutes * 60);
            int millilseconds = Mathf.FloorToInt(t * 100) % 100;
            
            timerText.text = string.Format("{0:}:{1:00}:{2:00}", minutes, seconds, millilseconds);
            
            yield return null;
        }
    }
}
