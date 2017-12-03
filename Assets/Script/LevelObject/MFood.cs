using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public enum FoodType
{
	Normal,
	Meat,
	Grass,
}

public class MFood : MBehavior {

	public enum State
	{
		Enter,
		Normal,
		Destory,
	}
	[SerializeField][ReadOnly] protected State m_state;

	public FoodType m_type;
	public EnergyType m_energyType;
	[SerializeField] ParticleSystem ps;
	[ReadOnly]public Rigidbody m_rigidBody;
	[SerializeField] float duration = 30f;
	[SerializeField][ReadOnly] float timer = -1f;
	[SerializeField] float destoryDuration = 1f;
	[SerializeField] float energy = 10f;
	[SerializeField] float minEnergy = 0.1f;

	public static List<MFood> FoodList = new List<MFood>();

	public static MFood FindNearestFood ( FoodType type , Vector3 position )
	{
		if (FoodList == null || FoodList.Count <= 0)
			return null;
		
		float dis = 999999f;
		MFood food=null;

		foreach (var f in FoodList) {
			if (f.m_type == type) {
				var mDis = Global.GetDistance (position, f.transform.position);
				if ( mDis  < dis) {
					dis = mDis;
					food = f;
				}
			}
		}

		return food;
	}

	public float GetEnergy()
	{
		return energy;
	}

	public bool IsAviable()
	{
		return energy > minEnergy;
	}

	public float GrabEnergy( float partition )
	{
		if (ps != null) {
			var emission = ps.emission;
			emission.rateOverTimeMultiplier *= 1f - partition;
		}

		var useEnergy = partition * energy;

		energy -= useEnergy;

		return useEnergy;
	}

	protected override void MAwake ()
	{
		base.MAwake ();

		if (FoodList == null)
			FoodList = new List<MFood> ();
		FoodList.Add (this);

		m_state = State.Enter;
	}

	protected override void MStart ()
	{
		base.MStart ();

		if (ps != null) {
			var main = ps.main;
			main.startColor = WorldManager.Instance.GetEnergyColor (m_energyType);
		}
		m_rigidBody = GetComponent<Rigidbody> ();
	}


	public void OnCollisionEnter( Collision col )
	{
		if (col.collider.gameObject.layer == LayerMask.NameToLayer("Floor") ) {
			OnTouchFloor ();
		}

	}

	protected virtual void OnTouchFloor()
	{
		M_Event.FireLogicEvent (LogicEvents.FoodReady, new FoodReadyArg (this, this));
		m_state = State.Normal;
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();

		if (m_state == State.Normal) {
				timer += Time.deltaTime;

			if (timer > duration) {
				SelfDestory ();
			}

			if (energy < minEnergy)
				SelfDestory ();
		}
	}

	virtual protected void SelfDestory()
	{
		if (m_state != State.Destory) {
			m_rigidBody.isKinematic = true;
			transform.DOMoveY (-1f, destoryDuration).OnComplete (delegate {
				Destroy (gameObject , 1f);
				FoodList.Remove (this);
			});
			if (ps != null) {
				ps.Stop ();
			}
			m_state = State.Destory;
		}
	}



}
