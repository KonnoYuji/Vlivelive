using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;
using System.Collections;

namespace VrGrabber
{

[RequireComponent(typeof(Rigidbody))]
public class VrgGrabber : MonoBehaviour
{
    const float grabBeginThreshold = 0.55f;
    const float grabEndThreshold = 0.35f;
    const float minGrabSmoothDist = 0.5f;
    const float maxGrabSmoothDist = 2f;
    const float minGrabSmoothFilter = 0.15f;

    public ControllerSide side = ControllerSide.Left;

    public bool isLeft
    {
        get { return side == ControllerSide.Left; }
    }

    public bool isRight
    {
        get { return side == ControllerSide.Right; }
    }

    [SerializeField]
    Transform grip = null;


    [SerializeField]
    float maxGrabDistance = 10f;

    [SerializeField]
    float stickMoveSpeed = 0.1f;

    [SerializeField]
    LayerMask layerMask = ~0;

    public class TargetClickEvent : UnityEvent<VrgGrabber, RaycastHit> {}
    public TargetClickEvent onTargetClicked = new TargetClickEvent();

    public Action<RaycastHit> updateTouchHitEvent;
    public Action updateTouchUnHitEvent;

    internal class AverageVelocity
    {
        private const int n = 3;
        private Vector3[] velocities_ = new Vector3[n];
        private int index_ = 0;

        public Vector3 average
        {
            get
            {
                var a = Vector3.zero;
                for (int i = 0; i < n; ++i)
                {
                    a += velocities_[i];
                }
                return a / n;
            }
        }

        public void Add(Vector3 velocity)
        {
            velocities_[index_] = velocity;
            index_ = (index_ + 1) % n;
        }
    }

    internal class GrabInfo
    {
        public int id = -1;
        public VrgGrabber grabber = null;
        public VrgGrabbable grabbable = null;
        public Matrix4x4 initGripToGrabbableMat;
        public Matrix4x4 initGrabbableToGrabMat; //initGrabbableToGrabMat = grabbable.transform.worldToLocalMatrix * grabMat;
        public float distance = 0f;
        public AverageVelocity velocity = new AverageVelocity();
        public bool isKinematic = false;
        public float smoothFilter = 0f;
        public float stickMove = 0f;

        //Matrixの値が変わり続けるのはこいつ
        public Matrix4x4 grabMat
        {
            get
            {
                //ある空間座標にいるベクトルの平行移動行列を求める ここではxy平面上で、z軸に伸びるベクトルの平行移動行列を求めている. おいおいこの座標はGrabberのモデル変換座標によりワールド空間に変換されるため、grabberから見て平行移動する変換座標になる
                var transMat = Matrix4x4.Translate(new Vector3(0, 0, distance));

                //変換行列をまとめる モデル変換行列×平行移動行列(Grabber基準)
                return grabber.gripTransform.localToWorldMatrix * transMat;
            }
        }

        public Matrix4x4 gripToGrabbableMat
        {
            get
            {
                //grabMat : モデル変換行列×平行移動行列(Grabber基準)
                //initGripToGrabbableMat = grabMat.inverse * grabbable.transform.localToWorldMatrix = 逆行列(モデル変換行列×平行移動行列) × Grabbableのモデル変換行列
                //grabMat * initGripToGrabbableMat = モデル変換行列×平行移動行列 × 逆行列(モデル変換行列×平行移動行列) × Grabbableのモデル変換行列 = Grabbableのモデル変換行列                
                return grabMat * initGripToGrabbableMat;                
            }
        }
    }
    GrabInfo grabInfo_ = new GrabInfo();

    internal class DualGrabInfo
    {
        public Vector3 primaryToSecondary;
        public Vector3 pos;
        public Vector3 center;
        public Vector3 scale;
        public Quaternion rot;
    }
    DualGrabInfo dualGrabInfo_ = new DualGrabInfo();

    internal class CandidateInfo
    {
        public VrgGrabbable grabbable;
        public Collider collider;
        public int refCount = 0;
    }
    Dictionary<Collider, CandidateInfo> directGrabCandidates_ = new Dictionary<Collider, CandidateInfo>();

    RaycastHit targetHit_;

#if OCULUS_TOUCH    
    float holdInput_ = 0f;
    bool isHoldStart_ = false;
    bool isHoleEnd_ = false;
#elif OCULUS_GO || (UNITY_EDITOR && UNITY_STANDALONE)
    bool isHolding = false;
    bool isMovingForward = false;
    bool isMovingBack = false;

#endif    
    Vector3 preRayDirection_;

