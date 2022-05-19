using UnityEngine;

public class CrossLine : MonoBehaviour
{
    [SerializeField] private CrossLines startOrFinish;
    [SerializeField] private ParticleSystem[] particles;
    [SerializeField] private IntEvent timerEvent;
    
    private bool hasCrossed;
    private void OnTriggerEnter(Collider other) {
        if (!hasCrossed && other.CompareTag(Tags.Player)) {
            

            foreach (var particleSystem in particles) {
                particleSystem.Play();
            }
            
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