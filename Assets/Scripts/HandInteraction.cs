using UnityEngine;

public class HandInteraction : MonoBehaviour
{
    [SerializeField] private GameObject leftHandRay;
    [SerializeField] private GameObject leftHandDirect;
    [SerializeField] private GameObject rightHandRay;
    [SerializeField] private GameObject rightHandDirect;

    public void ToggleHandInteraction(bool isRay) {
        leftHandRay.SetActive(isRay);
        leftHandDirect.SetActive(!isRay);

        rightHandRay.SetActive(isRay);
        rightHandDirect.SetActive(!isRay);
    }
}