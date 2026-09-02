using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common;
using Doozy.Runtime.Mody;
using Doozy.Runtime.UIManager.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Signals;

public abstract class SignalProvider : MonoBehaviour, ISignalProvider
{
	public static class Local
	{
		public static class Pointer
		{
			public enum Name
			{
				Click = 0,
				DoubleClick = 1,
				Down = 2,
				Enter = 3,
				Exit = 4,
				LeftClick = 5,
				LongClick = 6,
				MiddleClick = 7,
				RightClick = 8,
				Up = 9
			}

			public const string k_ProviderCategory = "Pointer";

			public static readonly List<ProviderAttributes> AttributesList = new List<ProviderAttributes>
			{
				new ProviderAttributes(ProviderType.Local, "Pointer", Name.Click.ToString(), typeof(PointerClickTrigger)),
				new ProviderAttributes(ProviderType.Local, "Pointer", Name.DoubleClick.ToString(), typeof(PointerDoubleClickTrigger)),
				new ProviderAttributes(ProviderType.Local, "Pointer", Name.Down.ToString(), typeof(PointerDownTrigger)),
				new ProviderAttributes(ProviderType.Local, "Pointer", Name.Enter.ToString(), typeof(PointerEnterTrigger)),
				new ProviderAttributes(ProviderType.Local, "Pointer", Name.Exit.ToString(), typeof(PointerExitTrigger)),
				new ProviderAttributes(ProviderType.Local, "Pointer", Name.LeftClick.ToString(), typeof(PointerLeftClickTrigger)),
				new ProviderAttributes(ProviderType.Local, "Pointer", Name.LongClick.ToString(), typeof(PointerLongClickTrigger)),
				new ProviderAttributes(ProviderType.Local, "Pointer", Name.MiddleClick.ToString(), typeof(PointerMiddleClickTrigger)),
				new ProviderAttributes(ProviderType.Local, "Pointer", Name.RightClick.ToString(), typeof(PointerRightClickTrigger)),
				new ProviderAttributes(ProviderType.Local, "Pointer", Name.Up.ToString(), typeof(PointerUpTrigger))
			};

			public static IEnumerable<string> GetProviderNames()
			{
				return from Name name in Enum.GetValues(typeof(Name))
					select name.ToString();
			}

			public static ProviderId GetProviderId(Name providerName)
			{
				return new ProviderId(ProviderType.Local, "Pointer", providerName.ToString());
			}

			public static ISignalProvider Get(Name providerName, GameObject signalSource)
			{
				return SignalsService.GetProvider(GetProviderId(providerName), signalSource);
			}
		}

		public static class UI
		{
			public enum Name
			{
				Deselected = 0,
				Selected = 1,
				Submit = 2
			}

			public const string k_ProviderCategory = "UI";

			public static readonly List<ProviderAttributes> AttributesList = new List<ProviderAttributes>
			{
				new ProviderAttributes(ProviderType.Local, "UI", Name.Deselected.ToString(), typeof(UIDeselectedTrigger)),
				new ProviderAttributes(ProviderType.Local, "UI", Name.Selected.ToString(), typeof(UISelectedTrigger)),
				new ProviderAttributes(ProviderType.Local, "UI", Name.Submit.ToString(), typeof(UISubmitTrigger))
			};

			public static IEnumerable<string> GetProviderNames()
			{
				return from Name name in Enum.GetValues(typeof(Name))
					select name.ToString();
			}

			public static ProviderId GetProviderId(Name providerName)
			{
				return new ProviderId(ProviderType.Local, "UI", providerName.ToString());
			}

			public static ISignalProvider Get(Name providerName, GameObject signalSource)
			{
				return SignalsService.GetProvider(GetProviderId(providerName), signalSource);
			}
		}

		public const ProviderType k_ProviderType = ProviderType.Local;

		public static IEnumerable<string> GetProviderCategories()
		{
			return new List<string> { "Pointer", "UI" };
		}

