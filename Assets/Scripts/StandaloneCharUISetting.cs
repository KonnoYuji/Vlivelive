using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StandaloneCharUISetting : MonoBehaviour {

    static private StandaloneCharUISetting _instance;
    static public StandaloneCharUISetting Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<StandaloneCharUISetting>();                
            }
            return _instance;
        }
    }

    [SerializeField]
    private Button jump;

    [SerializeField]
    private Button UpHand;

	public void ListenJumpMethod(UnityAction action, bool isListened)
    {
        if(isListened)
        {
            jump.onClick.AddListener(action);
        }
        else
        {
            jump.onClick.RemoveListener(action);
        }
    }

    public void ListenUpHandMethod(UnityAction action, bool isListened)
    {
        if(isListened)
        {
            UpHand.onClick.AddListener(action);
        }
        else
        {
            UpHand.onClick.RemoveListener(action);
        }
    }
}
