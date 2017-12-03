using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTreeRunTime : CreatureInfoRunTime {

	public override bool IsCreatable ()
	{
		return WorldManager.Instance.Score >= 2000;
	}

}
