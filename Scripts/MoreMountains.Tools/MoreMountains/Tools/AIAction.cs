using UnityEngine;

namespace MoreMountains.Tools;

public abstract class AIAction : MonoBehaviour
{
	public string Label;

	protected AIBrain _brain;

	public bool ActionInProgress { get; set; }

	public abstract void PerformAction();

	protected virtual void Awake()
	{
		_brain = base.gameObject.GetComponentInParent<AIBrain>();
	}

	public virtual void Initialization()
	{
	}

	public virtual void OnEnterState()
	{
		ActionInProgress = true;
	}

	public virtual void OnExitState()
	{
		ActionInProgress = false;
	}
}
