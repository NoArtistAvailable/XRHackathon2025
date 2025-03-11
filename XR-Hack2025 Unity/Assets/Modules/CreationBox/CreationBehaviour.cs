using System;
using System.Collections.Generic;
using UnityEngine;

public class CreationBehaviour : MonoBehaviour
{
	[NonSerialized]
	public CreationAnchor a, b;

	private bool canScale = true;
	private float fixedScaleFactor;

	public float breakPercent = 0.1f;

	private HashSet<StickySurface> potentialSticky = new HashSet<StickySurface>();

	void Update()
	{
		if (!a || !b) return;
		var delta = a.transform.position - b.transform.position;
		var center = (a.transform.position + b.transform.position) / 2;
		var rot = Quaternion.LookRotation(delta.normalized, Vector3.up);
		var localSize = new Vector3(1, 1, delta.magnitude);
		transform.position = center;
		transform.rotation = rot;
		if(canScale) transform.localScale = localSize;
		else if (Mathf.Abs(delta.magnitude - fixedScaleFactor) / fixedScaleFactor > breakPercent)
		{
			Break();
		}
	}

	public void FixScale()
	{
		canScale = false;
		var delta = a.transform.position - b.transform.position;
		fixedScaleFactor = delta.magnitude;
	}

	public void Break()
	{
		this.enabled = false;
		a.createdObject = null;
		b.createdObject = null;
		if (potentialSticky.Count > 0)
		{
			// sticking
			this.gameObject.AddComponent<StickySurface>();
		}
		else
		{
			var rb = this.GetComponentInChildren<Rigidbody>();
			rb.isKinematic = false;
		}
	}

	public void RegisterSticky(StickySurface surf)
	{
		potentialSticky.Add(surf);
	}

	public void UnregisterSticky(StickySurface surf)
	{
		potentialSticky.Remove(surf);
	}
}