using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweetBushRunTime : CreatureInfoRunTime {

	public override bool IsCreatable ()
	{
		
		return WorldManager.Instance.Score >= 1000;
	}
}
