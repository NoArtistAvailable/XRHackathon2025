using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

public class HandController : MonoBehaviour
{
    private XRHandTrackingEvents m_HandTrackingEvents;
    public XRHandShape m_HandShape;
    public XRHandPose m_HandPose;
    public CreationAnchor anchor;

    public float m_MinimumHoldTime = 0.1f;

    private bool m_WasDetected;
    private float m_HoldStartTime;
    private float m_TimeOfLastConditionCheck;
    private bool m_PerformedTriggered;
    float m_GestureDetectionInterval = 0.1f;
    
    public UnityEvent m_GesturePerformed;
    public UnityEvent m_GestureEnded;
    
    private void OnEnable()
    {
        m_HandTrackingEvents = GetComponent<XRHandTrackingEvents>();
        m_HandTrackingEvents?.jointsUpdated.AddListener(OnJointsUpdated);
        
    }

    private void OnDisable()
    {
        m_HandTrackingEvents?.jointsUpdated.RemoveListener(OnJointsUpdated);
    }

    void Opened()
    {
        // VRDebugConsole.Log($"{name} : opened hand");
        m_GesturePerformed?.Invoke();
        if (anchor) {anchor.wantToLetLoose = true;}
    }

    void Closed()
    {
        // VRDebugConsole.Log($"{name} : closed hand");
        m_GestureEnded?.Invoke();
        if (anchor)
        {
            anchor.wantToLetLoose = false;
            anchor.TryGrab();
        }
    }

    void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
    {
        if (!isActiveAndEnabled || Time.timeSinceLevelLoad < m_TimeOfLastConditionCheck + m_GestureDetectionInterval)
            return;

        var detected =
            m_HandTrackingEvents.handIsTracked &&
            m_HandShape != null && m_HandShape.CheckConditions(eventArgs) ||
            m_HandPose != null && m_HandPose.CheckConditions(eventArgs);

        if (!m_WasDetected && detected)
        {
            m_HoldStartTime = Time.timeSinceLevelLoad;
        }
        else if (m_WasDetected && !detected)
        {
            m_PerformedTriggered = false;
            Closed();
        }

        m_WasDetected = detected;

        if (!m_PerformedTriggered && detected)
        {
            var holdTimer = Time.timeSinceLevelLoad - m_HoldStartTime;
            if (holdTimer > m_MinimumHoldTime)
            {
                Opened();
                m_PerformedTriggered = true;
            }
        }

        m_TimeOfLastConditionCheck = Time.timeSinceLevelLoad;
    }
}
