using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxRunTime : CreatureInfoRunTime {

	public override bool IsCreatable ()
	{
		return MCreatureManager.Instance.GetCreatureCount (CreatureType.NormalDeer) >= 2
		&& MCreatureManager.Instance.GetCreatureCount (CreatureType.NormalWolf) >= 2;
		
	}
}
