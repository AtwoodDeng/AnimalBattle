using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestory : MonoBehaviour {

	public float destoryDelay = 0;
	public bool isDestoryOnStart = true;

	void Start()
	{
		if (isDestoryOnStart) {
			DoSelfDestory ();
		}
	}

	public void DoSelfDestory()
	{
		Destroy (gameObject, destoryDelay);
		
	}

}
