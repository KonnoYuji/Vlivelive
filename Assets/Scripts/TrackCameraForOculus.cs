using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCameraForOculus : MonoBehaviour {

    [SerializeField]
    private Transform Controller;

    [SerializeField]
    private GameObject UI;

    private Vector3 diffVector;

    private void Awake()
    {
        diffVector = transform.position - Controller.position;
    }

    private void Update()
    {
        OVRInput.Update();

        if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
        {
            var active = UI.activeSelf;
            UI.SetActive(!active);
        }

        transform.position = Controller.position + diffVector;
    }
}