		public static IEnumerable<string> GetProviderNames(string category)
		{
			if (!(category == "Pointer"))
			{
				if (category == "UI")
				{
					return UI.GetProviderNames();
				}
				throw new Exception(string.Format("There is no {0} {1} '{2}' category registered in the {3}", ProviderType.Local, "SignalProvider", category, "SignalProvider"));
			}
			return Pointer.GetProviderNames();
		}

		public static IEnumerable<ProviderAttributes> GetAttributesList(string category)
		{
			if (!(category == "Pointer"))
			{
				if (category == "UI")
				{
					return UI.AttributesList;
				}
				throw new Exception(string.Format("There is no {0} {1} '{2}' category registered in the {3}", ProviderType.Local, "SignalProvider", category, "SignalProvider"));
			}
			return Pointer.AttributesList;
		}
	}

	public static class Global
	{
		public static class Input
		{
			public enum Name
			{
				BackButton = 0
			}

			public const string k_ProviderCategory = "Input";

			public static readonly List<ProviderAttributes> AttributesList = new List<ProviderAttributes>
			{
				new ProviderAttributes(ProviderType.Global, "Input", Name.BackButton.ToString(), typeof(InputBackButtonTrigger))
			};

			public static IEnumerable<string> GetProviderNames()
			{
				return from Name name in Enum.GetValues(typeof(Name))
					select name.ToString();
			}

			public static ProviderId GetProviderId(Name providerName)
			{
				return new ProviderId(ProviderType.Global, "Input", providerName.ToString());
			}

			public static ISignalProvider Get(Name providerName, GameObject signalSource)
			{
				return SignalsService.GetProvider(GetProviderId(providerName), signalSource);
			}
		}

		public const ProviderType k_ProviderType = ProviderType.Global;

		public static IEnumerable<string> GetProviderCategories()
		{
			return new List<string> { "Input" };
		}

		public static IEnumerable<string> GetProviderNames(string category)
		{
			if (category == "Input")
			{
				return Input.GetProviderNames();
			}
			throw new Exception(string.Format("There is no {0} {1} '{2}' category registered in the {3}", ProviderType.Global, "SignalProvider", category, "SignalProvider"));
		}

		public static IEnumerable<ProviderAttributes> GetAttributesList(string category)
		{
			if (category == "Input")
			{
				return Input.AttributesList;
			}
			throw new Exception(string.Format("There is no {0} {1} '{2}' category registered in the {3}", ProviderType.Global, "SignalProvider", category, "SignalProvider"));
		}
	}

	[SerializeField]
	private ProviderState ProviderCurrentState;

	[SerializeField]
	private float SignalCooldown;

	[SerializeField]
	private Timescale SignalTimescale;

	private Coroutine m_CooldownCoroutine;

	public ProviderAttributes attributes { get; }

	public SignalStream stream { get; private set; }

	public bool isConnected { get; private set; }

	public ProviderState currentState
	{
		get
		{
			return ProviderCurrentState;
		}
		private set
		{
			ProviderCurrentState = value;
			onStateChanged?.Invoke(value);
		}
	}

	public UnityAction<ProviderState> onStateChanged { get; set; }

	public bool isIdle => currentState == ProviderState.Idle;

	public bool isRunning => currentState == ProviderState.IsRunning;

	public bool inCooldown => currentState == ProviderState.InCooldown;

	public bool isActive
	{
		get
		{
			if (!isRunning)
			{
				return inCooldown;
			}
			return true;
		}
	}

	public float cooldown
	{
		get
		{
			if (!(SignalCooldown > 0f))
			{
				return 0f;
			}
			return SignalCooldown;
		}
		set
		{
			SignalCooldown = ((value > 0f) ? value : 0f);
		}
	}

	public bool isTimescaleIndependent
	{
		get
		{
			return SignalTimescale == Timescale.Independent;
		}
		internal set
		{
			SignalTimescale = ((!value) ? Timescale.Dependent : Timescale.Independent);
		}
	}

