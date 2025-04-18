using System;
using System.Collections;
using System.Collections.Generic;
using elZach.Common;
using UnityEngine;

public class CreationBox : MonoBehaviour
{
    public HashSet<CreationAnchor> inside = new HashSet<CreationAnchor>();
    private AudioSource audioSource;
    [SerializeField] AudioClip Sfx;
    private bool canPlay = true;
    private CreationBehaviour GetPrefab() => prefabs.GetRandom();

    public bool fixScaleOnExit = false;
    public List<CreationBehaviour> prefabs;

    public event Action onBeforeCreate;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        var anchor = other.GetComponentInParent<CreationAnchor>();
        if (!anchor) return;
        inside.Add(anchor);
    }

    private void OnTriggerExit(Collider other)
    {
        var anchor = other.GetComponentInParent<CreationAnchor>();
        if (!anchor) return;
        inside.Remove(anchor);
        if (inside.Contains(anchor.paired))
        {
            if (anchor.createdObject) return;
            try
            {
                onBeforeCreate?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            var prefab = GetPrefab();
            var clone = Instantiate(prefab);
            clone.name = prefab.name;
            clone.a = anchor.paired;
            clone.b = anchor;

            anchor.createdObject = clone;
            anchor.paired.createdObject = clone;
        } else if (anchor.createdObject && fixScaleOnExit)
        {
            anchor.createdObject.FixScale();
        }
        if (canPlay)
        {
            audioSource.PlayOneShot(Sfx);
            StartCoroutine(AudicoCooldown());
        }
    }

    IEnumerator AudicoCooldown()
    {
        canPlay = false;
        yield return new WaitForSeconds(1f);
        canPlay = true;
    }
}
