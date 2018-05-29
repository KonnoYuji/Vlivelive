using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OculusController : MonoBehaviour {

#if OCULUSGO
    public UnityAction TouchedPad;
    public UnityAction ClickedPad;
    public UnityAction ClickedTrigger;
#elif OCULUSRIFT
    public UnityAction LeftDpad;
    public UnityAction RightDpad;
#endif

    private void Update()
    {
        //if Input can't work on Oculus go, Please set bellow. But Oculus Rift may not work properly.
        //OVRInput.Update() is always running in OVRManager.
        //OVRInput.Update();

#if OCULUSGO
        if (OVRInput.Get(OVRInput.Button.One))
        {
            if(ClickedPad != null)
            {
                ClickedPad(); 
            }            
            return;
        }
        else if(OVRInput.Get(OVRInput.Touch.One))
        {
            if(TouchedPad != null)
            {
                TouchedPad();
            }            
            return;
        }
        else if(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            if(ClickedTrigger != null)
            { 
                ClickedTrigger();
            }            
            return;
        }
#elif OCULUSRIFT
        if(OVRInput.Get(OVRInput.Button.DpadLeft))
        {
            if(LeftDpad != null)
            {
                LeftDpad();
            }
            return;
        }

        else if(OVRInput.Get(OVRInput.Button.DpadRight))
        {
            if (RightDpad != null)
            {
                RightDpad();
            }
            return;
        }
#endif
    }
}
