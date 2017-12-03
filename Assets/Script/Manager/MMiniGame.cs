using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum MiniGameType
{
	OneButton,
	TwoRandomButton,
}

[System.Serializable]
public class MMiniGame {
	public MiniGameType MiniGameType {
		get { return m_type; }
	}

	[SerializeField][ReadOnly] MiniGameType m_type;

	virtual public void OnEnter()
	{
	}

	virtual public void OnEvent( LogicArg arg )
	{
	}

	/// <summary>
	/// Raises the update event.
	/// Return ture if is ready for next turn
	/// </summary>
	virtual public bool OnUpdate()
	{
		
		return false;
	}

	virtual public void OnExit()
	{
	}

}
