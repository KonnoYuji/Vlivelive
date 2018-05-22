using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityChanController : MonoBehaviour {

    private Rigidbody myRb;

    private Animator myAnim;

    private Vector3 currentPos;

	// Use this for initialization
	void Start () {

        myRb = GetComponent<Rigidbody>();
        myAnim = GetComponent<Animator>();
        currentPos = transform.position;
        AudioManager.Instance.PlayCheerSound();
        //currentMagnitude = myRb.velocity.magnitude;
	}
	
	// Update is called once per frame
	void Update () {


        if (Mathf.Approximately(Mathf.Floor(currentPos.x * 5), Mathf.Floor(transform.position.x * 5)) && 
            Mathf.Approximately(Mathf.Floor(currentPos.z * 5), Mathf.Floor(transform.position.z * 5)))
        {
            myAnim.SetTrigger("Idle");
            return;
        }

        //現在の速度ベクトルの大きさ
        currentPos = transform.position;
        myAnim.SetTrigger("Walk");
	}
}
