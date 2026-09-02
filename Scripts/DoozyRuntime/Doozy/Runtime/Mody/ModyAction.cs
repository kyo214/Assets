using System;
using System.Collections;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Signals;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody;

[Serializable]
public abstract class ModyAction : MultiSignalsReceiver<SignalReceiver>
{
	public enum TriggeredActionState
	{
		Disabled = 0,
		Idle = 1,
		StartDelay = 2,
		OnStart = 3,
		Run = 4,
		OnFinish = 5,
		Cooldown = 6
	}

	[SerializeField]
	private MonoBehaviour ActionBehaviourReference;

	[SerializeField]
	private string ActionName;

	[SerializeField]
	private ActionState ActionCurrentState;

	[SerializeField]
	private bool ActionEnabled;

	[SerializeField]
	private float ActionStartDelay;

	[SerializeField]
	private float ActionDuration;

	[SerializeField]
	private float ActionCooldown;

	[SerializeField]
	private Timescale ActionTimescale;

	[SerializeField]
	private bool ActionOnStartStopOtherActions;

	[SerializeField]
	private ModyEvent OnStartEvents;

	[SerializeField]
	private ModyEvent OnFinishEvents;

	private Coroutine m_RunCoroutine;

	private Coroutine m_CooldownCoroutine;

	private bool m_BehaviourIsModule;

	private ModyModule m_Module;

	public bool HasValue;

	public Type ValueType;

	public bool IgnoreSignalValue;

	public bool ReactToAnySignal;

	public MonoBehaviour actionBehaviourReference
	{
		get
		{
			return ActionBehaviourReference;
		}
		internal set
		{
			ActionBehaviourReference = value;
			m_BehaviourIsModule = false;
			if (ActionBehaviourReference is ModyModule module)
			{
				m_Module = module;
				m_BehaviourIsModule = true;
			}
		}
	}

	public string actionName => ActionName;

	public ActionState currentState
	{
		get
		{
			return ActionCurrentState;
		}
		private set
		{
			ActionCurrentState = value;
			if (m_BehaviourIsModule)
			{
				m_Module.UpdateState();
			}
		}
	}

	public bool isIdle => currentState == ActionState.Idle;

	public bool inStartDelay => currentState == ActionState.InStartDelay;

	public bool isRunning => currentState == ActionState.IsRunning;

	public bool inCooldown => currentState == ActionState.InCooldown;

	public bool isActive
	{
		get
		{
			if (!inStartDelay && !isRunning)
			{
				return inCooldown;
			}
			return true;
		}
	}

	public bool enabled
	{
		get
		{
			return ActionEnabled;
		}
		set
		{
			if (value)
			{
				OnActivate();
				ActionEnabled = true;
				currentState = ActionState.Idle;
				onStateChanged?.Invoke(TriggeredActionState.Idle);
			}
			else
			{
				OnDeactivate();
				ActionEnabled = false;
				currentState = ActionState.Disabled;
				onStateChanged?.Invoke(TriggeredActionState.Disabled);
			}
		}
	}

	public float startDelay
	{
		get
		{
			if (!(ActionStartDelay > 0f))
			{
				return 0f;
			}
			return ActionStartDelay;
		}
		internal set
		{
			ActionStartDelay = ((value > 0f) ? value : 0f);
		}
	}

	public float duration
	{
		get
		{
			if (!(ActionDuration > 0f))
			{
				return 0f;
			}
			return ActionDuration;
		}
		internal set
		{
			ActionDuration = ((value > 0f) ? value : 0f);
		}
	}

	public float totalDuration => startDelay + duration;

	public float cooldown
	{
		get
		{
			if (!(ActionCooldown > 0f))
			{
				return 0f;
			}
			return ActionCooldown;
		}
		internal set
		{
			ActionCooldown = ((value > 0f) ? value : 0f);
		}
	}

