using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Raycast時に、OculusGoInputに登録するイベント
 interface IOculusRaycastEventDefinition {	 

	 //次の同じインプットイベントのインターバルを決める
	 float Interval
	 {
		 get;
	 }

	//自分がレイキャストされ続けてるときに呼ばれるイベント(infoはレイキャスヒット時の自身の情報)
	void CatchHittedInfo(RaycastHit info);

	void TouchedPad();

	 void ClickedPad();

	 void UpFlicked();

	 void DownFlicked();

	 void LeftFlicked();

	 void RightFlicked();

	 void TriggerEntered();

	 void GetUpTouchPad();

	 void Gaze();

	 void UnGaze();

	//自分からレイキャスが外れたときに、違うオブジェクトが続けてレイキャスされたときに呼ばれるイベント
	 void GetNextHittedObj(RaycastHit nextObjInfo);
}