    public Transform gripTransform
    {
        get { return grip ? grip : transform; }
    }

    public Vector3 gripDir
    {
        get { return gripTransform.forward; }
    }

    public Vector3 targetPos
    {
        get 
        { 
            if (isGrabbing)
            {
                var grabMat = grabInfo_.grabbable.transform.localToWorldMatrix;
                //initGrabbableToGrabMat = grabbable.transform.worldToLocalMatrix * grabMat;
                //grabMat * grabInfo_.initGrabbableToGrabMat = grabbable.transform.localToWorldMatrix x grabbable.transform.worldToLocalMatrix * grabber.gripTransform.localToWorldMatrix * transMat;
                //Grabbableのモデル変換行列 × Grabbableの逆モデル変換行列 × Grabberのモデル変換行列 * Grabberの平行移動行列 = Grabberのモデル変換行列 * Grabberの平行移動行列  
                //同次座標系では、4列目に空間の位置を示すベクトル成分が入っている. Grabberのモデル変換行列もあるので、ここでは、Grabberから見た平行移動になる
                return (grabMat * grabInfo_.initGrabbableToGrabMat).GetPosition();
            }
            else if (targetHit_.transform)
            {
                return targetHit_.point;
            }
            else if (directGrabCandidates_.Count > 0)
            {
                return gripTransform.position;
            }
            else 
            {
                return gripTransform.position + preRayDirection_ * maxGrabDistance;
            }
        }
    }

    public bool isGrabbing
    {
        get { return grabInfo_.grabbable != null; }
    }

    VrgGrabber opposite
    {
        get
        {
            if (!isGrabbing)
            {
                return null;
            }
            return grabInfo_.grabbable.grabbers.Find(grabber => grabber != this);
        }
    }

    public bool isPrimary
    {
        get
        {
            if (!isGrabbing) return false;

            if (!grabInfo_.grabbable.isMultiGrabbed) return true;

            return grabInfo_.id < opposite.grabInfo_.id;
        }
    }

    void Update()
    {
        UpdateInput();
        UpdateGrab();
    }

    void LateUpdate()
    {
        UpdateTransform();

        if (!isGrabbing)
        {
            UpdateTouch();
        }
    }

