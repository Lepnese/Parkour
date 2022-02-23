using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LedgeInteractable : XRBaseInteractable
{
    private Collider col;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (args.interactorObject is XRDirectInteractor) {
            col = GetComponent<Collider>();
            Climber.climbingHand = args.interactorObject.transform.GetComponent<Hand>();
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        
        if (!(args.interactorObject is XRDirectInteractor)) return;
        if (Climber.climbingHand && Climber.climbingHand.name == args.interactorObject.transform.name) {
            Climber.climbingHand = null;
        }
    }
}
