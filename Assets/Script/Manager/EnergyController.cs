using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public enum EnergyType
{
	CreateWolf = 1,
	CreateDeer = 5,
	Brightness = 100,
	Darkness = 110,
	Fire = 120,
	Water = 130,
	Tree = 140,
	Rock = 150,
	Wind = 160,
}


[System.Serializable]
public class EnergyController  {

//	[HideInInspector]
//	public EnergyType[] energyTypeList = {
//		EnergyType.CreateWolf,
//		EnergyType.Brightness,
//		EnergyType.Darkness,
//	};
//
//	[System.Serializable]
//	public class RunTimeEnergy {
//		public EnergyType type;
//		public float energy;
//		public void AddEnergy ( float add )
//		{
//			energy += add;
//		}
//	}
//	[SerializeField][ReadOnly] List<RunTimeEnergy> m_energyList = new List<RunTimeEnergy>();
//
//	[SerializeField][ReadOnly] private float m_score;
//
//	public float GetScore()
//	{
//		return m_score;
//	}
//
//	public EnergyController()
//	{
//		foreach (var t in energyTypeList) {
//			var data = new RunTimeEnergy ();
//			data.type = t;
//			data.energy = 0;
//			m_energyList.Add (data);
//		}
//	}
//
//	public void AddEnergy( EnergyType type , float _add = 1f  ) {
//		foreach (var v in m_energyList) {
//			if ( v.type == type )
//				v.AddEnergy (_add);
//		}
//
//		AddScore (_add);
//	}
//
//	public void AddScore( float _add )
//	{
//		m_score += _add;
//	}
//
//	public RunTimeEnergy GetEnergyInfo( EnergyType type )
//	{
//		foreach (var i in m_energyList) {
//			if (i.type == type)
//				return i;
//		}
//		return null;
//	}
//
//
//	public void Update () {
//
//		var b = GetEnergyInfo (EnergyType.Brightness);
//		var d = GetEnergyInfo (EnergyType.Darkness);
//		var threshod = MGameManager.Instance.NormalTime;
//
//		if ( b.energy + d.energy > threshod ) {
//			if (b.energy > d.energy) {
//				M_Event.FireLogicEvent (LogicEvents.EnergyBurst, new EnergyBurstArg( this , b.type, b.energy));
//			} else {
//				M_Event.FireLogicEvent (LogicEvents.EnergyBurst, new EnergyBurstArg( this , d.type, d.energy));
//			}
//
//			b.energy = 0;
//			d.energy = 0;
//		}
//
//		foreach (var v in m_energyList) {
//			if (v.energy >= threshod ) {
//							M_Event.FireLogicEvent (LogicEvents.EnergyBurst, new EnergyBurstArg (v, v.type, v.energy));
//				v.energy = 0;
//				return;
//			}
//		}
//
//
// 	}
}

	
