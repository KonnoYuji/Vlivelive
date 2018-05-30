using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OculusController : MonoBehaviour {

#if OCULUS_GO
    public UnityAction TouchedPad;
    public UnityAction ClickedPad;
    public UnityAction ClickedTrigger;
#elif OCULUS_RIFT
    public UnityAction LeftDpad;
    public UnityAction RightDpad;
#elif OCULUS_TOUCH
    public UnityAction ThreeClicked;
    public UnityAction FourClicked;
#endif

    private void Update()
    {
        //if Input can't work on Oculus go, Please set bellow. But Oculus Rift may not work properly.
        //OVRInput.Update() is always running in OVRManager.
        //OVRInput.Update();

#if OCULUS_GO
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
#elif OCULUS_RIFT
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
#elif OCULUS_TOUCH
        if(OVRInput.Get(OVRInput.Button.Three))
        {
            if (ThreeClicked != null)
            {
                ThreeClicked();
            }
            return;
        }

        else if(OVRInput.Get(OVRInput.Button.Four))
        {
            if(FourClicked != null)
            {
                FourClicked();
            }
            return;
        }
#endif
    }
}
