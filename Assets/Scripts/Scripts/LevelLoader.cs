using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider slider;
    [SerializeField] private Text progressText;
    
    public void LoadLevel(int sceneIndex)
    {
        var newState = sceneIndex switch {
            2 => GameManager.GameStates.Play,
            _ => GameManager.GameStates.Pause
        };

        GameManager.Instance.ChangeState(newState);
        
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }
    
    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;
            progressText.text = progress * 100f + "%";
            yield return null;
        }
    }

    public void Quit() {
        Application.Quit();
    }
}
