using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaserPointer : MonoBehaviour {

	public bool IsActive = true;            //稼働フラグ
    public float defaultLength = 0.5f;      //ヒットなしのときのレーザーの長さ

    public bool shotRay = true;             //Ray を撃つ(false のときは target に指す位置を入れる)
    public float rayLength = 500f;          //Ray の長さ
    public LayerMask rayExclusionLayers;    //Ray 判定を除外するレイヤー

    public Transform target = null;                //指す位置（shotRay=true のときはヒットしたオブジェクトの transform が入る）

    public Transform anchor;                //発射位置（コントローラの位置）
    public LineRenderer lineRenderer;       //レーザーを描画するラインレンダラ


    // Use this for initialization
    void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();		
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsActive)
        {
            lineRenderer.enabled = false;
            return;
        }

        if (shotRay)
        {
            Ray ray = new Ray(anchor.position, anchor.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayLength, ~rayExclusionLayers))
            {
				//前のフレームと今のフレームでhitしているobjectが異なるときは入力イベントをdetachする
				if(target!=null && target.gameObject.name != hit.collider.gameObject.name)
				{
					//Debug.Log("target and hit is different");									
					InitializeTarget();									
				}			

				//今のフレームのObjectがフリックイベントを保つ場合にAttachする
				var hitEventAttacher = hit.transform.GetComponent(typeof(IEventDefinition)) as IEventDefinition;
				if(hitEventAttacher != null)
				{
					//Debug.Log("Called AttachEvent");
					hitEventAttacher.AttachedEvents();					
				}

                target = hit.transform;

                DrawTo(hit.point);     //ヒットした位置にしたいため
                return;
            }

			//今回何もhitしていないときかつ前のフレームでhitしたobjectがあった場合もフリックイベントをdetachする
			InitializeTarget();	
            target = null;
        }

        if (target != null)
            DrawTo(target.position);
        else
            DrawTo(anchor.position + anchor.forward * defaultLength);   //コントローラの正面方向へ一定の長さ
    }

    //レーザーを描く
    void DrawTo(Vector3 pos)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, anchor.position);
        lineRenderer.SetPosition(1, pos);
    }

	void InitializeTarget()
	{
		if(target == null)
		{
			return;
		}

		var targetEventAttacher = target.GetComponent(typeof(IEventDefinition)) as IEventDefinition;
		if(targetEventAttacher != null)
		{					
			//Debug.Log("Called DetachEvent");	
			targetEventAttacher.DetachedEvents();						
		}
		else
		{
			//Debug.LogError("No targetEventAttacher");
		}			
	}
}
