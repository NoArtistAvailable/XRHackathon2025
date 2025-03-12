using System;
using System.Collections.Generic;
using UnityEngine;

public class CreationBehaviour : MonoBehaviour
{
	[NonSerialized]
	public CreationAnchor a, b;

	private bool canScale = true;
	private float fixedScaleFactor;

	public bool autoBreak = false;
	public float breakPercent = 0.1f;

	private HashSet<StickySurface> potentialSticky = new HashSet<StickySurface>();

	void Start()
	{
		VRDebugConsole.Log("created");
		SetToCarry();
	}

	public void SetToCarry()
	{
		foreach (var collider in GetComponentsInChildren<Collider>()) collider.isTrigger = true;
		GetComponent<Rigidbody>().isKinematic = true;
		this.enabled = true;
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
		else if (autoBreak && Mathf.Abs(delta.magnitude - fixedScaleFactor) / fixedScaleFactor > breakPercent)
		{
			Break();
		}
		if(!autoBreak && a.wantToLetLoose && b.wantToLetLoose) Break();
	}

	public void FixScale()
	{
		canScale = false;
		var delta = a.transform.position - b.transform.position;
		fixedScaleFactor = delta.magnitude;
	}

	[ContextMenu("Break")]
	public void Break()
	{
		foreach (var collider in GetComponentsInChildren<Collider>()) collider.isTrigger = false;
		// this.enabled = false;
		if(a) a.createdObject = null;
		if(b) b.createdObject = null;
		a = null;
		b = null;
		if (potentialSticky.Count > 0)
		{
			// sticking
			var sticky = this.gameObject.GetComponent<StickySurface>();
			if(!sticky) this.gameObject.AddComponent<StickySurface>();
			var rb = this.GetComponentInChildren<Rigidbody>();
			rb.isKinematic = true;
		}
		else
		{
			var sticky = this.gameObject.GetComponent<StickySurface>();
			if(sticky) Destroy(sticky);
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

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log($"this happened on {name}");
		var sticky = collision.collider.GetComponentInParent<StickySurface>();
		if (sticky)
		{
			RegisterSticky(sticky);
			Break();
		}
	}

	public void RegisterSticky(StickySurface surf)
	{
		VRDebugConsole.Log($"{name} registered sticky");
		potentialSticky.Add(surf);
	}

	public void UnregisterSticky(StickySurface surf)
	{
		VRDebugConsole.Log($"{name} unregistered sticky");
		potentialSticky.Remove(surf);
	}
}