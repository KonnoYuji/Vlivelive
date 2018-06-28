using UnityEngine;

namespace VrGrabber
{

public static class MatrixExtension
{
    public static Vector3 GetRight(this Matrix4x4 m)
    {
        //GetColumnは列を取得 0~3
        return m.GetColumn(0);
    }

    public static Vector3 GetUp(this Matrix4x4 m)
    {
        //2列目にGrabbableオブジェクトの上方向が入っている
        //Debug.LogFormat("GetUp : {0}, Y : {1}, Z : {0}", m.GetColumn(1).x, m.GetColumn(1).y, m.GetColumn(1).z);
        return m.GetColumn(1);
    }

    public static Vector3 GetForward(this Matrix4x4 m)
    {
        //3列目にGrabbableオブジェクトの前方向が入っている
        //Debug.LogFormat("GetForward : {0}, Y : {1}, Z : {0}", m.GetColumn(2).x, m.GetColumn(2).y, m.GetColumn(2).z);
        return m.GetColumn(2);
    }

    public static Vector3 GetPosition(this Matrix4x4 m)
    {
        //4列目にWorld座標が入っている
        return m.GetColumn(3);
    }

    public static Quaternion GetRotation(this Matrix4x4 m)
    {        
        return Quaternion.LookRotation(m.GetForward(), m.GetUp());
    }

    public static Vector3 GetScale(this Matrix4x4 m)
    {
        return new Vector3(
            m.GetRight().magnitude,
            m.GetUp().magnitude,
            m.GetForward().magnitude);
    }
}
}