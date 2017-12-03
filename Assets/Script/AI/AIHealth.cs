using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class AIHealth {

	public float hungry = 0.4f;
	public float Size = 10f;
	public float eatSpeed = 0.2f;

	Dictionary<EnergyType , float> m_energyDict = new Dictionary<EnergyType, float>();

	public bool IsHungry {
		get { return hungry < .4f; }
	}

	public void Update() {
		hungry -= 1f / 25f / Size * Time.deltaTime;
	}

	public void EatFood (  MFood food )
	{
		float par = eatSpeed * Time.deltaTime;
		float energy = food.GrabEnergy (par);
		hungry += energy / Size;

		if (m_energyDict.ContainsKey (food.m_energyType)) {
			m_energyDict [food.m_energyType] += energy;
		} else {
			m_energyDict [food.m_energyType] = energy;
		}

	}

	public bool IsFull {
		get {
			return hungry >= 1f;
		}
	}
}
