using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElement : MBehavior {

	public virtual void OnEnter( bool isForce = true )
	{
	}

	public virtual void OnExit( bool isDestory = false)
	{
	}


	public virtual void PlayMoveFrom( Vector3 toPos , float duration = 1f , float delay = -1f )
	{
		
	}
}
