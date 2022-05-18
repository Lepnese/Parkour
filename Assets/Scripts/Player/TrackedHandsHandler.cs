using System;
using UnityEngine;

public class TrackedHandsHandler : MonoBehaviour
{
    private Hand leftTrackedHand;
    private Hand rightTrackedHand;

    public Hand LeftTrackedHand => leftTrackedHand;
    public Hand RightTrackedHand => rightTrackedHand;
}
