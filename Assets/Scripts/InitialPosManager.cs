using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialPosManager : MonoBehaviour {

    static private InitialPosManager _instance;
    static public InitialPosManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<InitialPosManager>();
                return _instance;
            }
            return _instance;
        }

    }

    public enum Platform
    {
        StandAlone, Mobile, VR, OculusGo
    };

    [SerializeField]
    private Platform myPlatform;

    private Dictionary<Platform, Vector3> offsetDicWithCamera;

    private void Awake()
    {
        offsetDicWithCamera = new Dictionary<Platform, Vector3>();
        offsetDicWithCamera.Add(Platform.StandAlone, new Vector3(0, 0, 0));
        offsetDicWithCamera.Add(Platform.Mobile, new Vector3(0, 0, 0));
        offsetDicWithCamera.Add(Platform.VR, new Vector3(1.4f, 0, 2.0f));
        offsetDicWithCamera.Add(Platform.OculusGo, new Vector3(0, 1.0f, 0));
    }

    public Vector3 MyPlatformOffset()
    {
        return offsetDicWithCamera[myPlatform];
    }
}
