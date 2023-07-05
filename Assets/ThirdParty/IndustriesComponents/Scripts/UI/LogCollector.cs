using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * 
 * LogCollector receives the application logs and put then in queue.
 * An event is generated when a new log occurs
 * 
 **/

public class LogCollector : MonoBehaviour
{
    public int maxLines = 13;
    public struct Log
    {
        public string condition;
        public LogType type;
    }
    public Queue<Log> queue = new Queue<Log>();

    public UnityEvent onNewLogs;

    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string condition, string stackTrace, UnityEngine.LogType type)
    {
        // Delete oldest message
        if (queue.Count >= maxLines) queue.Dequeue();

        queue.Enqueue(new Log { condition = condition, type = type });

        if (onNewLogs != null) onNewLogs.Invoke();
    }

    [ContextMenu("TestLog")]
    public void TestLog()
    {
        Debug.LogError("Test Error");
        Debug.LogWarning("Test Warning");
        Debug.Log("Test");
    }
}

