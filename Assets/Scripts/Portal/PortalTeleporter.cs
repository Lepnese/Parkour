using UnityEngine;
using Unity.XR.CoreUtils;

public class PortalTeleporter : MonoBehaviour
{
	[SerializeField] private XROrigin player;
	[SerializeField] private Transform reciever;

	private Transform playerTransform;
	private bool playerIsOverlapping = false;

    private void Awake() {
		playerTransform = player.GetComponent<Transform>();
    }

    private void Update() {
		if (!playerIsOverlapping) return;
		
		Vector3 portalToPlayer = playerTransform.position - transform.position;
		float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

		// If dotProduct < 0: The player has moved across the portal
		if (dotProduct >= 0f) return;
		
		// Teleport him!
		float rotationDiff = -Quaternion.Angle(transform.rotation, reciever.rotation);
		rotationDiff += 180;
		playerTransform.Rotate(Vector3.up, rotationDiff);

		Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
		playerTransform.position = reciever.position + positionOffset;

		playerIsOverlapping = false;
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player"))
			playerIsOverlapping = true;
	}

	private void OnTriggerExit(Collider other) {
		if (other.CompareTag("Player"))
			playerIsOverlapping = false;
	}
}
