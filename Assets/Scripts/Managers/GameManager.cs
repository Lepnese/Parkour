using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameStates GameState { get; private set; }
    
    private static GameManager _instance;
    public static GameManager Instance {
        get {
            if(!_instance) {
                GameObject go = new GameObject("GameManager");
                go.AddComponent<GameManager>();
            }
 
            return _instance;
        }
    }
    
    private void Awake() {
        _instance = this;
    }
    
    private void Start() {
        DontDestroyOnLoad(transform);
        GameState = GameStates.Pause;
    }

    public void ChangeState(int i) {
        GameState = i == 0 ? GameStates.Pause : GameStates.Play;
    }

    public enum GameStates
    {
        Play,
        Pause
    }
}