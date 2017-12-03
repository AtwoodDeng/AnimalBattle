using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum CreatureType {
	None = 0 ,


	NormalWolf = 1,
	NormalDeer = 20,
	NormalFox = 40,
	NormalBoar = 60,

	SweatBush = 201,
	Tree = 301,

}

public class MCreature : MonoBehaviour {

	[SerializeField]private CreatureType m_CreatureType;
	public CreatureType MCreatureType { get { return m_CreatureType; } } 
	[SerializeField][ReadOnly] CreatureInfoRunTime m_info;

	public bool IsAnimal {
		get {
			return m_info.data.IsAnimal;
		}
	}

	public bool IsPlant {
		get {
			return m_info.data.IsPlant;
		}
	}

	public void Init(  )
	{
		m_info = MCreatureManager.Instance.GetCreatureInfo( MCreatureType ) ;
	}
}
