using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkCreater : MonoBehaviour {

	[SerializeField]
	private float width = 1.0f;

	[SerializeField]
	private float height = 1.0f;

	[SerializeField]
	private Material inkMat;

	[SerializeField]
	private GameObject emptyInk;

	//二次元ベジェでtで線形補間した頂点位置を取る
	//p0 : 位置ベクトル1, p1 : 位置ベクトル2, p2 : 位置ベクトル3
	Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		var a = Vector3.Lerp(p0, p1, t); //p0とp1のtでの線形補間
		var b = Vector3.Lerp(p1, p2, t); //p1とp2のtでの線形補間
		
		return Vector3.Lerp(a, b, t); //ベジェでの頂点位置取得		
	}

	public void CreateInk(Vector3 centerPos)
	{
		//Planeの各4点座標位置を計算. 左上:x1, 右上:x2, 右下:x3, 左下:x4
		/*
			x1-------------x2
			  ❘				❘
			  ❘		 *		❘
			  ❘				❘
			x4-------------x3
		 */
		var x1 = centerPos + new Vector3(-(width/2.0f), (height/2.0f) , 0); 
		var x2 = centerPos + new Vector3((width/2.0f), (height/2.0f) , 0);
		var x3 = centerPos + new Vector3((width/2.0f), -(height/2.0f) , 0);
		var x4 = centerPos + new Vector3(-(width/2.0f), -(height/2.0f) , 0);
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
		
		var inkObj = Instantiate(emptyInk, centerPos, Quaternion.identity);
		var renderer = inkObj.AddComponent<MeshRenderer>();
		renderer.material = new Material(inkMat);

		var meshFilter = inkObj.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = ink;
	}
}
