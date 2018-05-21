using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCameraOnce : MonoBehaviour {

    [SerializeField]
    private Transform Controller;

    private Vector3 diffVector;

    private void Awake()
    {
        diffVector = transform.position - Controller.position;
    }

    private void Update()
    {
        //OVRInput.Update();

        //if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
        //{
        //    transform.position = Camera.main.transform.forward;
        //}

        transform.position = Controller.position + diffVector;
    }

}
