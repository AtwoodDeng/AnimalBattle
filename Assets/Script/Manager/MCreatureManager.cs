using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MCreatureManager : MBehavior {

	static private MCreatureManager m_Instance;
	static public MCreatureManager Instance {
		get {
			if (m_Instance == null)
				m_Instance = FindObjectOfType<MCreatureManager> ();
			return m_Instance;
		}
	}

	[SerializeField][ReadOnly] List<MCreature> m_CreatureList = new List<MCreature>();

	[SerializeField][ReadOnly] List<CreatureInfo> m_creatureInfoList = new List<CreatureInfo>();


	public void InitInfoList()
	{
		if (m_creatureInfoList == null || m_creatureInfoList.Count <= 0) {
			string path = "Creature/Info";
			m_creatureInfoList = new List<CreatureInfo> ();
			m_creatureInfoList.AddRange( Resources.LoadAll<CreatureInfo> (path) );
		}
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterEvent (LogicEvents.CreateAnimal, OnCreateAnimal);
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnregisterEvent (LogicEvents.CreateAnimal, OnCreateAnimal);
	}

	public void OnCreateAnimal(LogicArg arg )
	{
		var cArg = (CreateArg)arg;
		var myCreature = cArg.obj.GetComponent<MCreature> ();

		if (myCreature != null) {
			myCreature.Init ();
			m_CreatureList.Add (myCreature);
		}
	}

	public int GetAnimalCount()
	{
		int count = 0;
		foreach (var c in m_CreatureList)
			if (c.IsAnimal)
				count++;
		return count;
	}

	public int GetCreatureCount( CreatureType type )
	{
		int count = 0;
		foreach (var c in m_CreatureList)
			if (c.MCreatureType == type )
				count++;
		return count;
	}

	public static CreatureType[] CreatureLists = {
		CreatureType.NormalDeer,
		CreatureType.NormalWolf,
		CreatureType.NormalFox,
		CreatureType.NormalBoar,
		CreatureType.SweatBush,
		CreatureType.Tree,
	};


	public CreatureInfoRunTime GetCreatureInfo( CreatureType type )
	{
		InitInfoList ();

		var info = m_creatureInfoList.Find (x => x.type == type);
		CreatureInfoRunTime res;

		switch (type) {
		case CreatureType.NormalDeer:
			res =  new DeerRuntime ();
			break;
		case CreatureType.NormalWolf:
			res =  new WolfRunTime ();
			break;
		case CreatureType.NormalFox:
			res =  new FoxRunTime ();
			break;
		case CreatureType.NormalBoar:
			res =  new BoarRunTime ();
			break;
		case CreatureType.Tree:
			res =  new BasicTreeRunTime ();
			break;
		case CreatureType.SweatBush:
			res =  new SweetBushRunTime ();
			break;
		default :
			res = new CreatureInfoRunTime ();
			break;
		};

		res.Init (info);

		return res;
	}
}
