using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTracer : MonoBehaviour {

	[SerializeField]
	private Transform standardAxis; //基準となるローカル座標系
	
	[SerializeField]
	private float Z_Offset = 0;

	[SerializeField]
	private TouchPadPanelManipulator manipulator;

	private void Awake()
	{
		manipulator.OnChangedCurrentAxis += TraceTouch;		
	}
	
	private void TraceTouch(Vector2 axis, float normalizedValue)
	{
		var localAxis = new Vector3(axis.x * normalizedValue, axis.y * normalizedValue, Z_Offset);	//基準位置に対するローカル座標を取得(zはもともと0)
		var worldAxis = standardAxis.TransformPoint(localAxis); //ローカルからワールド座標への変換
		this.transform.position = worldAxis;
	}
}
