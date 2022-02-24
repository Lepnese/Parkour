using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance {
        get {
            if (!_instance) {
                _instance = new GameObject().AddComponent<GameManager>();
                // name it for easy recognition
                _instance.name = _instance.GetType().ToString();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    public Action GameStart;
    private int startTime = 5;

    private void Start() {
        StartCoroutine(Timer());
    }

    private IEnumerator Timer() {
        yield return new WaitForSecondsRealtime(startTime);
        GameStart();
    }
}
