using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class KeyBoardManipulator : TouchPadPanelManipulator {

	private GameObject currentItem = null;

	[SerializeField]
	private Text outputText;

	private Color[] PanelsDefaultColor;

	[SerializeField]
	private Color SelectedColor = new Color(0.5f, 1.0f, 1.0f);

	private void Awake()
	{	
		base.Awake();

		PanelsDefaultColor = new Color[Panels.Length];
		if(PanelsDefaultColor != null)
		{
			for(int i=0; i<PanelsDefaultColor.Length; i++)
			{
				var tempColor = Panels[i].GetComponent<Image>().color;
				PanelsDefaultColor[i] = tempColor;
			}
		}
	}

	private void SelectPanel(int index)
	{		
		ResetPanelColor();

		currentItem = Panels[index].gameObject;
		Image image;
		image = currentItem.GetComponent<Image>();
			
		if(image)
		{			
			image.color = SelectedColor;		
		}						
	}

	protected override void ClickOnPanel()
	{
		//ここでのitemはインプットイベントで文字を入力するパネルとなる
		if(!currentItem)
		{
			return;
		}

		var text = currentItem.GetComponentInChildren<Text>();
		outputText.text += text.text;
	} 

	private void OnEnable()
	{		
		base.OnEnable();	
	}

	private void OnDisable()
	{
		base.OnDisable();
		ResetPanelColor();
	}
	private void Update () {
		
		base.Update();

		if(currentAxis.x == 0 && currentAxis.y == 0)
		{				
			ResetPanelColor();			
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

	//パネルカラーをもとに戻す
	private void ResetPanelColor()
	{
		//Debug.Log("Call ResetCurrentItenm");
		if(Panels == null)
		{
			return;
		}

		for(int i=0; i<Panels.Length; i++)
		{
			Image image;
			image = Panels[i].GetComponent<Image>();
			if(image)
			{
				image.color = PanelsDefaultColor[i];
			}							
		}	
	}
}
