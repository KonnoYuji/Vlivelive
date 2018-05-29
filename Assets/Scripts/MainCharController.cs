using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainCharController : Photon.MonoBehaviour {

    [SerializeField]
    private Animator myAnim;

    [SerializeField]
    private PhotonView myView;

    private Vector3 currentPos;

    private ViveLeftHandController leftHand;

    private ViveRightHandController rightHand;

    private OculusGoController oculusGoController;

    private bool isJumped = false;

    private bool isHandUp = false;

    [SerializeField]
    private GameObject[] offRenderingParts;

    public bool isMyPlayer = false;

    private void Awake()
    {
        if(myView == null)
        {
            myView = GetComponent<PhotonView>();
        }
        
        //VRmodeでのコントローラー入力
#if VRMode

#if UNITY_STANDALONE
        leftHand = FindObjectOfType<ViveLeftHandController>();
        rightHand = FindObjectOfType<ViveRightHandController>();

        if (leftHand != null && rightHand != null)
        {
            leftHand.TouchPadClicked += ChangeJumpState;
            rightHand.TouchPadClicked += ChangeHandUpState;
        }

#elif UNITY_ANDROID
        oculusGoController = FindObjectOfType<OculusGoController>();

        if(oculusGoController != null)
        {
            oculusGoController.ClickedPad += ChangeJumpState;
            oculusGoController.TouchedPad += ChangeHandUpState;
        }        
#endif

#else
        var charUISetting = StandaloneCharUISetting.Instance;
        charUISetting.ListenJumpMethod(ChangeJumpState, true);
        charUISetting.ListenUpHandMethod(ChangeHandUpState, true);

#endif
        PhotonManager.Instance.leaveEvent += DetachInputEvent;
        PhotonManager.Instance.leaveEvent += DestroyMyself;        
    }

    // Use this for initialization
    void Start () {        

        if(myAnim == null)
        {
            myAnim = GetComponent<Animator>();
        }
        
        AudioManager.Instance.PlayCheerSound();
        currentPos = transform.position;

        if(myView.isMine)
        {
            for(int i=0; i<offRenderingParts.Length; i++)
            {
                offRenderingParts[i].SetActive(false);
            }
        }
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

    private void ChangeJumpState()
    {
        if (myView.isMine)
        {
            isJumped = !isJumped;
            Jump(isJumped);
        }
    }

    private void ChangeHandUpState()
    {
        if (myView.isMine)
        {
            isHandUp = !isHandUp;
            UpHand(isHandUp);
        }
    }

    private void Jump(bool state)
    {
        bool? curentState = myAnim.GetBool("Jump");
        if (curentState == null || curentState == state)
        {
            return;
        }
        myAnim.SetBool("Jump", state);
    }

    private void UpHand(bool state)
    {
        bool? currentState = myAnim.GetBool("UpHand");
        if (currentState == null ||  currentState == state)
        {
            return;
        }
        myAnim.SetBool("UpHand", state);
    }

    private void DestroyMyself()
    {
        if(PhotonManager.Instance.leaveEvent != null)
        {
            PhotonManager.Instance.leaveEvent -= DestroyMyself;
        }

        if(myView.isMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    private void DetachInputEvent()
    {
        //VRmodeでのコントローラー入力
#if VRMode

#if UNITY_STANDALONE
        
        if (leftHand != null && rightHand != null)
        {       
            leftHand.TouchPadClicked -= ChangeJumpState;
            rightHand.TouchPadClicked -= ChangeHandUpState;
        }

#elif UNITY_ANDROID
        
        if(oculusGoController != null)
        {
            oculusGoController.ClickedPad -= ChangeJumpState;
            oculusGoController.TouchedPad -= ChangeHandUpState;
        }        
#endif

#else
        var charUISetting = StandaloneCharUISetting.Instance;
        charUISetting.ListenJumpMethod(ChangeJumpState, false);
        charUISetting.ListenUpHandMethod(ChangeHandUpState, false);
#endif
    }
}
