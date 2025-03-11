using System;
using UnityEngine;

public class StickySurface : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		var creationBehaviour = collision.body?.GetComponentInChildren<CreationBehaviour>();
		if (!creationBehaviour) return;
		creationBehaviour.RegisterSticky(this);
	}

	private void OnCollisionExit(Collision collision)
	{
		var creationBehaviour = collision.body?.GetComponentInChildren<CreationBehaviour>();
		if (!creationBehaviour) return;
		creationBehaviour.UnregisterSticky(this);
	}
}