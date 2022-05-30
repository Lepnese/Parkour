using UnityEngine;

public class CrossLine : MonoBehaviour
{
    public ParticleSystem collison;

    [SerializeField] private CrossLines startOrFinish;
    [SerializeField] private IntEvent timerEvent;

    private bool hasCrossed;
    private void OnTriggerEnter(Collider other)
    {
        if (!hasCrossed && other.CompareTag(Tags.Player))
        {

            var explosion = collison.emission;

            explosion.enabled = true;
            collison.Play();
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
