using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private void Awake() {
        GameManager.OnGameStateChanged += GameManagerOnStateChanged;
    }

    private void OnDestroy() {
        GameManager.OnGameStateChanged -= GameManagerOnStateChanged;
    }

    private void GameManagerOnStateChanged(GameState state) {
        if (panel)
            panel.SetActive(state == GameState.Test);
    }
}