    void FixedUpdate()
    {
        if (isGrabbing)
        {
            FixedUpdateGrabbingObject();
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        var grabbable =
            collider.GetComponent<VrgGrabbable>() ??
            collider.GetComponentInParent<VrgGrabbable>();
        if (!grabbable) return;

        CandidateInfo info;
        if (!directGrabCandidates_.TryGetValue(collider, out info))
        {
            info = new CandidateInfo();
            info.collider = collider;
            info.grabbable = grabbable;
            directGrabCandidates_.Add(collider, info);
        }
        info.refCount++;
    }

    void OnTriggerExit(Collider collider)
    {
        CandidateInfo info = null;
        if (!directGrabCandidates_.TryGetValue(collider, out info)) return;

        info.refCount--;
        if (info.refCount <= 0)
        {
            directGrabCandidates_.Remove(collider);
        }
    }

    void UpdateTransform()
    {
#if UNITY_EDITOR && UNITY_STANDALONE

#else        
        transform.localPosition = Device.instance.GetLocalPosition(side);
        transform.localRotation = Device.instance.GetLocalRotation(side);
#endif        
    }

    void UpdateInput()
    {
#if OCULUS_TOUCH                         
        var preHoldInput = holdInput_;
        holdInput_ = Device.instance.GetHold(side);
        isHoldStart_ = (holdInput_ >= grabBeginThreshold) && (preHoldInput < grabBeginThreshold);
        isHoleEnd_ = (holdInput_ <= grabEndThreshold) && (preHoldInput > grabEndThreshold);        
#elif OCULUS_GO
        var triggerClicked = Device.instance.GetTriggerClicked(side);
        if(triggerClicked)
        {
            isHolding = !isHolding;
        }        
#elif UNITY_EDITOR && UNITY_STANDALONE
        var triggerClicked = Input.GetMouseButton(0);
        if(triggerClicked)
        {
            isHolding = !isHolding;
        }
#endif                
    }

    void UpdateGrab()
    {
#if OCULUS_TOUCH        
        if (isHoldStart_)
        {
            //DirectGrab();
            RemoteGrab();
        }
        else if (isHoleEnd_)
        {
            Release();
        }
#elif OCULUS_GO || (UNITY_EDITOR && UNITY_STANDALONE)
        if(isHolding)
        {
            RemoteGrab();
        }
        else
        {
            Release();
        }
#endif       
    }

    void UpdateTouch()
    {        
#if UNITY_EDITOR && UNITY_STANDALONE
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hitinfo;
		bool hit = Physics.Raycast(ray, out hitinfo);
        var forward = this.transform.forward;        
#else
        var forward = gripTransform.forward;
        
        var ray = new Ray();
        ray.origin = gripTransform.position;
        ray.direction = Vector3.Lerp(preRayDirection_, forward, 0.25f);

        targetHit_ = new RaycastHit();
        bool hit = Physics.Raycast(ray, out targetHit_, maxGrabDistance, layerMask);

        if(hit && updateTouchHitEvent != null)
        {
            updateTouchHitEvent(targetHit_);
        }
        else if(updateTouchUnHitEvent != null) 
        {
            updateTouchUnHitEvent();
        }
#endif        
        preRayDirection_ = hit ? ray.direction : forward;
    }

    void Grab(VrgGrabbable grabbable, float distance)
    {
        //初回、もしくはGrabbableをRelease時に初期化され、再度Grabbableにレイキャスがあたったときに呼ばれる        
        grabInfo_.grabber = this;
        grabInfo_.grabbable = grabbable;
        grabInfo_.distance = distance;
        var grabMat = grabInfo_.grabMat;
        grabInfo_.initGripToGrabbableMat = grabMat.inverse * grabbable.transform.localToWorldMatrix;
        grabInfo_.initGrabbableToGrabMat = grabbable.transform.worldToLocalMatrix * grabMat;
        grabInfo_.isKinematic = grabbable.rigidbody.isKinematic;

        if (!grabbable.avoidIntersection)
        {
            grabbable.rigidbody.isKinematic = true;
        }

        // if (grabbable.isGrabbed)
        // {
        //     SecondGrab(grabbable);
        // }

        grabInfo_.id = grabbable.OnGrabbed(this);
    }

    void DirectGrab()
    {
        if (isGrabbing || directGrabCandidates_.Count == 0) return;

        VrgGrabbable grabbable = null;
        float minDist = float.MaxValue;

        var gripPos = gripTransform.position;
        foreach (var kv in directGrabCandidates_)
        {
            var candidate = kv.Value;
            var pos = candidate.collider.ClosestPoint(gripPos);
            var dist = Vector3.Distance(gripPos, pos);

            if (dist < minDist)
            {
                grabbable = candidate.grabbable;
                minDist = dist;
            }
        }

        if (grabbable)
        {
            Grab(grabbable, 0f);
        }
    }

    void RemoteGrab()
    {
        //VrGrabbableがアタッチされたスクリプトのみ対応
        if (isGrabbing) return;

#if UNITY_EDITOR && UNITY_STANDALONE
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;
		bool hitted = Physics.Raycast(ray, out hit);
        if(!hitted)
        {
            return;
        }
#else
        var ray = new Ray();
        ray.origin = gripTransform.position;
        ray.direction = gripTransform.forward;        
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, maxGrabDistance, layerMask))
        {
            return;
        }
#endif
        var grabbable =
            hit.collider.GetComponent<VrgGrabbable>() ??
            hit.collider.GetComponentInParent<VrgGrabbable>();

        if (grabbable)
        {
            Grab(grabbable, hit.distance);
        }
    }


    //grabInfo_をここで初期化する
    void Release()
    {
        if (!isGrabbing) return;

        var grabbable = grabInfo_.grabbable;

        Assert.IsTrue(grabbable.isGrabbed);

        grabbable.velocity = grabInfo_.velocity.average;
        grabbable.OnReleased(this);

        if (grabbable.isGrabbed)
        {
            // opposite.ReGrab();
        }
        else
        {
            grabbable.rigidbody.isKinematic = grabInfo_.isKinematic;
        }

        grabInfo_ = new GrabInfo();
    }

