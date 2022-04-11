using Unity.XR.CoreUtils;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject playerPrefab;

    private XROrigin playerOrigin;
    public XROrigin PlayerOrigin => playerOrigin;

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
        SpawnPlayer();
    }

    private void SpawnPlayer() {
        var player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        playerOrigin = player.GetComponentInChildren<XROrigin>();
        playerOrigin.transform.position = spawnPoint.position;
    }
}