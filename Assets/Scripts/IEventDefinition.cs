using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventDefinition  {

	void AttachedEvents();
	void DetachedEvents();
	void CatchHittedInfo(RaycastHit info);
}
