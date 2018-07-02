using UnityEngine;

namespace VrGrabber
{

public enum ControllerSide 
{
    Left,
    Right,
}

public interface IDevice 
{
    Vector3 GetLocalPosition(ControllerSide side);
    Quaternion GetLocalRotation(ControllerSide side);
    float GetHold(ControllerSide side);

    bool GetTriggerClicked(ControllerSide side);

    bool GetTriggerClicking(ControllerSide side);
    
    bool GetUpFlicked();

    
    bool GetDownFlicked();

    bool GetLeftFlicked();

    bool GetRightFlicked();

    bool GetHover(ControllerSide side);
    bool GetClick(ControllerSide side);
    Vector2 GetCoord(ControllerSide side);
}

}