    void FixedUpdateGrabbingObject()
    {
        //Grabbableに登録されているイベントを、grabbingしているときにタッチパネルがクリックされたらコールする
#if UNITY_EDITOR && UNITY_STANDALONE
        if(Input.GetMouseButtonDown(1))
        {
            grabInfo_.grabbable.OnGrabClicked(this);
        }        
#else        
        if (Device.instance.GetClick(side))
        {
            grabInfo_.grabbable.OnGrabClicked(this);
        }
        // if (grabInfo_.grabbable.isMultiGrabbed)
        // {
        //     FixedUpdateGrabbingObjectByDualHand();
        // }
#endif
        else
        {
            FixedUpdateGrabbingObjectBySingleHand();        
            //MoveWithFlickInput();       
        }
    }

    //タッチパネル上下押下中にものが移動
    void FixedUpdateGrabbingObjectBySingleHand()
    {            
#if OCULUS_GO 
        //掴んだObjectの参照
        var grabbable = grabInfo_.grabbable;

        float stickY = 0.0f;

        if(Device.instance.GetUpFlicked())
        {
            if(!isMovingForward && !isMovingBack)
            {
                isMovingForward = true;
                StartCoroutine(MoveFoward());    
            }
        }                

        if(isMovingForward)
        {
           stickY = 1.0f;     
           //Debug.Log("stickY is 1.0f");
        }
        else if(!isMovingBack)
        {
            stickY = 0.0f;
        }

        if(Device.instance.GetDownFlicked())
        {
            if(!isMovingForward && !isMovingBack)
            {
                isMovingBack = true;
                StartCoroutine(MoveBack());    
            }
        }

        if(isMovingBack)
        {
           stickY = -1.0f;     
           //Debug.Log("stickY is 1.0f");
        }
        else if(!isMovingForward)
        {
            stickY = 0.0f;
        }

        //StickMoveSpeedは0.1がデフォルト, ここで正負の移動を決める
        var stickMove = stickY * stickMoveSpeed;
        
        //Y軸ギリギリにタッチしたときでも動くようにする
        var stickMoveFilter = stickY > Mathf.Epsilon ? 0.1f : 0.3f;

        //Yの入力位置に対して、移動方向を変更し続ける
        grabInfo_.stickMove += (stickMove - grabInfo_.stickMove) * stickMoveFilter;

        //移動距離の算出 ここで、grabしたobjectを前に進ませる、後ろに進ませる距離を算出
        var dist = Mathf.Clamp(grabInfo_.distance + grabInfo_.stickMove, 0f, maxGrabDistance);
        
        //コントローラーとGrabオブジェクト間の距離
        var actualDist = (targetPos - gripTransform.position).magnitude;
        
        //移動させる距離の比が明らかに大きい場合
        var deltaDist = dist - actualDist;
        var threshDist = Mathf.Max(dist * 0.1f, 0.1f);
        if (Mathf.Abs(deltaDist) > threshDist)
        {
            dist = Mathf.Lerp(grabInfo_.distance, actualDist, 0.05f);
        }


        grabInfo_.distance = dist;

        //Grabしているオブジェクトに座標変換するようの行列(Grabbableのモデル変換行列)      
        var mat = grabInfo_.gripToGrabbableMat;

        var pos = mat.GetPosition();
        var rot = mat.GetRotation();
       
        FixedUpdateGrabbingObjectTransform(pos, rot, grabbable.transform.localScale);

#elif UNITY_EDITOR && UNITY_STANDALONE

        //掴んだObjectの参照
        var grabbable = grabInfo_.grabbable;

        float stickY = 0.0f;

        if(Input.GetKeyDown(KeyCode.W))
		{
            //Debug.Log("W pressed");
			if(!isMovingForward && !isMovingBack)
            {
                isMovingForward = true;
                StartCoroutine(MoveFoward());    
            }
		}

        if(isMovingForward)
        {
           stickY = 1.0f;     
           //Debug.Log("stickY is 1.0f");
        }
        else if(!isMovingBack)
        {
            stickY = 0.0f;
        }

        if(Input.GetKeyDown(KeyCode.S))
		{
            //Debug.Log("W pressed");
			if(!isMovingForward && !isMovingBack)
            {
                isMovingBack = true;
                StartCoroutine(MoveBack());    
            }
		}

        if(isMovingBack)
        {
           stickY = -1.0f;     
           //Debug.Log("stickY is 1.0f");
        }
        else if(!isMovingForward)
        {
            stickY = 0.0f;
        }

        //StickMoveSpeedは0.1がデフォルト, ここで正負の移動を決める
        var stickMove = stickY * stickMoveSpeed;
        
        //Y軸ギリギリにタッチしたときでも動くようにする
        var stickMoveFilter = stickY > Mathf.Epsilon ? 0.1f : 0.3f;

        //Yの入力位置に対して、移動方向を変更し続ける
        grabInfo_.stickMove += (stickMove - grabInfo_.stickMove) * stickMoveFilter;

        //移動距離の算出 ここで、grabしたobjectを前に進ませる、後ろに進ませる距離を算出
        var dist = Mathf.Clamp(grabInfo_.distance + grabInfo_.stickMove, 0f, maxGrabDistance);
        
        //コントローラーとGrabオブジェクト間の距離
        var actualDist = (targetPos - gripTransform.position).magnitude;
        
        //移動させる距離の比が明らかに大きい場合
        var deltaDist = dist - actualDist;
        var threshDist = Mathf.Max(dist * 0.1f, 0.1f);
        if (Mathf.Abs(deltaDist) > threshDist)
        {
            dist = Mathf.Lerp(grabInfo_.distance, actualDist, 0.05f);
        }

        grabInfo_.distance = dist;

        //Grabしているオブジェクトに座標変換するようの行列
        var mat = grabInfo_.gripToGrabbableMat;
        Quaternion rot = mat.GetRotation();

        if(Input.GetKeyDown(KeyCode.A))
		{
            rot = Quaternion.Euler(new Vector3(0, -30.0f, 0) + mat.GetRotation().eulerAngles);            
		}

        if(Input.GetKeyDown(KeyCode.D))
		{
            rot = Quaternion.Euler(new Vector3(0, 30.0f, 0) + mat.GetRotation().eulerAngles);            
		}
        
        var pos = mat.GetPosition();
        //var rot = mat.GetRotation();
       
        FixedUpdateGrabbingObjectTransform(pos, rot, grabbable.transform.localScale);

#elif OCULUS_TOUCH  
        
        //掴んだObjectの参照
        var grabbable = grabInfo_.grabbable;

        //TouchPadのY方向の座標の大きさをもとに移動率を決定  stickY = -1 ~ 1の間
        var stickY = Device.instance.GetCoord(side).y;

        //StickMoveSpeedは0.1がデフォルト, ここで正負の移動を決める
        var stickMove = stickY * stickMoveSpeed;
        
        //Y軸ギリギリにタッチしたときでも動くようにする
        var stickMoveFilter = stickY > Mathf.Epsilon ? 0.1f : 0.3f;

        //Yの入力位置に対して、移動方向を変更し続ける
        grabInfo_.stickMove += (stickMove - grabInfo_.stickMove) * stickMoveFilter;

        //移動距離の算出 ここで、grabしたobjectを前に進ませる、後ろに進ませる距離を算出
        var dist = Mathf.Clamp(grabInfo_.distance + grabInfo_.stickMove, 0f, maxGrabDistance);
        
        //コントローラーとGrabオブジェクト間の距離
        var actualDist = (targetPos - gripTransform.position).magnitude;
        
        //移動させる距離の比が明らかに大きい場合
        var deltaDist = dist - actualDist;
        var threshDist = Mathf.Max(dist * 0.1f, 0.1f);
        if (Mathf.Abs(deltaDist) > threshDist)
        {
            dist = Mathf.Lerp(grabInfo_.distance, actualDist, 0.05f);
        }

        grabInfo_.distance = dist;

        //Grabしているオブジェクトに座標変換するようの行列
        var mat = grabInfo_.gripToGrabbableMat;

        var pos = mat.GetPosition();
        var rot = mat.GetRotation();
       
        FixedUpdateGrabbingObjectTransform(pos, rot, grabbable.transform.localScale);
#endif
    }

