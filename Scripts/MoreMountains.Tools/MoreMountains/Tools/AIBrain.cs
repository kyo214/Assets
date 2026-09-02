using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/AI/AIBrain")]
public class AIBrain : MonoBehaviour
{
	[Header("Debug")]
	public bool BrainActive = true;

	[MMReadOnly]
	public GameObject Owner;

	public List<AIState> States;

	[MMReadOnly]
	public float TimeInThisState;

	[MMReadOnly]
	public Transform Target;

	[MMReadOnly]
	public Vector3 _lastKnownTargetPosition = Vector3.zero;

	[Header("Frequencies")]
	public float ActionsFrequency;

	public float DecisionFrequency;

	public bool RandomizeFrequencies;

	[MMVector(new string[] { "min", "max" })]
	public Vector2 RandomActionFrequency = new Vector2(0.5f, 1f);

	[MMVector(new string[] { "min", "max" })]
	public Vector2 RandomDecisionFrequency = new Vector2(0.5f, 1f);

	protected AIDecision[] _decisions;

	protected AIAction[] _actions;

	protected float _lastActionsUpdate;

	protected float _lastDecisionsUpdate;

	protected AIState _initialState;

	public AIState CurrentState { get; protected set; }

	public virtual AIAction[] GetAttachedActions()
	{
		return base.gameObject.GetComponentsInChildren<AIAction>();
	}

	public virtual AIDecision[] GetAttachedDecisions()
	{
		return base.gameObject.GetComponentsInChildren<AIDecision>();
	}

	protected virtual void Awake()
	{
		foreach (AIState state in States)
		{
			state.SetBrain(this);
		}
		_decisions = GetAttachedDecisions();
		_actions = GetAttachedActions();
		if (RandomizeFrequencies)
		{
			ActionsFrequency = Random.Range(RandomActionFrequency.x, RandomActionFrequency.y);
			DecisionFrequency = Random.Range(RandomDecisionFrequency.x, RandomDecisionFrequency.y);
		}
	}

	protected virtual void Start()
	{
		ResetBrain();
	}

	protected virtual void Update()
	{
		if (!BrainActive || CurrentState == null || Time.timeScale == 0f)
		{
			return;
		}
		if (Time.time - _lastActionsUpdate > ActionsFrequency)
		{
			CurrentState.PerformActions();
			_lastActionsUpdate = Time.time;
		}
		if (BrainActive)
		{
			if (Time.time - _lastDecisionsUpdate > DecisionFrequency)
			{
				CurrentState.EvaluateTransitions();
				_lastDecisionsUpdate = Time.time;
			}
			TimeInThisState += Time.deltaTime;
			StoreLastKnownPosition();
		}
	}

	public virtual void TransitionToState(string newStateName)
	{
		if (CurrentState == null)
		{
			CurrentState = FindState(newStateName);
			if (CurrentState != null)
			{
				CurrentState.EnterState();
			}
		}
		else if (newStateName != CurrentState.StateName)
		{
			CurrentState.ExitState();
			OnExitState();
			CurrentState = FindState(newStateName);
			if (CurrentState != null)
			{
				CurrentState.EnterState();
			}
		}
	}

	protected virtual void OnExitState()
	{
		TimeInThisState = 0f;
	}

	protected virtual void InitializeDecisions()
	{
		if (_decisions == null)
		{
			_decisions = GetAttachedDecisions();
		}
		AIDecision[] decisions = _decisions;
		for (int i = 0; i < decisions.Length; i++)
		{
			decisions[i].Initialization();
		}
	}

	protected virtual void InitializeActions()
	{
		if (_actions == null)
		{
			_actions = GetAttachedActions();
		}
		AIAction[] actions = _actions;
		for (int i = 0; i < actions.Length; i++)
		{
			actions[i].Initialization();
		}
	}

	protected AIState FindState(string stateName)
	{
		foreach (AIState state in States)
		{
			if (state.StateName == stateName)
			{
				return state;
			}
		}
		if (stateName != "")
		{
			Debug.LogError("You're trying to transition to state '" + stateName + "' in " + base.gameObject.name + "'s AI Brain, but no state of this name exists. Make sure your states are named properly, and that your transitions states match existing states.");
		}
		return null;
	}

	protected virtual void StoreLastKnownPosition()
	{
		if (Target != null)
		{
			_lastKnownTargetPosition = Target.transform.position;
		}
	}

	public virtual void ResetBrain()
	{
		InitializeDecisions();
		InitializeActions();
		BrainActive = true;
		base.enabled = true;
		if (CurrentState != null)
		{
			CurrentState.ExitState();
			OnExitState();
		}
		if (States.Count > 0)
		{
			CurrentState = States[0];
			CurrentState?.EnterState();
		}
	}
}
