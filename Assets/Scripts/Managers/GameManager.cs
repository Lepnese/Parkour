using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameState State;
    public static event Action<GameState> OnGameStateChanged;
    
    public static GameManager Instance;
    // public static GameManager Instance {
    //     get {
    //         if (!_instance) {
    //             _instance = new GameObject().AddComponent<GameManager>();
    //             // name it for easy recognition
    //             _instance.name = _instance.GetType().ToString();
    //             // mark root as DontDestroyOnLoad();
    //             DontDestroyOnLoad(_instance.gameObject);
    //         }
    //         return _instance;
    //     }
    // }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        UpdateGameState(GameState.Test);
    }

    public void UpdateGameState(GameState newState) {
        State = newState;

        switch (newState) {
            case GameState.Test:
                HandleTest();
                break;
            case GameState.PlayerTurn:
                break;
            case GameState.EnemyTurn:
                break;
            case GameState.Victory:
                break;
            case GameState.Lose:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private void HandleTest() { 
        
    }
}

public enum GameState
{
    Test,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Lose
}
