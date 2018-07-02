using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VrGrabber;

public class VrgOculusGoDevice : IDevice {

    private OVRInput.Controller GetOVRController(ControllerSide side) 
    {        
        //Oculus Goは右手限定
        return OVRInput.Controller.RTrackedRemote;
    }

    public Vector3 GetLocalPosition(ControllerSide side) 
    {
        return OVRInput.GetLocalControllerPosition((GetOVRController(side)));
    }

    public Quaternion GetLocalRotation(ControllerSide side) 
    {
        return OVRInput.GetLocalControllerRotation(GetOVRController(side));
    }

    public float GetHold(ControllerSide side) 
    {        
        return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, GetOVRController(side));        
    }

    public bool GetTriggerClicked(ControllerSide side)
    {
        return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, GetOVRController(side));
    }

    public bool GetTriggerClicking(ControllerSide side)
    {
        return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, GetOVRController(side));
    }

    public bool GetHover(ControllerSide side) 
    {     
        return OVRInput.Get(OVRInput.Touch.PrimaryTouchpad, GetOVRController(side));
    }

    public bool GetClick(ControllerSide side) 
    {
        return OVRInput.Get(OVRInput.Button.One, GetOVRController(side));
    }

    public Vector2 GetCoord(ControllerSide side) 
    {        
        return OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, GetOVRController(side));
    }

    public bool GetUpFlicked()
    {
        return OVRInput.GetDown(OVRInput.Button.Up);
    }

    public bool GetDownFlicked()
    {
        return OVRInput.GetDown(OVRInput.Button.Down);
    }

    public bool GetLeftFlicked()
    {
        return OVRInput.GetDown(OVRInput.Button.Left);
    }

    public bool GetRightFlicked()
    {
        return OVRInput.GetDown(OVRInput.Button.Right);        
    }
}
