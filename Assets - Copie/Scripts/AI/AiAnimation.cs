using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AiAnimation : MonoBehaviour
{
    [SerializeField] private float aimAnimationDuration = 0.3f;
    [SerializeField] private Rig aimLayer;
    [SerializeField] private Rig bodyLayer;

    private bool isAiming;

    public void StartAiming() {
        isAiming = true;
    }

    public void StopAiming() {
        isAiming = false;
    }

    private void Update() {
        switch (isAiming) {
            case true when aimLayer.weight < 1:
                aimLayer.weight += Time.deltaTime / aimAnimationDuration;
                bodyLayer.weight += Time.deltaTime / aimAnimationDuration;
                break;
            case false when aimLayer.weight > 0:
                aimLayer.weight -= Time.deltaTime / aimAnimationDuration;
                bodyLayer.weight -= Time.deltaTime / aimAnimationDuration;
                break;
        }
    }
}