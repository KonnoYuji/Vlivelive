using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//タッチパッド上にパネルをマッピング、タッチパネルの入力からどのパネルをタッチしたか判定
public class TouchPadPanelManipulator : MonoBehaviour {

	[SerializeField]
	private OculusGoControllerInfo touchPadAxisSource;

	[SerializeField]
	protected RectTransform[] Panels; //Manipulatorで扱うパネル
	private Vector2[] PanelsPositionOnTouchPad; //パネルのTouchPad上でのポジション

	[SerializeField]
	private RectTransform normalizedObj; //規格化用のパネル このロジックはローカル座標でのz軸が各パネルで同じ時に適応可能

	private float normalizedValue = 0;

	[SerializeField]
	private float panelTouchableRadius = 0.05f; //デフォルトは直径0.1の大きさの球の範囲を一パネルの認識範囲とする

	[SerializeField]
	private Transform standardAxis; //基準となるローカル座標系

	protected Vector2 currentAxis;

	public Action<Vector2, float> OnChangedCurrentAxis;
	
	protected void Awake()
	{
		if(normalizedObj != null)
		{			
			normalizedValue = CalculateNormalizedValue();			
		}	

		if(Panels == null || Panels.Length == 0)
		{
			var childNum = transform.childCount;
			Panels = new RectTransform[childNum];
			
			for(int i=0; i<childNum; i++)
			{				
				Panels[i] = transform.GetChild(i).GetComponent<RectTransform>();
			}
		}

		if(Panels != null)
		{
			PanelsPositionOnTouchPad = new Vector2[Panels.Length];			
		}

		if(!standardAxis)
		{
			var canvas = this.GetComponentInParent<Canvas>();
			if(!canvas)
			{
				standardAxis = canvas.transform;
			}
		}

		if(PanelsPositionOnTouchPad != null)
		{
			for(int i=0; i<PanelsPositionOnTouchPad.Length; i++)
			{
				PanelsPositionOnTouchPad[i] = CalculatePositionOnTouchPad(Panels[i]);
			}			
		}		
	}

	private Vector2 CatchTouchAxis()
	{
		if(touchPadAxisSource == null)
		{
			touchPadAxisSource = FindObjectOfType<OculusGoControllerInfo>();
		}	
		return touchPadAxisSource.PrimaryTouchpad;
	}

	//規格化係数の計算
	private float CalculateNormalizedValue()
	{
		//Vector2 tempTwoAxis = new Vector2(normalizedObj.localPosition.x, normalizedObj.localPosition.y);
		Vector2 tempTwoAxis = standardAxis.InverseTransformPoint(normalizedObj.position);
		return tempTwoAxis.magnitude;
	}

	//パネルのタッチパネル上での位置を計算
	private Vector2 CalculatePositionOnTouchPad(RectTransform targetObj)
	{
		//Vector2 tempTwoAxis = new Vector2(targetObj.localPosition.x, targetObj.localPosition.y);
		Vector2 tempTwoAxis = standardAxis.InverseTransformPoint(targetObj.position); //基準ローカル座標への変換
		tempTwoAxis.x = tempTwoAxis.x / normalizedValue;
		tempTwoAxis.y = tempTwoAxis.y / normalizedValue;		

		//Debug.LogFormat("NormalizedAxis. X:{0}, Y:{1}", tempTwoAxis.x, tempTwoAxis.y);		
		return tempTwoAxis;		
	}	

	//タッチパッドから入力された位置が、各パネルの領域内にいるかチェック
	protected int[] CheckExistenceInPanelTerritory(Vector2 InputAxis)
	{
		List<int> indexs = new List<int>(); //パネルのインデックス

		for(int i=0; i<PanelsPositionOnTouchPad.Length; i++)
		{
			var isChecked = CheckExistenceInPanelTerritoryInternal(InputAxis, PanelsPositionOnTouchPad[i]);
            if(isChecked)
            {
                indexs.Add(i);
            }		
		}		

		return indexs.ToArray();
	}
	
	//入力されたタッチパッドが、パネルの領域内にいるかチェック
	protected bool CheckExistenceInPanelTerritoryInternal(Vector2 InputAxis, Vector2 targetPanelAxis)
	{
		var result = Mathf.Pow((InputAxis.x - targetPanelAxis.x), 2) + Mathf.Pow((InputAxis.y - targetPanelAxis.y), 2);
		result = Mathf.Pow(result, 0.5f);
		if(result > panelTouchableRadius)
		{
			return false;
		}
		else return true;
	}

	protected void OnEnable()
	{		
		OculusGoInput.Instance.ClickedPad += ClickOnPanel;	
	}

	protected void OnDisable()
	{
		OculusGoInput.Instance.ClickedPad -= ClickOnPanel;
	}
	
	protected virtual void ClickOnPanel()
	{		
	}

	// Update is called once per frame
	protected void Update () {
		
		currentAxis = CatchTouchAxis();							
		if(OnChangedCurrentAxis != null)
		{
			OnChangedCurrentAxis(currentAxis, normalizedValue);
		}
	}
}
