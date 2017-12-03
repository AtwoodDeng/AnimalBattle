using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MGameManager : MBehavior {

	static private MGameManager m_Instance;
	static public MGameManager Instance {
		get {
			if (m_Instance == null)
				m_Instance = FindObjectOfType<MGameManager> ();
			return m_Instance;
		}
	}

	public enum State
	{
		None,
		MainState,
		Create,
		CreateMiniGame,
		CreateFeedback,
		Action,
		Play,
		Defend,
	}

	AStateMachine<State,LogicEvents> m_stateMachine = new AStateMachine<State, LogicEvents>(State.None);
	[SerializeField][ReadOnly] State m_state;

	#region TURN
	[SerializeField][ReadOnly] int m_turn = 1;
	[SerializeField] float turnMuplifitation = 10f;

	public void AdvanceTurn()
	{
		m_turn++;
	}

	public int Turn {
		get { return m_turn; }
	}

	public float NormalTime
	{
		get { 
			return ( Mathf.Log10( m_turn * 1f + 10f ) ) * turnMuplifitation;
		}
	}

	#endregion

	#region MINI_GAME

	[SerializeField][ReadOnly] MMiniGame m_miniGame;


	#endregion

	protected override void MAwake ()
	{
		base.MAwake ();
		InitStateMachine ();
	}

	private UIButton buttonCache;
	private ButtonType buttonTypeCache;

	public void InitStateMachine()
	{
//		m_stateMachine.AddEnter (State.Create, delegate {
//			UIManager.Instance.ClearElement();
//			UIManager.Instance.AddButton(ButtonType.CreateWolf , IconType.Create , 1, 1);
//		});
//
//		m_stateMachine.AddExit (State.Create, delegate {
//			AdvanceTurn ();
//		});
//
//		m_stateMachine.AddEnter (State.Feed, delegate {
//			UIManager.Instance.ClearElement();
//
//			UIManager.Instance.AddButtonAtRandomPosition(ButtonType.FeedSun , IconType.Sun );
//			UIManager.Instance.AddButtonAtRandomPosition(ButtonType.FeedMoon , IconType.Moon );
//		});
//
//		m_stateMachine.AddOnEvent (State.Feed, delegate(object arg) {
//			var lArg = (LogicArg)arg;
//			if ( lArg != null && lArg.eventType == LogicEvents.ButtonPress ) {
//				var bpArg = (ButtonArg)arg;
//				UIManager.Instance.ClearElement();
//				UIManager.Instance.AddButtonAtRandomPosition(ButtonType.FeedSun , IconType.Sun );
//				UIManager.Instance.AddButtonAtRandomPosition(ButtonType.FeedMoon , IconType.Moon );
//			}
//			
//		});
//
//		m_stateMachine.AddExit (State.Feed, delegate {
//			AdvanceTurn();	
//		});

		m_stateMachine.AddEnter (State.MainState, delegate {
			UIManager.Instance.AddButton(ButtonType.CreateButton , 1 , 1 );
			if ( MCreatureManager.Instance.GetAnimalCount() > 0 )
				UIManager.Instance.AddButton(ButtonType.ActionButton , 2 , 1 );
			if ( DisasterManager.Instance.IsDisasterReady )
			{
				UIManager.Instance.AddButton(ButtonType.DisasterButton , 0 , 1 );
			}
		});	

		m_stateMachine.AddOnEvent (State.MainState, delegate(object arg) {

			var lArg = (LogicArg)arg;
			if ( lArg.eventType == LogicEvents.ButtonPress ) {
				var bArg = (ButtonArg)lArg;
				if ( bArg.buttonType == ButtonType.CreateButton )
				{
					buttonCache = bArg.button;
					m_stateMachine.State = State.Create;
				}
			}
		});


		///// Create 

		m_stateMachine.AddEnter (State.Create, delegate {
//			if ( Turn == 1 )
			{
				// Basic Creature
				var list = GetCreatureCreateButtonList();
				foreach ( var btnType in list )
				{
					var btn = UIManager.Instance.AddButtonAtRandomPosition( btnType );
					if ( buttonCache != null )
						UIManager.Instance.MoveFrom( buttonCache , btn , 1f );
				}

				if ( buttonCache != null ) {
					UIManager.Instance.RemoveElement( buttonCache );
				}
			}

			// m_miniGame.OnEnter();
		});

		m_stateMachine.AddOnEvent (State.Create, delegate(object arg) {
			
			var lArg = (LogicArg)arg;
			if ( lArg.eventType == LogicEvents.ButtonPress ) {
				var bArg = (ButtonArg)lArg;

				buttonTypeCache = bArg.buttonType;
				m_stateMachine.State = State.CreateMiniGame;
			}
		});

		m_stateMachine.AddEnter (State.CreateMiniGame, delegate {
			m_miniGame = new FirstCreate();

			m_miniGame.OnEnter();
		});

		m_stateMachine.AddUpdate (State.CreateMiniGame, delegate {

			if ( m_miniGame.OnUpdate() )
			{
				m_stateMachine.State = State.Action;
			}
		});
			
		m_stateMachine.AddOnEvent (State.CreateMiniGame, delegate(object arg) {

			var lArg = (LogicArg)arg;
			if ( lArg.eventType == LogicEvents.ButtonPress ) {
				var bArg = (ButtonArg)lArg;
			}

			if ( m_miniGame != null )
			{
				m_miniGame.OnEvent( (LogicArg) arg );	
			}
		});

		m_stateMachine.AddExit (State.CreateMiniGame, delegate {
			m_miniGame.OnExit();
			m_miniGame = null;
			AdvanceTurn();
		});

		/////// Feed /////////

		m_stateMachine.AddEnter (State.Action, delegate {
//			if ( Turn == 2 )
			{
				m_miniGame = new FirstFeed();
			}

			m_miniGame.OnEnter();
		});

		m_stateMachine.AddUpdate (State.Action, delegate {
			if ( m_miniGame.OnUpdate() )
			{
				m_stateMachine.State = State.Create;
			}
		});

		m_stateMachine.AddOnEvent (State.Action, delegate(object arg) {

			if ( m_miniGame != null )
			{
				m_miniGame.OnEvent( (LogicArg) arg );	
			}
		});

		m_stateMachine.AddExit (State.Action, delegate {
			m_miniGame.OnExit();
			m_miniGame = null;
			AdvanceTurn();
		});

		/////// Play /////////

		m_stateMachine.AddEnter (State.Play, delegate {
//			if ( Turn == 1 )
			{
				m_miniGame = new NormalPlay();
			}

			m_miniGame.OnEnter();
		});

		m_stateMachine.AddUpdate (State.Play, delegate {
			if ( m_miniGame.OnUpdate() )
			{
				m_stateMachine.State = State.Action;
			}
		});

		m_stateMachine.AddOnEvent (State.Play, delegate(object arg) {

			if ( m_miniGame != null )
			{
				m_miniGame.OnEvent( (LogicArg) arg );	
			}
		});

		m_stateMachine.AddExit (State.Play, delegate {
			m_miniGame.OnExit();	
			m_miniGame = null;
			AdvanceTurn();
		});

		m_stateMachine.State = State.MainState;
	}

	public void AddOnChange( AStateMachine<State,LogicEvents>.StateChangeHandler handler )
	{
		m_stateMachine.AddOnChange (handler);
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();
		m_stateMachine.Update ();
		m_state = m_stateMachine.State;
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();

		M_Event.RegisterAll (OnEvent);
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnRegisterAll(OnEvent);
	}

	public void OnEvent( LogicArg arg )
	{
		
		m_stateMachine.OnEvent (arg);
	}

	public List<ButtonType> GetCreatureCreateButtonList()
	{
		List<ButtonType> res = new List<ButtonType> ();

		foreach (var ct in MCreatureManager.CreatureLists) {
			var info = MCreatureManager.Instance.GetCreatureInfo (ct);
			if (info.IsCreatable ()) {
				Debug.Log ("Add " + info.data.name);
				res.Add (info.data.CreateButtonType);
			}
		}

		while (res.Count > WorldManager.Instance.WorldLevel + 2) {
			res.RemoveAt (Random.Range (0, res.Count));
		}

		return res;
	}
}
