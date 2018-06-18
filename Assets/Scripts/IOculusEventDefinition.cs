using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
 interface IOculusEventDefinition {	 

	void CatchHittedInfo(RaycastHit info);

	void  TouchedPad();

	 void  ClickedPad();

	 void  UpFlicked();

	 void  DownFlicked();

	 void  LeftFlicked();

	 void  RightFlicked();

	 void  TriggerEntered();

	 void  GetUpTouchPad();

	 void Gaze();

	 void UnGaze();
}
