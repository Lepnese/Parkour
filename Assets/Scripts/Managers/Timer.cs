using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private float totalTime;

    private void Update() {
        if (GameManager.Instance.GameState == GameManager.GameStates.Play) {
            UpdateTimer();
        }
    }

    private void UpdateTimer() {
        totalTime += Time.deltaTime;
        var minutes = Mathf.FloorToInt(totalTime / 60F);
        var seconds = Mathf.FloorToInt(totalTime - minutes * 60);
            
        text.text = $"{minutes:0}:{seconds:00}";
    }
}
