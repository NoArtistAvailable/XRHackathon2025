using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VRDebugConsole : MonoBehaviour
{
    public static void Log(string message) => onMessageLogged?.Invoke(message);
    public static event Action<string> onMessageLogged;
    public TextMeshProUGUI textTemplate;
    public Transform parent;
    public int maxLines = 5;

    private Queue<TextMeshProUGUI> loggedLines = new Queue<TextMeshProUGUI>();

    private void OnEnable()
    {
        onMessageLogged += LogMessage;
    }

    private void OnDisable()
    {
        onMessageLogged -= LogMessage;
    }

    private void LogMessage(string message)
    {
        var clone = Instantiate(textTemplate, parent);
        clone.text = message;
        clone.gameObject.SetActive(true);
        loggedLines.Enqueue(clone);
        while (loggedLines.Count > maxLines)
        {
            var toDestroy = loggedLines.Dequeue();
            Destroy(toDestroy.gameObject);
        }
    }
}
