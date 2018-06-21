using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerTextPadController : MonoBehaviour {
 
	[SerializeField]
	private OculusGoControllerInfo touchPadAxisSource;

	[SerializeField]
	private GameObject[] items; //一つの円の中に入るパネルの数

	private Dictionary<int, Vector2> rotationRanges; //各パネルの領域(ある角度で決まる扇形の領域を指す)

	[SerializeField]
	private float radius = 1.0f;

	private int currentRangeKey = 0;

	// Use this for initialization
	void Start () {
		
		Vector2 rangeIncrement = new Vector2(0, 0);

		rotationRanges = new Dictionary<int, Vector2>(items.Length);
		
		for(int i=0; i<items.Length; i++)
		{			
			//Debug.Log("Search in rationRanges");			
			rangeIncrement = new Vector2(360/items.Length * i, 360/items.Length * (i+1));
			rotationRanges.Add(i, rangeIncrement);			
		}
		if(touchPadAxisSource == null)
		{
			touchPadAxisSource = FindObjectOfType<OculusGoControllerInfo>();
		}	
	}
	
	// Update is called once per frame
	void Update () {
		
		var currentAxis = touchPadAxisSource.PrimaryTouchpad;
		if(!CheckExistenceInCircle(currentAxis))
		{
			//Debug.Log("Not existed");
			return;
		}
		if(currentAxis.x == 0 && currentAxis.y == 0)
		{
			return;
		}

		var sita = GetSita(currentAxis);
		//Debug.LogFormat("sita : {0}", sita.ToString());
		
		var rangeKey = GetExistedRangeKey(sita);
		//Debug.LogFormat("rangeKey : {0}", rangeKey.ToString());
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

		currentRangeKey = rangeKey;
		ChangeItemColor(currentRangeKey);
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
		var result = Mathf.Pow(axis.x, 2) + Mathf.Pow(axis.y, 2);
		if(result <= radius)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private int GetExistedRangeKey(float sita)
	{
		foreach(var range in rotationRanges)
		{				
		 	//Debug.LogFormat("rotationRange : {0} - {1}", range.Value.x, range.Value.y);		
			// Debug.LogFormat("sita : {0}\n", sita);
			if((range.Value.x < sita) && (sita < range.Value.y))
			{			
				//Debug.LogFormat("Found Key {0}", range.Key);	
				return range.Key;
			}
		}

		//予期しないバグで領域内に角度が存在しなかったとき
		return -1;
	}

	private void ChangeItemColor(int key)
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
	}
}
