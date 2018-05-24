using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ViveRightHandController : MonoBehaviour {

    public UnityAction TriggerShallowClicked;
    public UnityAction TouchPadClicked;
    private SteamVR_TrackedObject trackObject;

    private void Awake()
    {
        trackObject= GetComponent<SteamVR_TrackedObject>();
        if(trackObject == null)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        var device = SteamVR_Controller.Input((int)trackObject.index);

        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            if(TriggerShallowClicked != null)
            {
                TriggerShallowClicked();
                return;
            }
        }

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if(TouchPadClicked != null)
            {
                TouchPadClicked();
                return;
            }
        }
    }
}