	public bool isTimescaleIndependent
	{
		get
		{
			return ActionTimescale == Timescale.Independent;
		}
		internal set
		{
			ActionTimescale = ((!value) ? Timescale.Dependent : Timescale.Independent);
		}
	}

	public bool onStartStopOtherActions
	{
		get
		{
			return ActionOnStartStopOtherActions;
		}
		internal set
		{
			ActionOnStartStopOtherActions = value;
		}
	}

	public ModyEvent onStartEvents => OnStartEvents;

	public ModyEvent onFinishEvents => OnFinishEvents;

	public UnityAction<TriggeredActionState> onStateChanged { get; set; }

	protected ModyAction(MonoBehaviour behaviour, string actionName)
	{
		actionBehaviourReference = behaviour;
		ActionName = actionName.RemoveWhitespaces().RemoveAllSpecialCharacters();
		currentState = ActionState.Disabled;
		ActionEnabled = false;
		ActionStartDelay = 0f;
		ActionDuration = 0f;
		ActionCooldown = 0f;
		ActionTimescale = Timescale.Independent;
		ActionOnStartStopOtherActions = true;
		OnStartEvents = new ModyEvent("OnStart");
		OnFinishEvents = new ModyEvent("OnFinish");
		HasValue = false;
		ValueType = null;
		IgnoreSignalValue = true;
		ReactToAnySignal = true;
	}

	protected override void OnSignal(Signal signal)
	{
		StartRunning(signal);
	}

	public virtual void OnActivate()
	{
		if (enabled)
		{
			Validate();
			ConnectReceivers();
			StopRunning();
			StopCooldown();
			currentState = ActionState.Idle;
			onStateChanged?.Invoke(TriggeredActionState.Idle);
		}
	}

	public virtual void OnDeactivate()
	{
		DisconnectReceivers();
		StopRunning();
		StopCooldown();
	}

	public virtual void Validate()
	{
		UpdateSignalReceivers();
	}

	public void StartRunning()
	{
		StartRunning(null, ignoreCooldown: false);
	}

	public void StartRunning(bool ignoreCooldown)
	{
		StartRunning(null, ignoreCooldown);
	}

	public void StartRunning(Signal signal)
	{
		StartRunning(signal, ignoreCooldown: false);
	}

	public void StartRunning(Signal signal, bool ignoreCooldown, bool forced = false)
	{
		if (!forced && !enabled)
		{
			return;
		}
		if (onStartStopOtherActions)
		{
			StopAllOtherActions();
		}
		if (currentState == ActionState.InCooldown)
		{
			if (!ignoreCooldown)
			{
				return;
			}
			StopCooldown();
		}
		if (currentState == ActionState.IsRunning)
		{
			StopRunning();
		}
		if (totalDuration == 0f)
		{
			onStartEvents?.Execute();
			onStateChanged?.Invoke(TriggeredActionState.OnStart);
			currentState = ActionState.IsRunning;
			onStateChanged?.Invoke(TriggeredActionState.Run);
			Run(signal);
			FinishRunning();
		}
		else
		{
			m_RunCoroutine = actionBehaviourReference.StartCoroutine(ExecuteRun(signal));
		}
	}

	protected IEnumerator ExecuteRun(Signal signal)
	{
		if (ActionStartDelay > 0f)
		{
			currentState = ActionState.InStartDelay;
			onStateChanged?.Invoke(TriggeredActionState.StartDelay);
			if (isTimescaleIndependent)
			{
				yield return new WaitForSecondsRealtime(startDelay);
			}
			else
			{
				yield return new WaitForSeconds(startDelay);
			}
		}
		onStartEvents?.Execute();
		onStateChanged?.Invoke(TriggeredActionState.OnStart);
		currentState = ActionState.IsRunning;
		onStateChanged?.Invoke(TriggeredActionState.Run);
		Run(signal);
		if (duration > 0f)
		{
			if (isTimescaleIndependent)
			{
				yield return new WaitForSecondsRealtime(duration);
			}
			else
			{
				yield return new WaitForSeconds(duration);
			}
		}
		FinishRunning();
	}

