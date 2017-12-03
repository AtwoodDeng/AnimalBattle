using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisasterManager : MBehavior {

	static private DisasterManager m_Instance;
	static public DisasterManager Instance {
		get {
			if (m_Instance == null)
				m_Instance = FindObjectOfType<DisasterManager> ();
			return m_Instance;
		}
	}

	public bool IsDisasterReady
	{
		get {
			return false;
		}
	}
}
