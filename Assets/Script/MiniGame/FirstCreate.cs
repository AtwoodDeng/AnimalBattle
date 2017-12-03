using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstCreate : MMiniGame {

	float timeCounter = 0 ;

	public override void OnEnter ()
	{
		base.OnEnter ();

		UIManager.Instance.ClearElement ();
		var deerBtn = UIManager.Instance.AddButtonAtRandomPosition (ButtonType.CreateDeer);
		UIManager.Instance.MoveFrom ( deerBtn , 1 , 1 );
		UIManager.Instance.AddButtonAtRandomPosition (ButtonType.CreateWolf);
		UIManager.Instance.MoveFrom ( deerBtn , 1 , 1 );
		timeCounter = 0;
	}

	public override void OnEvent (LogicArg arg)
	{
		base.OnEvent (arg);

		if (arg.eventType == LogicEvents.ButtonPress) {
			var bArg = (ButtonArg)arg;
			if (bArg.buttonType == ButtonType.CreateDeer) {
				timeCounter += 1f;
			}
		}

	}

	public override bool OnUpdate ()
	{
		if (timeCounter > MGameManager.Instance.NormalTime) {

			M_Event.FireLogicEvent (LogicEvents.CreateFeedback, new EnergyBurstArg (this, EnergyType.CreateDeer, timeCounter));

			return true;
		}
		return false;
	}

	public override void OnExit ()
	{
		base.OnExit ();
	}
}
