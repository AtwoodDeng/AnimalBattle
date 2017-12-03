using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MalbersAnimations;
using Sirenix.OdinInspector;



public enum AnimalAction
{
	Eat = 2,
	Drink = 7,
	Sleep =6 ,
	Seat = 8,
	CrawlUnder = 9,
	Dig = 10,
	Lie = 11,

	Walk = 100,
	Rest = 110,

}

public class M_InputAI : MBehavior {

	[ReadOnly] Animal m_animal;                    //The Animal Script
	public Animal Animal
	{
		get
		{
			if (m_animal == null)
			{
				m_animal = GetComponentInChildren<Animal>();
			}
			return m_animal;
		}
	}

	[ReadOnly] NavMeshAgent m_agent;                    //The Animal Script
	public NavMeshAgent Agent
	{
		get
		{
			if (m_agent == null)
			{
				m_agent = GetComponentInChildren<NavMeshAgent>();
			}
			return m_agent;
		}
	}

	public enum State
	{
		None,
		Walk,
		Rest,
		Lie,
		Enter,
		FindFood,
		WalkToFood,
		Eat,
	}

	AStateMachine<State,LogicEvents> m_stateMachine = new AStateMachine<State, LogicEvents>(State.None);
	[ReadOnly] public State m_state;
	[ReadOnly] public Vector3 target;
	[SerializeField][ReadOnly] float timer;
	[SerializeField] MinMax walkChangeDirectionTimer = new MinMax(5f ,10f);
	[SerializeField] float stopDistance = 1.2f;
	[SerializeField] List<FoodType> aviableFoodList = new List<FoodType>();

	[SerializeField][ReadOnly] MFood targetFood;

	[SerializeField] AIHealth m_health = new AIHealth();

	protected override void MStart ()
	{
		base.MStart ();

		if (Agent != null) {
			Agent.updatePosition = false;
			Agent.updateRotation = false;
			Agent.speed = 0.0001f;
			Agent.stoppingDistance = stopDistance;
		}

		InitStateMachine ();
	}

