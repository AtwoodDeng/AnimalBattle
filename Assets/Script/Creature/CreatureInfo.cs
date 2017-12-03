using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[CreateAssetMenu(fileName = "CreatureInfo" , menuName = "AB/CreatureInfo" )]
public class CreatureInfo : ScriptableObject {

	public CreatureType type;
	public string name;

	public ButtonType CreateButtonType;

	public bool IsAnimal;
	public bool IsPlant;

	[ShowIf("IsAnimal")]
	public bool EatMeat;
	[ShowIf("IsAnimal")]
	public bool EatPlant;
}
