using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : UIElement {

	public ButtonType m_type;
}

public enum IconType
{
	None = 0 ,

	Sun = 2,

	Fire = 3,

	Dot = 4,

	Create = 5,

	Moon = 6,
}


public enum ButtonType
{
	None = 0 ,

	CreateButton = 10,
	ActionButton = 20,
	ResearchButton = 30,
	DisasterButton = 40,


	CreateWolf = 101,
	CreateDeer = 120,
	CreateFox = 140,
	CreateBoar = 160,

	CreateSweetBush = 301,
	CreateBasicTree = 310,

	CreateStone = 401,

	FeedNormalMeat = 601,

	FeedNormalGrass = 701,

	PlaySound = 901,
	PlayWind = 910,
	PlaySunShine = 920,

//	CreateButton = 50,
//	FeedButton = 55,
//	BattleButton = 60,
//
//
//
//	CreateWolf = 1,
//	CreateDeer = 5,
//
//	FeedSun = 101,
//	FeedMoon = 102,



}