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

	void Start()
	{
		VRDebugConsole.Log("created");
		foreach (var collider in GetComponentsInChildren<Collider>()) collider.isTrigger = true;
	}
	
	void Update()
	{
		if (!a || !b) return;
		var delta = a.transform.position - b.transform.position;
		var center = (a.transform.position + b.transform.position) / 2;
		var rot = Quaternion.LookRotation(delta.normalized, Vector3.up);
		var localSize = Vector3.one * delta.magnitude;
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
		foreach (var collider in GetComponentsInChildren<Collider>()) collider.isTrigger = false;
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

	public void OnTriggerEnter(Collider other)
	{
		var sticky = other.GetComponentInParent<StickySurface>();
		if(sticky) RegisterSticky(sticky);
	}

	public void OnTriggerExit(Collider other)
	{
		var sticky = other.GetComponentInParent<StickySurface>();
		if(sticky) UnregisterSticky(sticky);
	}

	public void RegisterSticky(StickySurface surf)
	{
		VRDebugConsole.Log($"registered sticky");
		potentialSticky.Add(surf);
	}

	public void UnregisterSticky(StickySurface surf)
	{
		VRDebugConsole.Log($"unregistered sticky");
		potentialSticky.Remove(surf);
	}
}