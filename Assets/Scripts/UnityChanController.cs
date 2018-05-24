using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityChanController : Photon.MonoBehaviour {

    private Rigidbody myRb;

    private Animator myAnim;

    private Vector3 currentPos;

    private PhotonView myView;

    private ViveLeftHandController leftHand;

    private ViveRightHandController rightHand;

    private OculusGoController oculusGoController;

    private bool isJumped = false;

    private bool isHandUp = false;

    private void Awake()
    {
        myView = GetComponent<PhotonView>();
        //VRmodeでのコントローラー入力
#if VRMode

#if UNITY_STANDALONE
        leftHand = FindObjectOfType<ViveLeftHandController>();
        rightHand = FindObjectOfType<ViveRightHandController>();

        if (leftHand != null && rightHand != null)
        {
            leftHand.TouchPadClicked += () => {

                if (myView.isMine)isJumped = !isJumped;
                Jump(isJumped);
            };

            rightHand.TouchPadClicked += () => {

                if (myView.isMine) isHandUp = !isHandUp;
                UpHand(isHandUp);    
            };
        }
#elif UNITY_ANDROID
        oculusGoController = FindObjectOfType<OculusGoController>();
        oculusGoController.ClickedPad += () =>{

                if (myView.isMine)isJumped = !isJumped;
                Jump(isJumped);
            };
        oculusGoController.TouchedPad += () =>  {
                if (myView.isMine) isHandUp = !isHandUp;
                UpHand(isHandUp);              
            };
#endif

#else
        var charUISetting = StandaloneCharUISetting.Instance;
        charUISetting.RegisterJumpMethod(() =>
        {
            if (myView.isMine) isJumped = !isJumped;
            Jump(isJumped);
        });

        charUISetting.RegisterUpHandMethod(() =>
        {
            if (myView.isMine) isHandUp = !isHandUp;
            UpHand(isHandUp);
        }
        );
       
#endif
    }

    // Use this for initialization
    void Start () {

        myRb = GetComponent<Rigidbody>();
        myAnim = GetComponent<Animator>();
        myView = GetComponent<PhotonView>();
        AudioManager.Instance.PlayCheerSound();
        currentPos = transform.position;               
	}
	
	// Update is called once per frame
	void Update () {

        if (Mathf.Approximately(Mathf.Floor(currentPos.x * 5), Mathf.Floor(transform.position.x * 5)) && 
            Mathf.Approximately(Mathf.Floor(currentPos.z * 5), Mathf.Floor(transform.position.z * 5)))
        {
            if(!isJumped)
            {
                myAnim.SetTrigger("Idle");                
            }
            return;
        }

        //現在の速度ベクトルの大きさ
        currentPos = transform.position;

        if(!isHandUp)
        {
            myAnim.SetTrigger("Walk");
        }
        
	}

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(isJumped);
            stream.SendNext(isHandUp);
        }
        else
        {
            this.isJumped = (bool)stream.ReceiveNext();
            Jump(isJumped);
            this.isHandUp = (bool)stream.ReceiveNext();
            UpHand(isHandUp);
        }
    }

    private void Jump(bool state)
    {
        if(myAnim.GetBool("Jump") == state)
        {
            return;
        }
        myAnim.SetBool("Jump", state);
    }

    private void UpHand(bool state)
    {
        if (myAnim.GetBool("UpHand") == state)
        {
            return;
        }
        myAnim.SetBool("UpHand", state);
    }
}
