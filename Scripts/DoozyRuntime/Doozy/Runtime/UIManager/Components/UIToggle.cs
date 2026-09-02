using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Components;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Components/UIToggle")]
[SelectionBase]
public class UIToggle : UISelectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler
{
	[ClearOnReload]
	private static SignalStream s_stream;

	public UIToggleId Id = new UIToggleId();

	public ModyEvent OnToggleOnCallback = new ModyEvent("OnToggleOnCallback");

	public ModyEvent OnInstantToggleOnCallback = new ModyEvent("OnInstantToggleOnCallback");

	public ModyEvent OnToggleOffCallback = new ModyEvent("OnToggleOffCallback");

	public ModyEvent OnInstantToggleOffCallback = new ModyEvent("OnInstantToggleOffCallback");

	public BoolEvent OnValueChangedCallback = new BoolEvent();

	[SerializeField]
	private UIToggleGroup ToggleGroup;

	[SerializeField]
	protected bool IsLocked;

	public static HashSet<UIToggle> database { get; private set; } = new HashSet<UIToggle>();

	public static SignalStream stream => s_stream ?? (s_stream = SignalsService.GetStream("UISelectable", "UIToggle"));

	public static IEnumerable<UIToggle> availableToggles => database.Where((UIToggle item) => item.isActiveAndEnabled);

	public bool isSelected => EventSystem.current.currentSelectedGameObject == base.gameObject;

	public override SelectableType selectableType => SelectableType.Toggle;

	public UnityAction<ToggleValueChangedEvent> onToggleValueChangedCallback { get; set; }

	public bool inToggleGroup
	{
		get
		{
			if (ToggleGroup != null)
			{
				return ToggleGroup.toggles.Contains(this);
			}
			return false;
		}
	}

	public UIToggleGroup toggleGroup
	{
		get
		{
			return ToggleGroup;
		}
		internal set
		{
			ToggleGroup = value;
		}
	}

	public override bool isOn
	{
		get
		{
			return IsOn;
		}
		set
		{
			if (!isLocked)
			{
				bool previousValue = IsOn;
				IsOn = value;
				if (inToggleGroup)
				{
					toggleGroup.ToggleChangedValue(this, animateChange: true);
				}
				else
				{
					ValueChanged(previousValue, value, animateChange: true, triggerValueChanged: true);
				}
			}
		}
	}

	public bool isLocked
	{
		get
		{
			return IsLocked;
		}
		set
		{
			IsLocked = value;
		}
	}

	protected bool toggleInitialized { get; set; }

	[ExecuteOnReload]
	private static void OnReload()
	{
		database = new HashSet<UIToggle>();
	}

	protected override void Awake()
	{
		toggleInitialized = false;
		if (Application.isPlaying)
		{
			database.Add(this);
			base.Awake();
		}
	}

	protected override void OnEnable()
	{
		if (Application.isPlaying)
		{
			StopCooldown();
			database.Remove(null);
			base.OnEnable();
			InitializeToggle();
		}
	}

	protected override void OnDisable()
	{
		StopCooldown();
		database.Remove(null);
		base.OnDisable();
	}

	protected override void OnDestroy()
	{
		database.Remove(null);
		database.Remove(this);
		base.OnDestroy();
	}

	protected virtual void InitializeToggle()
	{
		if (!toggleInitialized)
		{
			AddToToggleGroup(toggleGroup);
			toggleInitialized = true;
			if (!inToggleGroup)
			{
				ValueChanged(isOn, isOn, animateChange: false, triggerValueChanged: false);
			}
		}
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (!base.inCooldown && eventData.button == PointerEventData.InputButton.Left && IsActive() && IsInteractable())
		{
			ToggleValue();
		}
	}

	public virtual void OnSubmit(BaseEventData eventData)
	{
		if (!base.inCooldown && IsActive() && IsInteractable())
		{
			ToggleValue();
			if (UISelectable.inputSettings.submitTriggersPointerClick)
			{
				base.behaviours.GetBehaviour(UIBehaviour.Name.PointerClick)?.Execute();
				base.behaviours.GetBehaviour(UIBehaviour.Name.PointerLeftClick)?.Execute();
			}
		}
	}

	protected virtual void ToggleValue()
	{
		if (!isLocked)
		{
			isOn = !isOn;
			StartCooldown();
		}
	}

	public void AddToToggleGroup(UIToggleGroup targetToggleGroup)
	{
		if (!(targetToggleGroup == null))
		{
			if (inToggleGroup && targetToggleGroup != toggleGroup)
			{
				RemoveFromToggleGroup();
			}
			if (isLocked)
			{
				isLocked = false;
			}
			targetToggleGroup.AddToggle(this);
		}
	}

	public void RemoveFromToggleGroup()
	{
		if (!(toggleGroup == null))
		{
			toggleGroup.RemoveToggle(this);
		}
	}

	protected internal virtual void UpdateValueFromGroup(bool newValue, bool animateChange, bool triggerValueChanged = true)
	{
		if (isLocked)
		{
			isLocked = false;
		}
		bool previousValue = IsOn;
		IsOn = newValue;
		ValueChanged(previousValue, newValue, animateChange, triggerValueChanged);
	}

	internal void SendSignal(bool newValue)
	{
		stream.SendSignal(new UIToggleSignalData(Id.Category, Id.Name, (!newValue) ? CommandToggle.Off : CommandToggle.On, base.playerIndex, this));
	}

	internal virtual void ValueChanged(bool previousValue, bool newValue, bool animateChange, bool triggerValueChanged)
	{
		RefreshState();
		if (newValue)
		{
			if (animateChange)
			{
				OnToggleOnCallback?.Execute();
			}
			else
			{
				OnInstantToggleOnCallback?.Execute();
			}
		}
		else if (animateChange)
		{
			OnToggleOffCallback?.Execute();
		}
		else
		{
			OnInstantToggleOffCallback?.Execute();
		}
		if (triggerValueChanged)
		{
			SendSignal(newValue);
			OnValueChangedCallback?.Invoke(newValue);
			onToggleValueChangedCallback?.Invoke(new ToggleValueChangedEvent(previousValue, newValue, animateChange));
		}
	}

	public static IEnumerable<UIToggle> GetToggles(string category, string name)
	{
		return from toggle in database
			where toggle.Id.Category.Equals(category)
			where toggle.Id.Name.Equals(name)
			select toggle;
	}

	public static IEnumerable<UIToggle> GetAllTogglesInCategory(string category)
	{
		return database.Where((UIToggle toggle) => toggle.Id.Category.Equals(category));
	}

	public static IEnumerable<UIToggle> GetAvailableToggles()
	{
		return database.Where((UIToggle toggle) => toggle.isActiveAndEnabled);
	}

	public static UIToggle GetSelectedToggle()
	{
		return database.FirstOrDefault((UIToggle toggle) => toggle.isSelected);
	}

	public static bool SelectToggle(string category, string name)
	{
		UIToggle uIToggle = availableToggles.FirstOrDefault((UIToggle b) => b.Id.Category.Equals(category) & b.Id.Name.Equals(name));
		if (uIToggle == null)
		{
			return false;
		}
		uIToggle.Select();
		return true;
	}
}
