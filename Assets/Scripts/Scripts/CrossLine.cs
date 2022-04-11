using UnityEngine;

public class CrossLine : MonoBehaviour
{
    [SerializeField] private CrossLines startOrFinish;
    [SerializeField] private IntEvent timerEvent;
    
    private bool hasCrossed;
    private void OnTriggerEnter(Collider other) {
        if (!hasCrossed && other.CompareTag(Tags.Player)) {
            hasCrossed = true;
            timerEvent.Raise((int)startOrFinish);
        }
    }
    
    private enum CrossLines
    {
        FinishLine = 0,
        StartLine = 1
    }
}
