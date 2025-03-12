using System;
using System.Collections.Generic;
using UnityEngine;

public class StickySurface : MonoBehaviour
{
	public static List<StickySurface> active = new List<StickySurface>();
	private void OnEnable()
	{
		active.Add(this);
	}

	private void OnDisable()
	{
		active.Remove(this);
	}

	// private void OnCollisionEnter(Collision collision)
	// {
	// 	VRDebugConsole.Log($"collision entered on sticky surface {name}");
	// 	var creationBehaviour = collision.body?.GetComponentInChildren<CreationBehaviour>();
	// 	if (!creationBehaviour) return;
	// 	creationBehaviour.RegisterSticky(this);
	// }
	//
	// private void OnCollisionExit(Collision collision)
	// {
	// 	var creationBehaviour = collision.body?.GetComponentInChildren<CreationBehaviour>();
	// 	if (!creationBehaviour) return;
	// 	creationBehaviour.UnregisterSticky(this);
	// }
}