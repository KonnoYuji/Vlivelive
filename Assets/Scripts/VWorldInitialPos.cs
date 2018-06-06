using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VWorldInitialPos : MonoBehaviour {

    private Dictionary<PlayerStyle, Vector3> offsetDicWithCamera;
    
    [SerializeField]
    private Vector3 initialYAxisOffsetOfCamera = new Vector3(0, 0, 0);

    public enum PlayerStyle{
        Main, Vip, Audience
    }

    private void Awake()
    {
        offsetDicWithCamera = new Dictionary<PlayerStyle, Vector3>();
        offsetDicWithCamera.Add(PlayerStyle.Main, new Vector3(0.35f, -2.38f, 24.014f));
        offsetDicWithCamera.Add(PlayerStyle.Vip, new Vector3(-1.67f, -2.625f, 21.81f));
        offsetDicWithCamera.Add(PlayerStyle.Audience, new Vector3(1.87f, -2.4f, 12.21f));
    }

    public Vector3 MyCharOffset(PlayerStyle myChar)
    {
        return offsetDicWithCamera[myChar] + initialYAxisOffsetOfCamera;
    }
}
