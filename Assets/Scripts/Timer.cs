using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class Timer : MonoBehaviour
{

    public Text timerText;
    private float startTime;
    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        float t = Time.time - startTime;
        int minutes = Mathf.FloorToInt(t / 60F);
        int seconds = Mathf.FloorToInt(t - minutes * 60);
        int millilseconds = Mathf.FloorToInt(t * 1000) %1000;

        timerText.text = string.Format("{00:00}:{1:00}:{2:000}", minutes, seconds, millilseconds);
    }
}
