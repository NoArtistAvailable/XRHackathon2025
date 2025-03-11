using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreationBox : MonoBehaviour
{
    public HashSet<CreationAnchor> inside = new HashSet<CreationAnchor>();
    public CreationBehaviour prefab;
    
    
    private void OnTriggerEnter(Collider other)
    {
        var anchor = other.GetComponentInParent<CreationAnchor>();
        if (!anchor) return;
        inside.Add(anchor);
    }

    private void OnTriggerExit(Collider other)
    {
        var anchor = other.GetComponentInParent<CreationAnchor>();
        if (!anchor || anchor.createdObject) return;
        inside.Remove(anchor);
        if (inside.Contains(anchor.paired))
        {
            var clone = Instantiate(prefab);
            clone.a = anchor.paired;
            clone.b = anchor;

            anchor.createdObject = clone;
            anchor.paired.createdObject = clone;
        }
    }
}
