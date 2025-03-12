using System;
using UnityEngine;
using UnityEngine.Events;

public class PushButton : MonoBehaviour
{
    public UnityEvent OnPush;
    public float pushedInThreshold = 0.01f;
    private Rigidbody buttonBody;
    private bool pushedIn = false;

    private void OnEnable()
    {
        buttonBody = GetComponentInChildren<SpringJoint>()?.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        bool shouldBePushedIn = buttonBody.transform.localPosition.y <= pushedInThreshold;
        if(!pushedIn && shouldBePushedIn) DoPush();
        pushedIn = shouldBePushedIn;
    }

    [ContextMenu("Do Push")]
    public void DoPush()
    {
        OnPush?.Invoke();
    }
}
