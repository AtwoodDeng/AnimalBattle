using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstFeed : MMiniGame {

	float bright;
	float dark;

	float count = 0;

	UIElement lastSun;
	UIElement lastMoon;

	public void GetRandomEdge( out int x , out int y )
	{
		int size = UIManager.Instance.GridSize;
		if (Random.Range (0, 1f) < .5f) {
			x = Random.Range (0, 1f) < .5f ? 0 : UIManager.Instance.GridSize - 1;
			y = Random.Range (0, size);
		} else {
			y = Random.Range (0, 1f) < .5f ? 0 : UIManager.Instance.GridSize - 1;
			x = Random.Range (0, size);
		}
	}

	public void GetGridPosition( out int sx , out int sy , out int mx , out int my )
	{
		int size = UIManager.Instance.GridSize;
		GetRandomEdge (out sx, out sy);
		mx = size - 1 - sx;
		my = size - 1 - sy;
	}


	public override void OnEnter ()
	{
		base.OnEnter ();

		UIElement fromEle = null;
		if (UIManager.Instance.GetElementCount () > 0)
			fromEle = UIManager.Instance.GetElements() [0];

		UIManager.Instance.ClearElement();

		int sx, sy, mx, my;

		GetGridPosition (out sx, out sy, out mx, out my);
		lastSun = UIManager.Instance.AddButton(ButtonType.FeedNormalMeat ,  sx , sy );
		lastMoon = UIManager.Instance.AddButton(ButtonType.FeedNormalGrass , mx ,my );

		if (fromEle != null) {
			UIManager.Instance.MoveFrom (fromEle, lastSun );
			UIManager.Instance.MoveFrom (fromEle, lastMoon);
		}
	}

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);

		if (arg.eventType == LogicEvents.ButtonPress) {
			var bArg = (ButtonArg)arg;
			if (bArg.buttonType == ButtonType.FeedNormalGrass) {
				dark += 1f;

				if (lastSun != null) {
					UIManager.Instance.RemoveElement (lastSun);
					lastSun = null;
				}

			} else if (bArg.buttonType == ButtonType.FeedNormalMeat ) {
				bright += 1f;

				if (lastMoon != null) {
					UIManager.Instance.RemoveElement (lastMoon);
					lastMoon = null;
				}
			}
//			count += 1f;
//
//			if (count > MGameManager.Instance.NormalTime * 0.5f ) {
//
//				if (bright + dark >= MGameManager.Instance.NormalTime * 2f) {
//					
//				} else {
//					UIManager.Instance.ClearElement ();
//
//					int sx, sy, mx, my;
//
//					GetGridPosition (out sx, out sy, out mx, out my);
//					var sun = UIManager.Instance.AddButton(ButtonType.FeedNormalMeat , IconType.Sun , sx , sy );
//					var moon = UIManager.Instance.AddButton(ButtonType.FeedNormalGrass , IconType.Moon , mx ,my );
//
//					if (lastSun != null)
//						UIManager.Instance.MoveFrom (lastSun, sun);
//					if (lastMoon != null)
//						UIManager.Instance.MoveFrom (lastMoon, moon);
//					lastSun = sun;
//					lastMoon = moon;
//
//					count = 0;
//				}
//			}
		}
	}

	public override bool OnUpdate ()
	{

		if (bright + dark >= MGameManager.Instance.NormalTime * 2f) {

			if (bright > dark)
				M_Event.FireLogicEvent (LogicEvents.FeedFeedBack, new FeedFeedbackArg (this, EnergyType.Brightness, FoodType.Meat , bright));
			else 
				M_Event.FireLogicEvent (LogicEvents.FeedFeedBack, new FeedFeedbackArg (this, EnergyType.Darkness, FoodType.Meat , dark));

			return true;
		}

		return false;
	}

	public override void OnExit ()
	{
		base.OnExit ();

	}
}