	public void InitStateMachine()
	{
		m_stateMachine.AddEnter (State.Enter, delegate {
			ResetTimer();
			Animal.ActionEmotion( -1);
			Animal.Action = true;
		});

		m_stateMachine.AddUpdate (State.Enter, delegate {
			if ( timer < 0 )
				m_stateMachine.State = State.Walk;
		});

		m_stateMachine.AddExit (State.Enter, delegate {
			Animal.Action = false; 	
		});


		m_stateMachine.AddEnter (State.Walk, delegate {
			SetRandomTarget();
			ResetTimer();
			M_Event.FireLogicEvent(LogicEvents.AnimalAction , new AnimalActionArg(this , AnimalAction.Walk, transform));

			if ( m_health.IsHungry )
				m_stateMachine.State = State.FindFood;
		});

		m_stateMachine.AddUpdate (State.Walk, delegate {

			Agent.nextPosition = transform.position;

			if ( Agent.remainingDistance > Agent.stoppingDistance )
			{
				if ( timer < -5f ) {
					SetRandomTarget();
					ResetTimer();
				}

				Vector3 walkDir = Agent.desiredVelocity.normalized;
				walkDir.y = 0;
				if ( walkDir.magnitude < 0.01f )
				{
					walkDir = transform.forward;
					walkDir.y = 0;
				}
				walkDir = walkDir.normalized;

				Debug.DrawLine( transform.position , transform.position + walkDir * 5f , Color.red );
				Animal.Move( walkDir , true );
			} else {

				if ( Random.Range( 0 , 1f ) < 0.2f )
					m_stateMachine.State = State.Lie;
				else 
					m_stateMachine.State = State.Rest;
			}

		});

		m_stateMachine.AddExit (State.Walk, delegate {
		});

		m_stateMachine.AddEnter (State.Rest, delegate {
			Animal.Move(Vector3.zero , false);
		});

		m_stateMachine.AddUpdate (State.Rest, delegate {
			if (IsTimerReady() )
				m_stateMachine.State = State.Walk;
		});

		m_stateMachine.AddExit (State.Rest, delegate {
		});

		m_stateMachine.AddEnter (State.Lie, delegate {
			Animal.ActionEmotion((int)AnimalAction.Lie);
			Animal.Action = true;
			ResetTimer();
			Animal.Move(Vector3.zero , false);

			M_Event.FireLogicEvent(LogicEvents.AnimalAction , new AnimalActionArg(this , AnimalAction.Lie, transform));
		});

		m_stateMachine.AddUpdate (State.Lie, delegate {
			Animal.Action = false;
			if ( IsTimerReady() && Animal.Stand )
			{
				m_stateMachine.State = State.Walk;
			}
		});

		m_stateMachine.AddExit (State.Lie, delegate {
			Animal.ActionEmotion(-1);
			Animal.Action = false;
			Animal.Move(new Vector3(0.001f , 0 , 0.001f) , false);
		});

		m_stateMachine.AddEnter (State.FindFood, delegate {

			Debug.Log("Find Food ");
			if ( targetFood == null ) {
				foreach ( var aviableFood in aviableFoodList ) {
					targetFood = MFood.FindNearestFood( aviableFood , transform.position );
					if ( targetFood != null )
						break;
				}
			}

			if ( targetFood == null ) {
				ResetTimer();
				m_stateMachine.State = State.Rest;
			} else {
				m_stateMachine.State = State.WalkToFood;
			}
		});

		m_stateMachine.AddEnter (State.WalkToFood, delegate {
			Animal.ActionEmotion(-1);

			Debug.Log("Walk To Food ");
			if ( targetFood == null )
				m_state = State.FindFood;
			else { 
				Agent.SetDestination( targetFood.transform.position );

				var lookAt = gameObject.GetComponent<MalbersAnimations.Utilities.LookAt>();
				if ( lookAt != null )
					lookAt.Target = targetFood.transform;
			}

		});

		m_stateMachine.AddUpdate (State.WalkToFood, delegate {
			Agent.nextPosition = transform.position;

			if ( targetFood == null || !targetFood.IsAviable()  )
			{
				m_stateMachine.State = State.FindFood;

			} else {
				Vector3 toward = targetFood.transform.position - transform.position;
				toward.y = 0;
				float towardAngle = Vector3.Angle( transform.forward , toward );

//				Debug.Log("Re D " + Agent.remainingDistance + " stop " + Agent.stoppingDistance );
				if ( Agent.remainingDistance > Agent.stoppingDistance  )
				{
					Vector3 walkDir = Agent.desiredVelocity.normalized;
					walkDir.y = 0;
					if ( walkDir.magnitude < 0.01f )
					{
						walkDir = transform.forward;
						walkDir.y = 0;
					}
					walkDir = walkDir.normalized;

					Debug.DrawLine( transform.position , transform.position + walkDir * 5f , Color.red );
					Animal.Move( walkDir , true );

					if ( toward.magnitude > 5f )
						Animal.Shift = true;
					else 
						Animal.Shift = false;

				} else {

					m_stateMachine.State = State.Eat;
				}
			}
		});

		m_stateMachine.AddExit (State.WalkToFood, delegate {

			Animal.Shift = false;

			var lookAt = gameObject.GetComponent<MalbersAnimations.Utilities.LookAt>();
			if ( lookAt != null )
				lookAt.Target = null;
		});

		m_stateMachine.AddEnter (State.Eat, delegate {
			Animal.ActionEmotion( (int)AnimalAction.Eat);
			Animal.Action = true;
			Animal.Move( Vector3.zero , true );

			var lookAt = gameObject.GetComponent<MalbersAnimations.Utilities.LookAt>();
			if ( lookAt != null && targetFood != null )
				lookAt.Target = targetFood.transform;


		});

		m_stateMachine.AddUpdate (State.Eat, delegate {
			Animal.Action = false;
			if ( targetFood == null || !targetFood.IsAviable()  )
			{
				m_stateMachine.State = State.FindFood;
			} if ( m_health.IsFull ) {
				m_stateMachine.State = State.Walk;
			}else {
				m_health.EatFood( targetFood );
			}
		});

		m_stateMachine.AddExit (State.Eat, delegate {
			Animal.ActionEmotion(-1);	
			targetFood = null;

			var lookAt = gameObject.GetComponent<MalbersAnimations.Utilities.LookAt>();
			if ( lookAt != null )
				lookAt.Target = null;

			if ( !m_health.IsHungry )
			{
				M_Event.FireLogicEvent(LogicEvents.AnimalAction , new AnimalActionArg( this , AnimalAction.Eat , transform ) );
			}
		});
		
		m_stateMachine.State = State.Walk;
	}

	public bool IsTimerReady()
	{
		return timer < 0;
	}

	public void ResetTimer()
	{
		timer = walkChangeDirectionTimer.rand;
	}

	public void SetRandomTarget()
	{
		Vector3 pos = WorldManager.Instance.GetRandomPosition ();
		Agent.SetDestination (pos);

	}


	protected override void MUpdate ()
	{
		base.MUpdate ();
		timer -= Time.deltaTime;
		m_stateMachine.Update ();
		m_state = m_stateMachine.State;
		m_health.Update ();

	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterEvent (LogicEvents.FoodReady, OnFoodReady);
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnregisterEvent (LogicEvents.FoodReady, OnFoodReady);
	}


	public void OnFoodReady( LogicArg arg )
	{
		if (m_health.IsHungry) {
			FoodReadyArg fArg = (FoodReadyArg)arg;

			if (fArg != null) {
				if (aviableFoodList.Contains (fArg.food.m_type)) {

					Debug.Log (" On Food Ready ");
					if (targetFood == null) {
						targetFood = fArg.food;
						m_stateMachine.State = State.WalkToFood;
					} else {
						if (Global.GetDistance (fArg.food.gameObject, gameObject) < Global.GetDistance (targetFood.gameObject, gameObject)) {
							targetFood = fArg.food;
						}
					}
				}
			}
		}
	}
}
