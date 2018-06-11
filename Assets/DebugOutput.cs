using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugOutput : MonoBehaviour {

	public Text message = null;

    private void Awake()
    {
        Application.logMessageReceived  += HandleLog;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived  += HandleLog;
    }

    private void HandleLog( string logText, string stackTrace, LogType type )
    {
        message.text += logText + '\n';
    }
}
