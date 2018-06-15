#if !UNITY_WSA
using UnityEngine;

namespace VrGrabber
{

public class VrgOculusTouchDevice : IDevice 
{
    private OVRInput.Controller GetOVRController(ControllerSide side) 
    {

#if OCULUS_GO        
        //Oculus Goは右手限定
        return OVRInput.Controller.RTrackedRemote;
#else
        return (side == ControllerSide.Left) ?
            OVRInput.Controller.LTouch :
            OVRInput.Controller.RTouch;
#endif            

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
#if OCULUS_GO        
        return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, GetOVRController(side));
#else
        return OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, GetOVRController(side));
#endif        
    }

    public bool GetHover(ControllerSide side) 
    {
#if OCULUS_GO     
        return OVRInput.Get(OVRInput.Touch.PrimaryTouchpad, GetOVRController(side));
#else
        return OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, GetOVRController(side));
#endif        
    }

    public bool GetClick(ControllerSide side) 
    {
#if OCULUS_GO
        return OVRInput.Get(OVRInput.Button.One, GetOVRController(side));
#else    
        return OVRInput.Get(OVRInput.Button.PrimaryThumbstick, GetOVRController(side));
#endif        
    }

    public Vector2 GetCoord(ControllerSide side) 
    {
#if OCULUS_GO        
        return OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, GetOVRController(side));
#else
        return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, GetOVRController(side));
#endif        
    }
}
}
#endif