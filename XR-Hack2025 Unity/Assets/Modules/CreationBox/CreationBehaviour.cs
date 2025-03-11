using System;
using UnityEngine;

public class CreationBehaviour : MonoBehaviour
{
	[NonSerialized]
	public CreationAnchor a, b;

	void Update()
	{
		if (!a || !b) return;
		var delta = a.transform.position - b.transform.position;
		var center = (a.transform.position + b.transform.position) / 2;
		var rot = Quaternion.LookRotation(delta.normalized, Vector3.up);
		var localSize = new Vector3(1, 1, delta.magnitude);
		transform.position = center;
		transform.rotation = rot;
		transform.localScale = localSize;
	}
}