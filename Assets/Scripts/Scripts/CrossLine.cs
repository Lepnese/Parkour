using UnityEngine;

public class CrossLine : MonoBehaviour
{
    public ParticleSystem collison;

    [SerializeField] private CrossLines startOrFinish;
    [SerializeField] private ParticleSystem[] particles;
    [SerializeField] private IntEvent timerEvent;

    private bool hasCrossed;
<<<<<<< HEAD
    private void OnTriggerEnter(Collider other)
    {
        if (!hasCrossed && other.CompareTag(Tags.Player))
        {

            var explosion = collison.emission;

            explosion.enabled = true;
            collison.Play();
=======
    private void OnTriggerEnter(Collider other) {
        if (!hasCrossed && other.CompareTag(Tags.Player)) {
            

            foreach (var particleSystem in particles) {
                particleSystem.Play();
            }
            
>>>>>>> 917b8eb1fa2273b668f64b49f865ebefb34f9854
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