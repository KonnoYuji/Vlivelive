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
	private bool isLastPanelLevel = false;

	[SerializeField]
	private RectTransform normalizedObj; //規格化用のパネル このロジックはローカル座標でのz軸が各パネルで同じ時に適応可能

	private float normalizedValue = 0;

	[SerializeField]
	private float panelTouchableRadius = 0.05f; //デフォルトは直径0.1の大きさの球の範囲を一パネルの認識範囲とする

	[SerializeField]
	private Transform standardAxis; //基準となるローカル座標系

	private GameObject currentItem = null;

	[SerializeField]
	private Text outputText;

	private bool onetimeShow = false;

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

		if(!standardAxis)
		{
			var canvas = this.GetComponentInParent<Canvas>();
			if(!canvas)
			{
				standardAxis = canvas.transform;
			}
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
		//Vector2 tempTwoAxis = new Vector2(normalizedObj.localPosition.x, normalizedObj.localPosition.y);
		Vector2 tempTwoAxis = standardAxis.InverseTransformPoint(normalizedObj.position);
		return tempTwoAxis.magnitude;
	}

	//パネルのタッチパネル上での位置の計算
	private Vector2 CalculatePositionOnTouchPad(RectTransform targetObj)
	{
		//Vector2 tempTwoAxis = new Vector2(targetObj.localPosition.x, targetObj.localPosition.y);
		Vector2 tempTwoAxis = standardAxis.InverseTransformPoint(targetObj.position); //基準ローカル座標への変換
		tempTwoAxis.x = tempTwoAxis.x / normalizedValue;
		tempTwoAxis.y = tempTwoAxis.y / normalizedValue;		

		//Debug.LogFormat("NormalizedAxis. X:{0}, Y:{1}", tempTwoAxis.x, tempTwoAxis.y);		
		return tempTwoAxis;		
	}	

	//入力されたタッチパッドが、各パネルの領域内にいるかチェック
	private int[] CheckExistenceInPanelTerritory(Vector2 InputAxis)
	{
		List<int> indexs = new List<int>(); //パネルのインデックス

		for(int i=0; i<ItemsPositionOnTouchPad.Length; i++)
		{
			var isChecked = CheckExistenceInPanelTerritoryInternal(InputAxis, ItemsPositionOnTouchPad[i]);
            if(isChecked)
            {
                indexs.Add(i);
            }		
		}		

		return indexs.ToArray();
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

	private void SelectPanel(int index)
	{		
		ResetPanelColor();
		DeactiveBelowPanel(index);

		currentItem = Items[index].gameObject;
		Image image;
		image = currentItem.GetComponent<Image>();
			
		if(image)
		{			
			image.color = new Color(0.5f, 1.0f, 1.0f);		
		}

		if(!isLastPanelLevel)
		{
			ShowNextPanel();
		}							
	}

	private void ShowNextPanel()
	{
		if(!currentItem)
		{
			return;
		}		
		
		var controller = currentItem.GetComponentInChildren<FlowerTextPadControllerMapVer>(true);

		if(!controller.gameObject.activeSelf)
		{
			controller.gameObject.SetActive(true);
		}		
	}

	private void OnEnable()
	{		
		if(isLastPanelLevel)
		{
			OculusGoInput.Instance.ClickedPad += InputText;
		}		
	}

	private void OnDisable()
	{
		if(isLastPanelLevel)
		{
			OculusGoInput.Instance.ClickedPad -= InputText;
		}
		ResetPanelColor();
		DeactiveBelowPanel(-1);
	}
	
	private void InputText()
	{
		//ここでのitemはインプットイベントで文字を入力するパネルとなる
		if(!currentItem)
		{
			return;
		}

		var text = currentItem.GetComponentInChildren<Text>();
		outputText.text += text.text;

		if(this.gameObject.activeSelf)
		{
			this.gameObject.SetActive(false);
		}
	}

	// Update is called once per frame
	private void Update () {

		// if(!onetimeShow)
		// {
		// 	for(int i=0; i<ItemsPositionOnTouchPad.Length; i++)
		// 	{
		// 		Debug.LogFormat("ItemPositionOnTouchPad Number:{0}  X:{1} Y:{2}\n", i, ItemsPositionOnTouchPad[i].x, ItemsPositionOnTouchPad[i].y);
		// 	}
		// 	onetimeShow = true;
		// }
		
		var currentAxis = CatchTouchAxis();			
		if(currentAxis.x == 0 && currentAxis.y == 0)
		{				
			ResetPanelColor();
			DeactiveBelowPanel(-1);
			return;
		}

		var indexs = CheckExistenceInPanelTerritory(currentAxis);
		int thisIndex = 0;

		if(indexs.Length > 1)
		{
			//Debug.Log("Found OverLap Panel");			
		}
		else if(indexs.Length > 0)
		{			
			thisIndex = indexs[0];
			SelectPanel(thisIndex);					
		}								
	}

	//パネルカラーをもとに戻し自分より下の階層のパネルをオフにする
	private void ResetPanelColor()
	{
		//Debug.Log("Call ResetCurrentItenm");
		for(int i=0; i<Items.Length; i++)
		{
			Image image;
			image = Items[i].GetComponent<Image>();				
			image.color = new Color(1.0f, 1.0f, 1.0f);			
		}	
	}

	//除外対象以外のGameObjectをfalseにする
	//exclusionIndexが負のときは除外対象はない
	private void DeactiveBelowPanel(int exclusionIndex)
	{
		for(int i=0; i<Items.Length; i++)
		{
			if(exclusionIndex > 0 && exclusionIndex == i)
			{
				continue;
			}			
			var beforeFlower = Items[i].GetComponentInChildren<FlowerTextPadControllerMapVer>();
			if(beforeFlower)
			{
				beforeFlower.gameObject.SetActive(false);
			}	
		}	
	}
}
