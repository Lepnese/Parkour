using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class CharacterControllerHelper : MonoBehaviour
{
    private XROrigin XROrigin;
    private CharacterController characterController;
    private CharacterControllerDriver driver;

    private void Start() {
        XROrigin = GetComponent<XROrigin>();
        characterController = GetComponent<CharacterController>();
        driver = GetComponent<CharacterControllerDriver>();
    }

    private void Update() {
        UpdateCharacterController();
    }

    protected virtual void UpdateCharacterController()
    {
        if (XROrigin == null || characterController == null)
            return;

        var height = Mathf.Clamp(XROrigin.CameraInOriginSpaceHeight, driver.minHeight, driver.maxHeight);

        Vector3 center = XROrigin.CameraInOriginSpacePos;
        center.y = height / 2f + characterController.skinWidth;

        characterController.height = height;
        characterController.center = center;
    }
}