    void FixedUpdateGrabbingObjectTransform(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        //現在のGrabbableを取得
        var grabbable = grabInfo_.grabbable;

        //minGrabSmoothDist = 0.5, maxGrabSmoothDist = 2.0f 動く割合を出してる？
        var a = (Mathf.Clamp(grabInfo_.distance, minGrabSmoothDist, maxGrabSmoothDist) - minGrabSmoothDist) / (maxGrabSmoothDist - minGrabSmoothDist);

        //minGrabSmoothFilter = 0.15
        var targetFilter = 1f - (1f - minGrabSmoothFilter) * a;
        var filter = grabInfo_.smoothFilter + (targetFilter - grabInfo_.smoothFilter) * 0.1f;
        grabInfo_.smoothFilter = filter;

        pos = Vector3.Lerp(grabbable.position, pos, filter);
        scale = Vector3.Lerp(grabbable.transform.localScale, scale, filter);

        var v = (pos - grabbable.position) / Time.fixedDeltaTime;
        grabInfo_.velocity.Add(v);

        grabbable.scale = scale;
        grabbable.position = pos;

        var mat = grabInfo_.gripToGrabbableMat;
        Quaternion matRot = mat.GetRotation();

        if(rot != matRot)
        {
            //grabbable.rotationの更新は次のFixedUpdateで反映されるため、回転後のgrabInfo_の更新をここでできない
            grabbable.transform.rotation = rot;
            //grabbable.rotation = rot;
            grabInfo_.initGripToGrabbableMat = grabInfo_.grabMat.inverse * grabbable.transform.localToWorldMatrix;
            grabInfo_.initGrabbableToGrabMat = grabbable.transform.worldToLocalMatrix * grabInfo_.grabMat;                                
        }
        else
        {
            grabbable.rotation = rot;
        }
                      
    }

