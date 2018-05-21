using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCamera : MonoBehaviour {

    public Vector3 offsetFromCamera;

    public Vector3 offsetFromCameraForVR;

    public Vector3 offsetfromCameraForOcuGo;

    public Vector3 currentOffset;

    private Transform mainCamera;

    private Vector3 currentCameraPos;

    private float currentCameraRotationY;

    private PhotonView myView;

    private void Awake()
    {
        mainCamera = Camera.main.transform;
        currentCameraRotationY = mainCamera.rotation.eulerAngles.y;

#if VRMode
        currentOffset = offsetFromCameraForVR;
#else
        currentOffset = offsetFromCamera;
#endif
        myView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!myView.isMine)
        {
            return;
        }

        if (mainCamera.position != currentCameraPos)
        {
            transform.position = new Vector3(mainCamera.position.x, transform.position.y, mainCamera.position.z);
            currentCameraPos = mainCamera.position;
        }

        if (mainCamera.rotation.eulerAngles.y != currentCameraRotationY)
        {
            currentCameraRotationY = mainCamera.rotation.eulerAngles.y;
            transform.localRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, currentCameraRotationY, transform.rotation.eulerAngles.z);
        }

    }
}
