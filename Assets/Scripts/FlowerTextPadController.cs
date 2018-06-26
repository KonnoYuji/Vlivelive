using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerTextPadController : MonoBehaviour{
 
	[SerializeField]
	private OculusGoControllerInfo touchPadAxisSource;

	[SerializeField]
	private GameObject[] items; //一つの円の中に入るパネルの数

	private Dictionary<int, Vector2> rotationRanges; //各パネルの領域(ある角度で決まる扇形の領域を指す)

	[SerializeField]
	private float radius = 1.0f;

	[SerializeField]
	private float ignoredRadius = 0; //radius > ignoredRadius
	private int currentRangeKey = 0;

	private GameObject currentItem = null;
	
	public int myLevel = 0; //フラワーパネルの階層番号 0~2まで

	[SerializeField]
	private Text outputText;

	
	[SerializeField]
	private Vector2 offset = new Vector2(0, 0); //扇角のoffset. offsetは、円弧的に必ずyのほうが大きくなるように設定することを前提
	
	[HideInInspector]
	public float sita1 = 0; //タッチ入力の検出範囲(最小値)

	[HideInInspector]
	public float sita2  = 360.0f; //タッチ入力の検出範囲(最大値)
	
	[HideInInspector]
	public bool isEnable = false;

	[HideInInspector]
	public bool settedRotationRange = false;

	[SerializeField]
	private	 bool isDebugging = false;

	[SerializeField]
	private bool isShowedNextPanelAuto = false;


	public Vector2 CatchTouchAxis()
	{
		if(touchPadAxisSource == null)
		{
			touchPadAxisSource = FindObjectOfType<OculusGoControllerInfo>();
		}	
		return touchPadAxisSource.PrimaryTouchpad;
	}

	public void ShowNextPanel()
	{
		if(!isEnable || currentItem == null)
		{
			return;
		}			

		var flowerCtr = currentItem.GetComponentInChildren<FlowerTextPadController>(true);
		if(flowerCtr != null)
		{
			if(!flowerCtr.settedRotationRange)
			{
				Vector2 range;
				bool existed = false;
				existed = rotationRanges.TryGetValue(currentRangeKey, out range);
				if(existed)
				{
					flowerCtr.SetTouchableRange(range);
				}				
			}
			flowerCtr.gameObject.SetActive(true);
			
			if(!isShowedNextPanelAuto)
			{
				isEnable = false;
			}			
			//Debug.LogFormat("isEnable is {0}", isEnable);			
		}
		else
		{
			outputText.text += currentItem.GetComponentInChildren<Text>().text;
			this.gameObject.SetActive(false);
			var parentFlowerCtr = GetComponentInParent<FlowerTextPadController>();
			if(parentFlowerCtr != null && !isShowedNextPanelAuto)
			{
				parentFlowerCtr.isEnable = true;
			}
		}
	}

	private void OnEnable()
	{
		if(!isEnable)
		{
			isEnable = true;
		}
		
		if(!isShowedNextPanelAuto)
		{
			OculusGoInput.Instance.ClickedPad += ShowNextPanel;
		}

		if(!settedRotationRange)
		{
			SetTouchableRange(new Vector2(sita1, sita2));
		}
	}

	private void OnDisable()
	{
		if(!isShowedNextPanelAuto)
		{
			OculusGoInput.Instance.ClickedPad -= ShowNextPanel;
		} 
	}

	public void SetTouchableRange (Vector2 range) {				

		if(isDebugging) Debug.LogFormat("Range before : {0} ~ {1}", range.x, range.y);
		Vector2 rangeWithOffset = new Vector2((range.x + offset.x), (range.y + offset.y));
		if(isDebugging) Debug.LogFormat("Range After : {0} ~ {1}", rangeWithOffset.x, rangeWithOffset.y);
		Vector2 rangeIncrement = new Vector2(0, 0);

		rotationRanges = new Dictionary<int, Vector2>(items.Length);
		
		for(int i=0; i<items.Length; i++)
		{									
			rangeIncrement = new Vector2(((rangeWithOffset.y - rangeWithOffset.x)/items.Length * i)+rangeWithOffset.x, ((rangeWithOffset.y - rangeWithOffset.x)/items.Length * (i+1))+rangeWithOffset.x);
			if(isDebugging) Debug.LogFormat("Range {0} : {1} ~ {2}", i, rangeIncrement.x, rangeIncrement.y);
			rotationRanges.Add(i, rangeIncrement);			
		}

		settedRotationRange = true;
	}

	// Update is called once per frame
	private void Update () {
		
		if(!isEnable)
		{
			return;
		}

		var currentAxis = CatchTouchAxis();	

		if(!CheckExistenceInCircle(currentAxis))
		{			
			//currentItem = null;			
			ResetPanel();			
			return;
		}
		
		var sita = GetSita(currentAxis);
		//Debug.LogFormat("sita : {0}", sita.ToString());
		
		
		var rangeKey = GetExistedRangeKey(sita);
		
		if(currentRangeKey == rangeKey)
		{
			//Debug.Log("Key Same");
			return;
		}
		else if(rangeKey == -1)
		{
			//Debug.Log("NotFoundRange");
			return;
		}
				
		if(currentItem != null && isShowedNextPanelAuto)
		{
			var beforeFlower = currentItem.GetComponentInChildren<FlowerTextPadController>();
			if(beforeFlower != null)
			{
				beforeFlower.gameObject.SetActive(false);
			}		
		}
		
		currentRangeKey = rangeKey;
		currentItem = items[currentRangeKey];
		SelectCurrentItem(currentRangeKey);
	}

	private float GetSita(Vector2 axis)
	{
		var denominator = Mathf.Pow(axis.x, 2) + Mathf.Pow(axis.y, 2);
		denominator = Mathf.Pow(denominator, 0.5f);
		var cos  = axis.x / denominator;

		float result = 0;

		if((axis.x > 0 && axis.y < 0) || (axis.x < 0 && axis.y < 0))
		{
			result = 180.0f + (180.0f - Mathf.Acos(cos) * (180.0f /3.14f));
		}
		else
		{
			result = Mathf.Acos(cos) * (180.0f /3.14f); 
		}
		//Debug.LogFormat("sita : {0}", result.ToString());
		return result; 
	}

	private bool CheckExistenceInCircle(Vector2 axis)
	{
		if(ignoredRadius >= radius)
		{
			//Debug.Log("ignoredRadius is larger than radius, must keep smaller than radius");
		}
		var result = Mathf.Pow(axis.x, 2) + Mathf.Pow(axis.y, 2);
		if(ignoredRadius< result && result < radius)
		{
			//Debug.Log("Existed");
			return true;
		}
		else
		{
			//Debug.Log("Not Existed");
			return false;
		}
	}

	private int GetExistedRangeKey(float sita)
	{
		foreach(var range in rotationRanges)
		{	
			//if(isDebugging && range.Key == 0) Debug.LogFormat("rotationRange : {0} ~ {1}", range.Value.x, range.Value.y);		
			// if(isDebugging && range.Key == 0) Debug.LogFormat("sita : {0}", sita);

			//範囲が360度をまたぐときの処理(x<0 y>0)
			if((range.Value.x < 0 && range.Value.y > 0))
			{				
				if((((360.0f + range.Value.x) < sita) && (sita < 360.0f)) || ((0 < sita) && (sita < range.Value.y)))
				{
					 return range.Key;
				} 				
			}
			//範囲が360度をまたぐときの処理(x>0 y>360)
			else if(range.Value.y > 360.0f)
			{
				if(((range.Value.x < sita) && (sita < 360.0f)) || ((0 < sita) && (sita < (range.Value.y - 360.0f))))
				{
					 return range.Key;
				}		
			}
			//負の範囲 (x, y < 0)			
			else if(range.Value.x < 0 && range.Value.y < 0)
			{				
				if((((360.0f + range.Value.x) < sita) && (sita < (360.0f + range.Value.y))))
				{
					 return range.Key;
				}
			}
			//どちらも範囲外 (x, y > 360)
			else if(range.Value.x > 360 && range.Value.y > 360)
			{
				if(((range.Value.x - 360.0f) < sita) && (sita < (range.Value.y - 360.0f)))
				{
					return range.Key;
				}
			}
			//通常の範囲(0 < x, y < 360)
			else
			{
				if((range.Value.x < sita) && (sita < range.Value.y))
				{												
					return range.Key;
				}
			}			
		}

		if(isDebugging) Debug.LogFormat("sita : {0}", sita);
		//予期しないバグで領域内に角度が存在しなかったとき
		return -1;
	}

	private void SelectCurrentItem(int key)
	{		
		if(key >= items.Length)
		{
			return;
		}

		for(int i=0; i<items.Length; i++)
		{
			Image image;
			image = items[i].GetComponent<Image>();
			
			if(i == key)
			{			
				image.color = new Color(0.5f, 1.0f, 1.0f);
				continue;
			}			
			image.color = new Color(1.0f, 1.0f, 1.0f);
		}				

		if(isShowedNextPanelAuto)
		{
			ShowNextPanel();
		}		
	}

	private void ResetPanel()
	{
		//Debug.Log("Call ResetCurrentItenm");
		for(int i=0; i<items.Length; i++)
		{
			Image image;
			image = items[i].GetComponent<Image>();				
			image.color = new Color(1.0f, 1.0f, 1.0f);
		}
	}
}
