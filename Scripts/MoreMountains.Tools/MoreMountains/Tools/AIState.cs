using System;
using UnityEngine;

namespace MoreMountains.Tools;

[Serializable]
public class AIState
{
	public string StateName;

	[MMReorderableAttribute(null, "Action", null)]
	public AIActionsList Actions;

	[MMReorderableAttribute(null, "Transition", null)]
	public AITransitionsList Transitions;

	protected AIBrain _brain;

	public virtual void SetBrain(AIBrain brain)
	{
		_brain = brain;
	}

	public virtual void EnterState()
	{
		foreach (AIAction action in Actions)
		{
			action.OnEnterState();
		}
		foreach (AITransition transition in Transitions)
		{
			if (transition.Decision != null)
			{
				transition.Decision.OnEnterState();
			}
		}
	}

	public virtual void ExitState()
	{
		foreach (AIAction action in Actions)
		{
			action.OnExitState();
		}
		foreach (AITransition transition in Transitions)
		{
			if (transition.Decision != null)
			{
				transition.Decision.OnExitState();
			}
		}
	}

	public virtual void PerformActions()
	{
		if (Actions.Count == 0)
		{
			return;
		}
		for (int i = 0; i < Actions.Count; i++)
		{
			if (Actions[i] != null)
			{
				Actions[i].PerformAction();
				continue;
			}
			Debug.LogError("An action in " + _brain.gameObject.name + " on state " + StateName + " is null.");
		}
	}

	public virtual void EvaluateTransitions()
	{
		if (Transitions.Count == 0)
		{
			return;
		}
		for (int i = 0; i < Transitions.Count; i++)
		{
			if (!(Transitions[i].Decision != null))
			{
				continue;
			}
			if (Transitions[i].Decision.Decide())
			{
				if (!string.IsNullOrEmpty(Transitions[i].TrueState))
				{
					_brain.TransitionToState(Transitions[i].TrueState);
					break;
				}
			}
			else if (!string.IsNullOrEmpty(Transitions[i].FalseState))
			{
				_brain.TransitionToState(Transitions[i].FalseState);
				break;
			}
		}
	}
}
