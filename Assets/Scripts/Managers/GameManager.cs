using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameStates initialState;
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
        GameState = initialState;
    }

    public void GameStart(int i) {
        ChangeState(i == 0 ? GameStates.Pause : GameStates.Play);
    }

    public void ChangeState(GameStates state) {
        GameState = state;
    }

    public enum GameStates
    {
        Play,
        Pause
    }
}