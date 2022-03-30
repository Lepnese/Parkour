using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandInteraction : MonoBehaviour
{

    private XRInteractorLineVisual lineVisual;

    private void Awake()
    {
        lineVisual = GetComponent<XRInteractorLineVisual>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Tags.InteractableArea))
        {
            var direct = GetComponent<XRDirectInteractor>();
            if(direct)
            {
                Destroy(direct);
            }
            if (!GetComponent<XRRayInteractor>())
            {
                gameObject.AddComponent<XRRayInteractor>();
            }

            lineVisual.enabled = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.InteractableArea))
        {
            lineVisual.enabled = false;
            var ray = GetComponent<XRRayInteractor>();
            if (!ray)
            {
                Destroy(ray);
            }
            if (!GetComponent<XRDirectInteractor>())
            {
                gameObject.AddComponent<XRDirectInteractor>();
            }
        }
    }
}
