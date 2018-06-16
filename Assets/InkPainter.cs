using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VrGrabber;

public class InkPainter : MonoBehaviour {

	[SerializeField]
	private float width = 1.0f;

	[SerializeField]
	private float height = 1.0f;

	[SerializeField]
	private Material inkMat;

	[SerializeField]
	private GameObject emptyInk;

	private VrgGrabber grabber;

	private void Awake()
	{
		grabber = FindObjectOfType<VrgGrabber>();
		if(grabber != null)
		{
			grabber.updateTouchHitEvent += CatchRayCastInfo;
		}	
		else
		{
			//Destroy(this);
		}	
	}

	public void CatchRayCastInfo(RaycastHit hit)
	{
		if(hit.collider.name == "WhiteBoard")
		{
			//Debug.Log("Hitted WhiteBoard");			
			CreateInk(hit.point, hit.collider.transform);
		}		
		else
		{
			//Debug.Log("Hitted" + hit.collider.name);
		}
	}

	//二次元ベジェでtで線形補間した頂点位置を取る
	//p0 : 位置ベクトル1, p1 : 位置ベクトル2, p2 : 位置ベクトル3
	Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		var a = Vector3.Lerp(p0, p1, t); //p0とp1のtでの線形補間
		var b = Vector3.Lerp(p1, p2, t); //p1とp2のtでの線形補間
		
		return Vector3.Lerp(a, b, t); //ベジェでの頂点位置取得		
	}

	private void CreateInk(Vector3 centerPos, Transform parent)
	{
		//Planeの各4点座標位置を計算. 左上:x1, 右上:x2, 右下:x3, 左下:x4
		/*
			x1-------------x2
			  ❘				❘
			  ❘		 *(0, 0)❘
			  ❘				❘
			x4-------------x3
		 */

		//Meshの作成はモデル座標原点に対するxy平面を使って行う
		//PositionやRotationの操作はモデル座標軸で行われてしまうので,原点基準でMeshを作成する
		var x1 = new Vector3(-(width/2.0f), (height/2.0f) , 0); 
		var x2 = new Vector3((width/2.0f), (height/2.0f) , 0);
		var x3 = new Vector3((width/2.0f), -(height/2.0f) , 0);
		var x4 = new Vector3(-(width/2.0f), -(height/2.0f) , 0);
		
		//t = 0.4, 0.6としてベジェによる頂点を取得
		float t1 = 0.4f;
		float t2 = 0.6f;

		//x1位置
		var x1_t1 = GetPoint(x4, x1, x2, t1);
		var x1_t2 = GetPoint(x4, x1, x2, t2);

		//x2位置
		var x2_t1 = GetPoint(x1, x2, x3, t1);
		var x2_t2 = GetPoint(x1, x2, x3, t2);

		//x3位置
		var x3_t1 = GetPoint(x2, x3, x4, t1);
		var x3_t2 = GetPoint(x2, x3, x4, t2);

		//x4位置
		var x4_t1 = GetPoint(x3, x4, x1, t1);
		var x4_t2 = GetPoint(x3, x4, x1, t2);

		//8つの頂点で八角形Meshを作成
		Mesh ink = new Mesh();		

		//頂点情報
		ink.vertices = new Vector3[]
		{
			x1_t1,
			x1_t2,

			x2_t1,
			x2_t2,

			x3_t1,
			x3_t2,

			x4_t1,
			x4_t2,
		};

		//UV情報
		ink.uv = new Vector2[]
		{
			new Vector2(0, 0.25f),
			new Vector2(0.25f, 0),

			new Vector2(0.75f, 0),
			new Vector2(1.0f, 0.25f),

			new Vector2(1.0f, 0.75f),
			new Vector2(0.75f, 1.0f),

			new Vector2(0.25f, 1.0f),
			new Vector2(0, 0.75f),			
		};

		//頂点インデックス情報
		ink.triangles = new int[]
		{
			0, 1, 2,
			0, 2, 3,
			0, 3, 4,
			0, 4, 7,
			4, 5, 7,
			5, 6, 7
		};

		//法線とバウンディングボックスの再計算
		ink.RecalculateNormals();
		ink.RecalculateBounds();
		
		//Debug.LogFormat("CenterPos; X : {0}, Y : {1}, Z : {2}", centerPos.x, centerPos.y, centerPos.z);

		//Meshをレイのヒットポイントへ移動
		var inkObj = Instantiate(emptyInk, new Vector3(centerPos.x, centerPos.y, centerPos.z), Quaternion.identity);
		
		var renderer = inkObj.AddComponent<MeshRenderer>();		
		renderer.material = inkMat;

		var meshFilter = inkObj.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = ink;
		
		//元のscaleを保持
		var parentScale = parent.localScale;
		var inkObjScale = inkObj.transform.localScale;
		
		inkObj.transform.parent = parent;

		//parentを親にした際にinkのlocalPositionの計算が正しく行われない場合があるので
		inkObj.transform.localScale = new Vector3(inkObjScale.x / parentScale.x, inkObjScale.y / parentScale.y, inkObjScale.z / parentScale.z);
		
		// Debug.LogFormat("inkObj_2; X : {0}, Y : {1}, Z : {2}", inkObj.transform.position.x, inkObj.transform.position.y, inkObj.transform.position.z);
		// Debug.LogFormat("Parent Z : {0}", parent.transform.position.z);
		// Debug.LogFormat("Child Z : {0}", parent.transform.GetChild(0).transform.position.z);
	}

	private void OnDestroy()
	{
		grabber.updateTouchHitEvent -= CatchRayCastInfo;
	}
}
