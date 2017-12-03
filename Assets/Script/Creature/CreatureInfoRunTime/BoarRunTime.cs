using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarRunTime : CreatureInfoRunTime {

	public override bool IsCreatable ()
	{
		return MCreatureManager.Instance.GetCreatureCount (CreatureType.NormalDeer) >= 3
			&& WorldManager.Instance.Score >= 500;
	}
}