	protected SignalProvider(ProviderType providerType, string providerCategory, string providerName, Type typeOfProvider)
	{
		attributes = new ProviderAttributes(providerType, providerCategory, providerName, typeOfProvider);
		stream = null;
		ProviderCurrentState = ProviderState.Disabled;
		SignalCooldown = 0f;
		SignalTimescale = Timescale.Independent;
	}

	public void OpenStream()
	{
		if (!isConnected)
		{
			stream = SignalsService.GetStream().SetSignalProvider(this);
			SignalsService.AddProvider(this);
			isConnected = true;
			currentState = ProviderState.Idle;
		}
	}

	public void CloseStream()
	{
		if (isConnected)
		{
			SignalsService.CloseStream(stream);
			stream = null;
			isConnected = false;
			currentState = ProviderState.Disabled;
		}
	}

	protected virtual void Awake()
	{
		stream = null;
		isConnected = false;
		OpenStream();
	}

	protected virtual void OnEnable()
	{
		currentState = (isConnected ? ProviderState.Idle : ProviderState.Disabled);
	}

	protected virtual void OnDisable()
	{
		StopCooldown();
		currentState = ProviderState.Disabled;
	}

	protected virtual void OnDestroy()
	{
		CloseStream();
	}

	public bool SendSignal()
	{
		return SendSignal(string.Empty);
	}

	public bool SendSignal(string message)
	{
		switch (currentState)
		{
		case ProviderState.Disabled:
		case ProviderState.InCooldown:
			return false;
		case ProviderState.Idle:
		case ProviderState.IsRunning:
		{
			currentState = ProviderState.IsRunning;
			bool result = isConnected && stream.SendSignal(this, message);
			StartCooldown();
			return result;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public bool SendSignal<T>(T signalValue)
	{
		return SendSignal(signalValue, string.Empty);
	}

	public bool SendSignal<T>(T signalValue, string message)
	{
		switch (currentState)
		{
		case ProviderState.Disabled:
		case ProviderState.InCooldown:
			return false;
		case ProviderState.Idle:
		case ProviderState.IsRunning:
		{
			currentState = ProviderState.IsRunning;
			bool result = isConnected && stream.SendSignal(signalValue, this, message);
			StartCooldown();
			return result;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public void StartCooldown()
	{
		if (currentState == ProviderState.Disabled)
		{
			return;
		}
		if (cooldown == 0f)
		{
			currentState = ProviderState.Idle;
			return;
		}
		if (currentState == ProviderState.InCooldown)
		{
			StopCooldown();
		}
		m_CooldownCoroutine = StartCoroutine(ExecuteCooldown());
	}

	protected IEnumerator ExecuteCooldown()
	{
		currentState = ProviderState.InCooldown;
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
			StopCoroutine(m_CooldownCoroutine);
			m_CooldownCoroutine = null;
		}
		currentState = ((base.isActiveAndEnabled && isConnected) ? ProviderState.Idle : ProviderState.Disabled);
	}

	public static IEnumerable<string> GetProviderCategories(ProviderType providerType)
	{
		return providerType switch
		{
			ProviderType.Global => Global.GetProviderCategories(), 
			ProviderType.Local => Local.GetProviderCategories(), 
			_ => throw new ArgumentOutOfRangeException("providerType", providerType, null), 
		};
	}

	public static IEnumerable<string> GetProviderNames(ProviderType providerType, string category)
	{
		return providerType switch
		{
			ProviderType.Global => Global.GetProviderNames(category), 
			ProviderType.Local => Local.GetProviderNames(category), 
			_ => throw new ArgumentOutOfRangeException("providerType", providerType, null), 
		};
	}

	public static Type GetProviderType(ProviderId providerId)
	{
		return providerId.Type switch
		{
			ProviderType.Global => (from a in Global.GetAttributesList(providerId.Category)
				where a.id == providerId
				select a).First().typeOfProvider, 
			ProviderType.Local => (from a in Local.GetAttributesList(providerId.Category)
				where a.id == providerId
				select a).First().typeOfProvider, 
			_ => throw new Exception("There is no " + providerId.Category + " " + providerId.Name + " SignalProvider registered in the SignalProvider"), 
		};
	}
}
