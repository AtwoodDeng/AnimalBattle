using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfRunTime : CreatureInfoRunTime {

	public override bool IsCreatable ()
	{
		return MCreatureManager.Instance.GetCreatureCount (CreatureType.NormalDeer) >= 1;
	}
}
