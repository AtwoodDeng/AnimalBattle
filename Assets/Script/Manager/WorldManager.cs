using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WorldManager : MBehavior {

	static private WorldManager m_Instance;
	static public WorldManager Instance {
		get {
			if (m_Instance == null)
				m_Instance = FindObjectOfType<WorldManager> ();
			return m_Instance;
		}
	}

	[System.Serializable]
	public class CreatureInfo{
		public EnergyType EnergyType;
		public FoodType FoodType;
		public GameObject creaturePrefab;
		public MinMax angle = new MinMax(0 , 360f);
		public float height = -1f;
		public float score = 10f;
		public Color col = new Color(1f,1f,1f,1f) ;
		public GameObject effect;
	}

	[System.Serializable]
	public class ActionScore{
		public AnimalAction action;
		public float score = 1f;
	}

	[TabGroup("Data", true )]
	public List<CreatureInfo> createInfoList = new List<CreatureInfo> ();
	[TabGroup("Data", true )]
	public List<CreatureInfo> feedInfoList = new List<CreatureInfo> ();

//	[TabGroup("Data", true )]
//	public EnergyController energyController;
	[TabGroup("Data", true )]
	[ReadOnly] public ScoreController scoreController = new ScoreController();
	[TabGroup("Data", true )]
	public List<ActionScore> actionScoreList = new List<ActionScore> ();
	[TabGroup("Effect", true )]
	public List<GameObject> actionScoreEffect;
	[TabGroup("Effect", true )]
	public GameObject clickEffect;
	[TabGroup("Effect", true )]
	public GameObject createEffect;
	[TabGroup("Effect", true )]
	public GameObject feedEffect;
	[System.Serializable]
	public class EnergyInfo{
		public EnergyType type;
		public Color color;
	}
	[TabGroup("Data", true )]
	public List<EnergyInfo> m_energyInfos;


	[TabGroup("Create", true  )]
	public Transform root;

	[TabGroup("Create", true  )]
	public float range;

	public float Score {
		get {
			return scoreController.GetScore ();
		}
	}

	public int WorldLevel {
		get {
			return 1;
		}
	}



	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterEvent(LogicEvents.ButtonPress , OnButtonPress );
		M_Event.RegisterEvent(LogicEvents.CreateFeedback , OnCreateFeedback );
		M_Event.RegisterEvent(LogicEvents.FeedFeedBack , OnFeedFeedback );
		M_Event.RegisterEvent(LogicEvents.PlayFeedback , OnCreateFeedback );
		M_Event.RegisterEvent(LogicEvents.AnimalAction , OnAnimalAction );
	}

	protected override void MOnDisable ()
	{
		base.MOnEnable ();
		M_Event.UnregisterEvent(LogicEvents.ButtonPress , OnButtonPress );
		M_Event.UnregisterEvent(LogicEvents.CreateFeedback , OnCreateFeedback );
		M_Event.UnregisterEvent(LogicEvents.FeedFeedBack , OnFeedFeedback );
		M_Event.UnregisterEvent(LogicEvents.PlayFeedback , OnCreateFeedback );
		M_Event.UnregisterEvent(LogicEvents.AnimalAction , OnAnimalAction );
	}

	protected override void MStart ()
	{
		base.MStart ();
//		energyController.Init ();
	}

	public void OnAnimalAction( LogicArg arg )
	{
		AnimalActionArg aaArg = (AnimalActionArg)arg;

		foreach (var data in actionScoreList) {
			if (aaArg.animalAction == data.action) {
				scoreController.AddScore (data.score);
				PlayScoreEffectOn (aaArg.trans , data.score );
			}
		}

	}

	public void PlayScoreEffectOn( Transform trans , float score )
	{
		if ( actionScoreEffect != null && actionScoreEffect.Count > 0 )
		{
			var effectPrefab = actionScoreEffect [Random.Range (0, actionScoreEffect.Count)];
//			var effect = Instantiate (effectPrefab, trans) as GameObject;
//			effect.transform.localPosition = Vector3.zero;
			CreateEffectOn( effectPrefab ,  trans.position , trans , Color.yellow , score / 10f );
		}
	}

	public Color GetEnergyColor( EnergyType _type )
	{
		if (m_energyInfos != null) {
			foreach (var info in m_energyInfos) {
				if (info.type == _type)
					return info.color;
			}
		}
		return Color.white;
	}

	public void OnButtonPress( LogicArg arg )
	{
		ButtonArg bArg = (ButtonArg)arg;

//		if (bArg.buttonType == ButtonType.CreateWolf)
//			energyController.AddEnergy (EnergyType.CreateWolf, 1f);
//		else if (bArg.buttonType == ButtonType.FeedSun)
//			energyController.AddEnergy (EnergyType.Brightness, 1f);
//		else if (bArg.buttonType == ButtonType.FeedMoon)
//			energyController.AddEnergy (EnergyType.Darkness, 1f);
		scoreController.AddScore(1f);

		Color col = Color.white;

//		if (bArg.buttonType == ButtonType.FeedMoon)
//			col = GetEnergyColor (EnergyType.Brightness);
//		else if (bArg.buttonType == ButtonType.FeedSun)
//			col = GetEnergyColor (EnergyType.Darkness);

		CreateEffectOn (clickEffect, GetRandomPosition () , null , col );
	}

	public void OnCreateFeedback( LogicArg arg )
	{
		CreateFeedbackArg eArg = (CreateFeedbackArg)arg;

		if (arg.eventType == LogicEvents.CreateFeedback) {

			foreach (var info in createInfoList)
				if (info.EnergyType == eArg.energyType) {
					var obj = Create (info, createEffect);
					scoreController.AddScore (info.score);
					M_Event.FireLogicEvent (LogicEvents.CreateAnimal, new CreateArg (this, obj));
					break;
				}
		}
	}

	public void OnFeedFeedback ( LogicArg arg ) {
		FeedFeedbackArg fArg = (FeedFeedbackArg)arg;
		if (arg.eventType == LogicEvents.FeedFeedBack) {
			foreach (var info in feedInfoList )
				if (info.FoodType == fArg.foodType && info.EnergyType == fArg.energyType ) {
					var obj = Create (info , feedEffect);
					scoreController.AddScore (info.score);
					M_Event.FireLogicEvent (LogicEvents.CreateFood, new CreateArg (this, obj));
					break;
				}
		}

	}

	public void OnPlayFeedback( LogicArg arg )
	{

	}

	public GameObject Create( CreatureInfo info , GameObject effect = null )
	{
		var pos = GetRandomPosition ();

		if (info.height > 0)
			pos.y = root.position.y + info.height;
		
		var obj = Instantiate (info.creaturePrefab, root) as GameObject;
		obj.transform.position = pos;
		obj.transform.eulerAngles = new Vector3 (0, info.angle.rand, 0);

		if (info.effect != null)
			CreateEffectOn (info.effect, pos, obj.transform , info.col);
		else if ( effect != null )
			CreateEffectOn (effect, pos, obj.transform, info.col);


		return obj;
	}

	public void CreateClickEffectOn( Vector3 pos , Color? _col = null , float value = 1f )
	{
		CreateEffectOn (clickEffect, pos , root , _col?? Color.white , value );
	}

	private void CreateEffectOn( GameObject _effect , Vector3 _position , Transform _parent = null  , Color? _col = null  , float _value = 1f)
	{
		
		if ( _effect != null  )
		{
			var parent = _parent?? root;

			var effObj = Instantiate (_effect, parent) as GameObject;
			effObj.transform.position = _position;
			var effectCom = effObj.GetComponent<MParticleEffect> ();
			if (effectCom != null) {
				effectCom.SetColor (_col ?? Color.white);
				effectCom.SetValue (_value);
			}
		}
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();
//		energyController.Update ();
	}

	public Vector3 GetRandomPosition()
	{
		Vector3 pos = Random.onUnitSphere * range + root.position;
		pos.y = 100 ;

		RaycastHit hitInfo;

		if (Physics.Raycast (pos, Vector3.down, out hitInfo, 1000f, LayerMask.GetMask ("Floor"))) {
			pos.y = hitInfo.point.y;
		}

		return pos;
		
	}
}
