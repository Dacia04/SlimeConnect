using System;
using System.Collections;
using UnityEngine;

public class Debug123 : MonoBehaviour
{
    uint qsize = 15;  // number of messages to keep
    Queue myLogQueue = new Queue();

    GUIStyle logStyle;
    

    private void Start() {
        Debug.Log("Started up logging");
        DontDestroyOnLoad(gameObject);

        logStyle = new GUIStyle();
        logStyle.fontSize = 50; // Set font size
        logStyle.normal.textColor = Color.black;
        logStyle.wordWrap = true;
    }

    void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        myLogQueue.Enqueue("[" + type + "] : " + logString);
        if (type == LogType.Exception)
            myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();

        // string color = "black";
        // myLogQueue.Enqueue($"<color={color}>[{type}] : {logString}</color>");
        // if (type == LogType.Exception)
        //     myLogQueue.Enqueue($"<color={color}>{stackTrace}</color>");
        
        // while (myLogQueue.Count > qsize)
        //     myLogQueue.Dequeue();
    }

    void OnGUI() {
        GUILayout.BeginArea(new Rect(0,0,Screen.width ,Screen.height));
        GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()),logStyle);
        GUILayout.EndArea();
    }
}
