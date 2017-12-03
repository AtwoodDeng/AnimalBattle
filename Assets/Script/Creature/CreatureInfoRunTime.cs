using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureInfoRunTime {

	virtual public void Init( CreatureInfo _info ) {
		data = _info;
	}

	public CreatureInfo data;

	public virtual bool IsCreatable() {
		return true;
	}

}
