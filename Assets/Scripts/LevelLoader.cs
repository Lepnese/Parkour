using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    //    public static LevelLoader Instance;

    //[SerializeField] private GameObject loadercanvas;
    //[SerializeField] private Image progressBar;
    //void Awake()
    //{
    //    if (Instance = null)
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else
    //        Destroy(gameObject);
    //}

    //public async void LoadScene(string sceneName)
    //{
    //    var scene = SceneManager.LoadSceneAsync(sceneName);
    //    scene.allowSceneActivation = false;

    //    loadercanvas.SetActive(true);

    //    do
    //    {
    //        await Task.Delay(100);
    //        progressBar.fillAmount = scene.progress;
    //    }
    //    while (scene.progress < 0.9f);

    //    scene.allowSceneActivation = false;
    //}
    public GameObject loadingScreen;
    public Slider slider;
    public Text progressText;
    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }
    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            //Debug.Log(progress);

            slider.value = progress;
            progressText.text = progress * 100f + "%";
            //allo
            yield return null;

        }
    }
}