	public void StopRunning()
	{
		if (m_RunCoroutine != null)
		{
			actionBehaviourReference.StopCoroutine(m_RunCoroutine);
			m_RunCoroutine = null;
		}
		ActionState actionState = currentState;
		if (actionState != ActionState.Disabled && actionState != ActionState.InCooldown)
		{
			currentState = ActionState.Idle;
		}
	}

	public void StartCooldown()
	{
		if (!enabled)
		{
			return;
		}
		if (currentState == ActionState.IsRunning)
		{
			StopRunning();
		}
		if (currentState == ActionState.Disabled)
		{
			return;
		}
		if (cooldown == 0f)
		{
			currentState = ActionState.Idle;
			onStateChanged?.Invoke(TriggeredActionState.Idle);
			return;
		}
		if (currentState == ActionState.InCooldown)
		{
			StopCooldown();
		}
		m_CooldownCoroutine = actionBehaviourReference.StartCoroutine(ExecuteCooldown());
	}

	protected IEnumerator ExecuteCooldown()
	{
		currentState = ActionState.InCooldown;
		onStateChanged?.Invoke(TriggeredActionState.Cooldown);
		if (isTimescaleIndependent)
		{
			yield return new WaitForSecondsRealtime(cooldown);
		}
		else
		{
			yield return new WaitForSeconds(cooldown);
		}
		StopCooldown();
	}

	public void StopCooldown()
	{
		if (m_CooldownCoroutine != null)
		{
			actionBehaviourReference.StopCoroutine(m_CooldownCoroutine);
			m_CooldownCoroutine = null;
		}
		currentState = ActionState.Idle;
		onStateChanged?.Invoke(TriggeredActionState.Idle);
	}

	public void FinishRunning()
	{
		if (isActive || enabled)
		{
			onFinishEvents?.Execute();
			onStateChanged?.Invoke(TriggeredActionState.OnFinish);
			if (cooldown > 0f)
			{
				StartCooldown();
				return;
			}
			currentState = ActionState.Idle;
			onStateChanged?.Invoke(TriggeredActionState.Idle);
		}
	}

	public void ExecuteMethod(RunAction method, bool ignoreCooldown = false, bool forced = false)
	{
		switch (method)
		{
		case RunAction.Start:
			StartRunning(null, ignoreCooldown, forced);
			break;
		case RunAction.Stop:
			StopRunning();
			break;
		case RunAction.Finish:
			FinishRunning();
			break;
		default:
			throw new ArgumentOutOfRangeException("method", method, null);
		}
	}

	public void StopAllOtherActions()
	{
		if (enabled)
		{
			((IHaveActions)actionBehaviourReference)?.StopAllActions();
		}
	}

	protected abstract void Run(Signal signal);

	public abstract bool SetValue(object objectValue);

	internal abstract bool SetValue(object objectValue, bool restrictValueType);

	private void UpdateSignalReceivers()
	{
		foreach (SignalReceiver signalsReceiver in SignalsReceivers)
		{
			switch (signalsReceiver.streamConnection)
			{
			case StreamConnection.None:
				SignalReceiverExtensions.SetSignalSource(signalsReceiver, actionBehaviourReference.gameObject);
				break;
			case StreamConnection.ProviderId:
				SignalReceiverExtensions.SetSignalSource(signalsReceiver, (signalsReceiver.providerId.Type == ProviderType.Local) ? actionBehaviourReference.gameObject : SingletonBehaviour<Doozy.Runtime.Signals.Signals>.instance.gameObject);
				break;
			case StreamConnection.ProviderReference:
				if (signalsReceiver.providerReference != null)
				{
					SignalReceiverExtensions.SetSignalSource(signalsReceiver, signalsReceiver.providerReference.gameObject);
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case StreamConnection.StreamId:
				break;
			}
		}
	}
}
