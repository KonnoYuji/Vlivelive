using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerTextPadControllerMapVer : MonoBehaviour{
 
	[SerializeField]
	private OculusGoControllerInfo touchPadAxisSource;

	[SerializeField]
	private RectTransform[] Items; //Controllerで扱うパネル

	private Vector2[] ItemsPositionOnTouchPad; //TouchPad上でのポジション

	[SerializeField]
	private int myLevel = 0; //階層のレベル 0~2まで

	[SerializeField]
	private RectTransform normalizedObj; //規格化用のパネル このロジックはローカル座標でのz軸が各パネルで同じ時に適応可能

	[SerializeField]
	private float normalizedValue = 0;

	[SerializeField]
	private float panelTouchableRadius = 0.05f; //デフォルトは直径0.1の大きさの球の範囲を一パネルの認識範囲とする


	private GameObject currentItem = null;

	[SerializeField]
	private Text outputText;


	private void Awake()
	{
		if(normalizedObj != null)
		{
			normalizedValue = CalculateNormalizedValue();
		}	

		if(Items != null)
		{
			ItemsPositionOnTouchPad = new Vector2[Items.Length];
		}

		if(ItemsPositionOnTouchPad != null)
		{
			for(int i=0; i<ItemsPositionOnTouchPad.Length; i++)
			{
				ItemsPositionOnTouchPad[i] = CalculatePositionOnTouchPad(Items[i]);
			}
		}		
	}

	public Vector2 CatchTouchAxis()
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
		Vector2 tempTwoAxis = new Vector2(normalizedObj.localPosition.x, normalizedObj.localPosition.y);
		return tempTwoAxis.magnitude;
	}

	//パネルのタッチパネル上での位置の計算
	private Vector2 CalculatePositionOnTouchPad(RectTransform targetObj)
	{
		Vector2 tempTwoAxis = new Vector2(targetObj.localPosition.x, targetObj.localPosition.y);
		tempTwoAxis.x = tempTwoAxis.x / normalizedValue;
		tempTwoAxis.y = tempTwoAxis.y / normalizedValue;		
		return tempTwoAxis;		
	}	

	//入力されたタッチパッドが、各パネルの領域内にいるかチェック
	private bool CheckExistenceInPanelTerritory(Vector2 InputAxis)
	{
		bool isChecked = false;

		for(int i=0; i<ItemsPositionOnTouchPad.Length; i++)
		{
			isChecked = CheckExistenceInPanelTerritoryInternal(InputAxis, ItemsPositionOnTouchPad[i]);		
		}		

		return isChecked;
	}
	
	//入力されたタッチパッドが、パネルの領域内にいるかチェック
	private bool CheckExistenceInPanelTerritoryInternal(Vector2 InputAxis, Vector2 targetPanelAxis)
	{
		var result = Mathf.Pow((InputAxis.x - targetPanelAxis.x), 2) + Mathf.Pow((InputAxis.y - targetPanelAxis.y), 2);
		result = Mathf.Pow(result, 0.5f);
		if(result > panelTouchableRadius)
		{
			return false;
		}
		else return true;
	}

	private void OnEnable()
	{
		
	}

	private void OnDisable()
	{
		
	}
	

	// Update is called once per frame
	private void Update () {

		var currentAxis = CatchTouchAxis();			
	}

	private bool CheckExistenceInCircle(Vector2 axis)
	{
		return true;
	}

	private void SelectCurrentItem(int key)
	{		
	}

	private void ResetPanel()
	{
		//Debug.Log("Call ResetCurrentItenm");
		for(int i=0; i<Items.Length; i++)
		{
			Image image;
			image = Items[i].GetComponent<Image>();				
			image.color = new Color(1.0f, 1.0f, 1.0f);
		}
	}
}
