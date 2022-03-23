using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class PortalTeleporter : MonoBehaviour
{

	public XROrigin player;
	public Transform reciever;

	private Transform playerTransform;
	private bool playerIsOverlapping = false;

    private void Awake()
    {
		playerTransform = player.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
	{
		if (playerIsOverlapping)
		{
			Vector3 portalToPlayer = playerTransform.position - transform.position;
			float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

			// If this is true: The player has moved across the portal
			if (dotProduct < 0f)
			{
				// Teleport him!
				float rotationDiff = -Quaternion.Angle(transform.rotation, reciever.rotation);
				rotationDiff += 180;
				playerTransform.Rotate(Vector3.up, rotationDiff);

				Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
				playerTransform.position = reciever.position + positionOffset;

				playerIsOverlapping = false;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			playerIsOverlapping = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			playerIsOverlapping = false;
		}
	}
}
