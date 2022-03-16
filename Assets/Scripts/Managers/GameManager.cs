using System.Collections;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    [SerializeField] private IntEvent timerEvent;

    private IEnumerator Start() {
        while (Time.timeSinceLevelLoad < 5f)
            yield return null;
        
        timerEvent.Raise(1);
        
        while (Time.timeSinceLevelLoad < 10f)
            yield return null;
        
        timerEvent.Raise(0);
    }
}