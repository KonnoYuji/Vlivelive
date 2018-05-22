using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraRotator : MonoBehaviour {

    // カメラオブジェクトを格納する変数
    private Camera mainCamera;
    // カメラの回転速度を格納する変数
    public Vector2 rotationSpeed = new Vector2(0.1f, 0.1f);

    // マウス座標を格納する変数
    private Vector2 lastMousePosition;
    // カメラの角度を格納する変数（初期値に0,0を代入）
    private Vector2 newAngle = new Vector2(0, 0);


    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // ゲーム実行中の繰り返し処理
    void Update()
    {
#if UNITY_STANDALONE
        // 左クリックした時
        if (Input.GetMouseButtonDown(1))
        {
            // カメラの角度を変数"newAngle"に格納
            newAngle = mainCamera.transform.localEulerAngles;
            // マウス座標を変数"lastMousePosition"に格納
            lastMousePosition = Input.mousePosition;
        }
        // 左ドラッグしている間
        else if (Input.GetMouseButton(1))
        {
            // Y軸の回転：マウスドラッグ方向に視点回転
            // マウスの水平移動値に変数"rotationSpeed"を掛ける
            //（クリック時の座標とマウス座標の現在値の差分値）
            newAngle.y -= (lastMousePosition.x - Input.mousePosition.x) * rotationSpeed.y;
            // X軸の回転：マウスドラッグ方向に視点回転
            // マウスの垂直移動値に変数"rotationSpeed"を掛ける
            //（クリック時の座標とマウス座標の現在値の差分値）
            newAngle.x -= (Input.mousePosition.y - lastMousePosition.y) * rotationSpeed.x;
            // "newAngle"の角度をカメラ角度に格納
            mainCamera.transform.localEulerAngles = newAngle;
            // マウス座標を変数"lastMousePosition"に格納
            lastMousePosition = Input.mousePosition;
        }

#elif UNITY_ANDROID || UNITY_IOS

        float RotateSpeed = 0.1f;
        float UpDownSpeed = 0.01f;
        int touchCount = Input.touches.Count(t => t.phase != TouchPhase.Ended || t.phase != TouchPhase.Canceled);
        if (touchCount == 1)
        {
            Touch t = Input.touches.First();
            switch (t.phase)
            {
                case TouchPhase.Moved:

                    //移動量
                    float xDelta = t.deltaPosition.x * RotateSpeed;
                    float yDelta = t.deltaPosition.y * UpDownSpeed;

                    //左右回転
                    mainCamera.transform.parent.transform.Rotate(0, xDelta, 0, Space.World);
                    //上下移動
                    mainCamera.transform.parent.position += new Vector3(0, -yDelta, 0);

                    break;
            }
        }    
#endif
    }
}
