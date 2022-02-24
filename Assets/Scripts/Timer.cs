using System;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    private float startTime;
    private bool timerStarted;

    private void Awake() {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        GameManager.Instance.GameStart += StartTimer;
    }

    private void StartTimer() {
        timerStarted = true;
        startTime = Time.time;
    }
    
    void Update() {
        if (!timerStarted) return;

        float t = Time.time - startTime;
        int minutes = Mathf.FloorToInt(t / 60F);
        int seconds = Mathf.FloorToInt(t - minutes * 60);
        int millilseconds = Mathf.FloorToInt(t * 100) % 100;
        
        timerText.text = string.Format("{0:}:{1:00}:{2:00}", minutes, seconds, millilseconds);
    }
}
