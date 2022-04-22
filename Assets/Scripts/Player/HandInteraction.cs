using UnityEngine;

public class HandInteraction : MonoBehaviour
{
    [SerializeField] private InteractorMode interactorStartMode;
    [SerializeField] private VoidEvent onInteractorChange;
    [SerializeField] private GameObject leftHandRay;
    [SerializeField] private GameObject leftHandDirect;
    [SerializeField] private GameObject rightHandRay;
    [SerializeField] private GameObject rightHandDirect;

    private Hand leftRay;
    private Hand leftDirect;
    private Hand rightRay;
    private Hand rightDirect;

    private Hand leftTrackedHand;
    private Hand rightTrackedHand;

    #region Singleton
        private static HandInteraction _instance;
        public static HandInteraction Instance
        {
            get {
                if(_instance == null) {
                    _instance = FindObjectOfType<HandInteraction>();
                }
                return _instance;
            }
        }
    #endregion

    private void Awake() {
        leftRay = leftHandRay.GetComponent<Hand>();
        leftDirect = leftHandDirect.GetComponent<Hand>();
        rightRay = rightHandRay.GetComponent<Hand>();
        rightDirect = rightHandDirect.GetComponent<Hand>();
        
        ToggleHandInteraction(interactorStartMode == InteractorMode.Ray);
    }

    public void ToggleHandInteraction(bool isRay) {
        leftHandRay.SetActive(isRay);
        leftHandDirect.SetActive(!isRay);

        rightHandRay.SetActive(isRay);
        rightHandDirect.SetActive(!isRay);
        
        leftTrackedHand = isRay ? leftRay : leftDirect;
        rightTrackedHand = isRay ? rightRay : rightDirect;

        onInteractorChange.Raise();
    }

    public Hand GetTrackedHand(ControllerSide side)
        => side == ControllerSide.Left ? leftTrackedHand : rightTrackedHand;

    private enum InteractorMode
    {
        Ray,
        Direct
    }
}
