using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject playerPrefab;
    
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
        DontDestroyOnLoad(transform.parent);
    }
}