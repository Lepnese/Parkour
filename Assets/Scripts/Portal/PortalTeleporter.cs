using Unity.XR.CoreUtils;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
	[SerializeField] private Transform receiver;

	private bool hasEntered;

	private void OnTriggerEnter(Collider other) {
		if (!other.CompareTag("Player") || hasEntered) return;
		
		var origin = other.GetComponent<XROrigin>();
		if (!origin) return;
		
		hasEntered = true;
		origin.transform.position = receiver.position + Vector3.up;
	}
}
