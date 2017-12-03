using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class ScoreController  {

	[SerializeField][ReadOnly] float m_score;

	public void AddScore( float s )
	{
		m_score += s;
	}

	public float GetScore( )
	{
		return m_score;
	}
}
