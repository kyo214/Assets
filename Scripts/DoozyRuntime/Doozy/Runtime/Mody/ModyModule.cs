using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Attributes;
using UnityEngine;

namespace Doozy.Runtime.Mody;

[Serializable]
public abstract class ModyModule : MonoBehaviour, IHaveActions
{
	[SerializeField]
	private string ModuleName;

	private HashSet<ModyAction> m_ModuleActions;

	[SerializeField]
	private ModuleState ModuleCurrentState = ModuleState.Disabled;

	[ClearOnReload(true)]
	private static readonly ListDatabase<GameObject, ModyModule> Database = new ListDatabase<GameObject, ModyModule>();

	public string moduleName
	{
		get
		{
			return ModuleName;
		}
		internal set
		{
			ModuleName = value;
		}
	}

	public HashSet<ModyAction> actions
	{
		get
		{
			return m_ModuleActions ?? (m_ModuleActions = new HashSet<ModyAction>());
		}
		protected internal set
		{
			m_ModuleActions = value;
		}
	}

	public IEnumerable<string> actionNames => actions.Select((ModyAction a) => a.actionName);

	public ModuleState state
	{
		get
		{
			return ModuleCurrentState;
		}
		internal set
		{
			ModuleCurrentState = value;
		}
	}

	public bool initialized { get; protected set; }

	protected ModyModule(string moduleName)
	{
		ModuleName = moduleName;
		SetupActions();
	}

	public virtual void Initialize()
	{
		if (!initialized)
		{
			SetupActions();
			initialized = true;
		}
	}

	public void UpdateState()
	{
		state = ModuleState.Disabled;
		if (actions.Any((ModyAction action) => action.isActive))
		{
			state = ModuleState.Active;
		}
		else if (actions.Any((ModyAction action) => action.isIdle))
		{
			state = ModuleState.Idle;
		}
	}

	public void ActivateActions()
	{
		foreach (ModyAction action in actions)
		{
			action.OnActivate();
		}
	}

	public void DeactivateActions()
	{
		foreach (ModyAction action in actions)
		{
			action.OnDeactivate();
		}
	}

	public void StartAction(string actionName, bool ignoreCooldown, bool forced = false)
	{
		foreach (ModyAction item in actions.Where((ModyAction action) => action.actionName.Equals(actionName)))
		{
			item.StartRunning(null, ignoreCooldown, forced);
		}
	}

	public void StopAction(string actionName)
	{
		foreach (ModyAction item in actions.Where((ModyAction action) => action.actionName.Equals(actionName)))
		{
			item.StopRunning();
		}
	}

	public void StopAllActions()
	{
		foreach (ModyAction action in actions)
		{
			action.StopRunning();
		}
	}

	public void FinishAction(string actionName)
	{
		foreach (ModyAction item in actions.Where((ModyAction action) => action.actionName.Equals(actionName)))
		{
			item.FinishRunning();
		}
	}

	public void FinishAllActions()
	{
		foreach (ModyAction action in actions)
		{
			action.FinishRunning();
		}
	}

	public void Execute(string actionName, RunAction method, bool ignoreCooldown = false, bool forced = false)
	{
		GetAction(actionName)?.ExecuteMethod(method, ignoreCooldown, forced);
	}

	public ModyAction GetAction(string actionName)
	{
		return actions.FirstOrDefault((ModyAction action) => action.actionName.Equals(actionName));
	}

	public bool ContainsAction(string actionName)
	{
		return actions.Any((ModyAction action) => action.actionName.Equals(actionName));
	}

	protected abstract void SetupActions();

	public virtual void Validate()
	{
		foreach (ModyAction action in actions)
		{
			action.SetBehaviour(this);
			action.Validate();
		}
	}

	protected virtual void Awake()
	{
		RegisterToDatabase();
	}

	protected virtual void Start()
	{
		Initialize();
	}

	protected virtual void OnEnable()
	{
		Validate();
		SetupActions();
		actions.Remove(null);
		ActivateActions();
	}

	protected virtual void OnDisable()
	{
		actions.Remove(null);
		DeactivateActions();
	}

	protected virtual void OnDestroy()
	{
		UnregisterFromDatabase();
	}

	public static List<ModyModule> GetModules(GameObject target)
	{
		return Database.GetValues(target);
	}

	private void RegisterToDatabase()
	{
		Database.Add(base.gameObject, this);
	}

	private void UnregisterFromDatabase()
	{
		Database.Remove(base.gameObject, this);
	}
}