    private IEnumerator MoveFoward()
    {
        float FixedPassedTime = 0;        

        //Debug.Log("isMovingFoward is true");
        while(FixedPassedTime < 0.5f)
        {
            FixedPassedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isMovingForward = false;        
        //Debug.Log("isMovingFoward is false");
    }


    private IEnumerator MoveBack()
    {
        float FixedPassedTime = 0;        

        Debug.Log("isMovingBack is true");
        while(FixedPassedTime < 0.5f)
        {
            FixedPassedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isMovingBack = false;        
        Debug.Log("isMovingBack is false");
    }


//     void MoveWithFlickInput()
//     {
// #if UNITY_EDITOR && UNITY_STANDALONE
// 		if(Input.GetKeyDown(KeyCode.W))
// 		{
//             Debug.Log("W pressed");
// 			isMovingForward = true;
// 		}
// 		else if(Input.GetKeyDown(KeyCode.S))
// 		{
//             Debug.Log("S pressed");
//             isMovingBack = true;
//         }
//         else if(Input.GetKeyDown(KeyCode.A))
// 		{
//             Debug.Log("A pressed");
// 			isRotatingLeft = true;
// 		}
// 		else if(Input.GetKeyDown(KeyCode.D))
// 		{
//             Debug.Log("D pressed");
//             isRotatingRight = true;
//         }
// #else        
//         if(Device.instance.GetUpFlicked())
//         {
//             isMovingForward = true;
//         }        

//         else if(Device.instance.GetDownFlicked())
//         {
//             isMovingDown = true;
//         }
// #endif        
//         var grabbable = grabInfo_.grabbable;

//         var mat = grabInfo_.gripToGrabbableMat;

//         var pos = mat.GetPosition();
//         var rot = mat.GetRotation();

//         if(isMovingForward)
//         {
//             //var movingAmount = Vector3.Lerp(grabbable.position, pos, 0.001f); 
// #if UNITY_EDITOR && UNITY_STANDALONE            
//             var movingAmount = (targetPos - Camera.main.transform.position).normalized * 2.0f;
//             //var movingAmount = (targetPos - gripTransform.position).normalized * 2.0f;
// #else           
//             var movingAmount = (targetPos - gripTransform.position).normalized;
// #endif            
//             Debug.LogFormat("MovingAmount X:{0}, Y:{1}, Z:{2}", movingAmount.x, movingAmount.y, movingAmount.z);
//             pos = pos + movingAmount;

//             grabInfo_.distance = pos.magnitude;
//             // var v = (pos - grabbable.position) / Time.fixedDeltaTime;
//             // grabInfo_.velocity.Add(v);
            
//             //Debug.LogFormat("MovingForward Amount : {0}", pos.magnitude);
//             grabbable.position = pos;
//             grabbable.rotation = rot;      
//             //Debug.LogFormat("Move Forward. After Pos X : {0}, Y : {1}, Z : {2}", pos.x, pos.y, pos.z);
                                    
//             isMovingForward = false;        
//         }
//         else if(isMovingBack)
//         {
//             //Debug.LogFormat("Move Down. Prev Pos X : {0}, Y : {1}, Z : {2}", pos.x, pos.y, pos.z);
//             //var movingAmount = Vector3.Lerp(pos, grabbable.position, 0.001f);                        
// #if UNITY_EDITOR && UNITY_STANDALONE            
//             var movingAmount = (targetPos - Camera.main.transform.position).normalized * 2.0f;
//             //var movingAmount = (targetPos - gripTransform.position).normalized * 2.0f;
// #else           
//             var movingAmount = (targetPos - gripTransform.position).normalized;
// #endif           
//             Debug.LogFormat("MovingAmount X:{0}, Y:{1}, Z:{2}", movingAmount.x, movingAmount.y, movingAmount.z);
//             pos = pos - movingAmount;
            
//             grabInfo_.distance = pos.magnitude;
//             // var v = (pos - grabbable.position) / Time.fixedDeltaTime;
//             // grabInfo_.velocity.Add(v);
            
//             Debug.LogFormat("MovingDown Amount : {0}", pos.magnitude);
//             grabbable.position = pos;
//             grabbable.rotation = rot;      
//             //Debug.LogFormat("Move Forward. After Pos X : {0}, Y : {1}, Z : {2}", pos.x, pos.y, pos.z);
                        
//             isMovingBack = false;        
//         }      
//     }

        // void FixedUpdateGrabbingObjectByDualHand()
    // {
    //     if (!isPrimary) return;

    //     var secondary = opposite;
    //     Assert.IsNotNull(secondary);

    //     var primaryGripPos = gripTransform.position;
    //     var primaryGripRot = gripTransform.rotation;
    //     var secondaryGripPos = secondary.gripTransform.position;
    //     var secondaryGripRot = secondary.gripTransform.rotation;

    //     var primaryMat = grabInfo_.gripToGrabbableMat;
    //     var secondaryMat = secondary.grabInfo_.gripToGrabbableMat;
    //     var primaryPos = primaryMat.GetPosition();
    //     var secondaryPos = secondaryMat.GetPosition();

    //     var center = (primaryPos + secondaryPos) / 2;
    //     var dCenter = center - dualGrabInfo_.center;
    //     var pos = dualGrabInfo_.pos + dCenter;

    //     var primaryToSecondary = primaryGripPos - secondaryGripPos;
    //     var currentDir = primaryToSecondary.normalized;
    //     var initDir = dualGrabInfo_.primaryToSecondary.normalized;
    //     var dRot = Quaternion.FromToRotation(initDir, currentDir);
    //     var rot = dRot * dualGrabInfo_.rot;

    //     var scale = dualGrabInfo_.scale;
    //     if (grabInfo_.grabbable.isScalable)
    //     {
    //         var currentDistance = primaryToSecondary.magnitude;
    //         var initDistance = dualGrabInfo_.primaryToSecondary.magnitude;
    //         scale *= currentDistance / initDistance;
    //     }

    //     grabInfo_.smoothFilter = 0f;
    //     FixedUpdateGrabbingObjectTransform(pos, rot, scale);
    // }

    
    // void ReGrab()
    // {
    //     var grabbable = grabInfo_.grabbable;
    //     if (!grabbable) return;

    //     var grabMat = grabInfo_.grabMat;
    //     grabInfo_.initGripToGrabbableMat = grabMat.inverse * grabbable.transform.localToWorldMatrix;
    // }

    
    // void SecondGrab(VrgGrabbable grabbable)
    // {
    //     var primary = opposite;
    //     var secondary = this;

    //     var primaryMat = primary.grabInfo_.gripToGrabbableMat;
    //     var secondaryMat = secondary.grabInfo_.gripToGrabbableMat;
    //     var primaryPos = primaryMat.GetPosition();
    //     var secondaryPos = secondaryMat.GetPosition();
    //     var primaryGripPos = primary.gripTransform.position;
    //     var secondaryGripPos = secondary.gripTransform.position;

    //     primary.dualGrabInfo_.primaryToSecondary = primaryGripPos - secondaryGripPos;
    //     primary.dualGrabInfo_.pos = grabbable.transform.position;
    //     primary.dualGrabInfo_.center = (primaryPos + secondaryPos) / 2;
    //     primary.dualGrabInfo_.rot = grabbable.transform.rotation;
    //     primary.dualGrabInfo_.scale = grabbable.transform.localScale;

    //     grabInfo_.isKinematic = primary.grabInfo_.isKinematic;
    // }
}

}