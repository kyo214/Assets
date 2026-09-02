using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Mody;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Components;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Components/UIToggle Group")]
[SelectionBase]
public class UIToggleGroup : UIToggle
{
	public enum Value
	{
		Off = 0,
		On = 1,
		MixedValues = 2
	}

	public enum ControlMode
	{
		Passive = 0,
		OneToggleOn = 1,
		OneToggleOnEnforced = 2,
		AnyToggleOnEnforced = 3
	}

	public enum SortMode
	{
		Disabled = 0,
		Hierarchy = 1,
		GameObjectName = 2,
		ToggleName = 3
	}

	[SerializeField]
	private bool OverrideInteractabilityForToggles;

	[SerializeField]
	private Value ToggleGroupValue;

	[SerializeField]
	private ControlMode Mode;

	[SerializeField]
	private bool HasMixedValues;

	[SerializeField]
	private SortMode AutoSort = SortMode.Hierarchy;

	public ModyEvent OnToggleGroupMixedValuesCallback;

	public UIToggleEvent OnToggleAddedCallback = new UIToggleEvent();

	public UIToggleEvent OnToggleRemovedCallback = new UIToggleEvent();

	public UIToggleEvent OnToggleTriggeredCallback = new UIToggleEvent();

	public UIToggle FirstToggle;

	public bool overrideInteractabilityForToggles
	{
		get
		{
			return OverrideInteractabilityForToggles;
		}
		set
		{
			OverrideInteractabilityForToggles = value;
		}
	}

	public Value toggleGroupValue
	{
		get
		{
			return ToggleGroupValue;
		}
		private set
		{
			ToggleGroupValue = value;
		}
	}

	public ControlMode mode
	{
		get
		{
			return Mode;
		}
		set
		{
			Mode = value;
			UpdateGroupValue(animateChange: false);
		}
	}

	public bool hasMixedValues
	{
		get
		{
			return HasMixedValues;
		}
		private set
		{
			if (HasMixedValues != value)
			{
				HasMixedValues = value;
				if (HasMixedValues)
				{
					OnToggleGroupMixedValuesCallback?.Execute();
				}
			}
		}
	}

	public SortMode autoSort
	{
		get
		{
			return AutoSort;
		}
		set
		{
			AutoSort = value;
			SortToggles(value);
		}
	}

	public List<UIToggle> toggles { get; private set; } = new List<UIToggle>();

	public int numberOfToggles => toggles?.Count ?? 0;

	public int numberOfTogglesOn => toggles?.Count((UIToggle toggle) => toggle.isOn) ?? 0;

	public int numberOfTogglesOff => toggles?.Count((UIToggle toggle) => !toggle.isOn) ?? 0;

	public bool anyOfTogglesOn => toggles?.Any((UIToggle toggle) => toggle.isOn) ?? false;

	public bool anyOfTogglesOff => toggles?.Any((UIToggle toggle) => !toggle.isOn) ?? false;

	public bool allTogglesAreOn => toggles?.All((UIToggle toggle) => toggle.isOn) ?? false;

	public bool allTogglesAreOff => toggles?.All((UIToggle toggle) => !toggle.isOn) ?? false;

	public IEnumerable<UIToggle> togglesOn => toggles?.Where((UIToggle toggle) => toggle.isOn);

	public IEnumerable<UIToggle> togglesOff => toggles?.Where((UIToggle toggle) => !toggle.isOn);

	public UIToggle firstToggleOn => toggles?.FirstOrDefault((UIToggle toggle) => toggle.isOn);

	public UIToggle firstToggleOff => toggles?.FirstOrDefault((UIToggle toggle) => !toggle.isOn);

	public UIToggle lastToggleOn => toggles?.LastOrDefault((UIToggle toggle) => toggle.isOn);

	public UIToggle lastToggleOff => toggles?.LastOrDefault((UIToggle toggle) => !toggle.isOn);

	public int firstToggleOnIndex
	{
		get
		{
			CleanToggles();
			UIToggle uIToggle = firstToggleOn;
			if (!(uIToggle == null))
			{
				return toggles.IndexOf(uIToggle);
			}
			return -1;
		}
	}

	public int firstToggleOffIndex
	{
		get
		{
			CleanToggles();
			UIToggle uIToggle = firstToggleOff;
			if (!(uIToggle == null))
			{
				return toggles.IndexOf(uIToggle);
			}
			return -1;
		}
	}

	public int lastToggleOnIndex
	{
		get
		{
			CleanToggles();
			UIToggle uIToggle = lastToggleOn;
			if (!(uIToggle == null))
			{
				return toggles.IndexOf(uIToggle);
			}
			return -1;
		}
	}

	public int lastToggleOffIndex
	{
		get
		{
			CleanToggles();
			UIToggle uIToggle = lastToggleOff;
			if (!(uIToggle == null))
			{
				return toggles.IndexOf(uIToggle);
			}
			return -1;
		}
	}

	private bool toggleGroupInitialized { get; set; }

	protected UIToggleGroup()
	{
		OnToggleGroupMixedValuesCallback = new ModyEvent("OnToggleGroupMixedValuesCallback");
	}

	protected override void Awake()
	{
		if (Application.isPlaying)
		{
			toggleGroupInitialized = false;
			base.Awake();
		}
	}

	protected override void OnEnable()
	{
		if (Application.isPlaying)
		{
			base.OnEnable();
			StartCoroutine(RefreshAllTogglesWithDelay());
		}
	}

	private IEnumerator RefreshAllTogglesWithDelay()
	{
		yield return null;
		RefreshAllToggleValues();
		toggleGroupInitialized = true;
	}

	protected override void OnDisable()
	{
		if (Application.isPlaying)
		{
			base.OnDisable();
			toggleGroupInitialized = false;
		}
	}

	private void LateUpdate()
	{
		if (!toggleGroupInitialized || !overrideInteractabilityForToggles)
		{
			return;
		}
		foreach (UIToggle toggle in toggles)
		{
			toggle.interactable = base.interactable;
		}
	}

	protected override void InitializeToggle()
	{
		if (!base.toggleInitialized)
		{
			base.toggleInitialized = true;
			AddToToggleGroup(base.toggleGroup);
			if (!base.inToggleGroup)
			{
				ValueChanged(isOn, isOn, animateChange: false, triggerValueChanged: false);
			}
		}
	}

	public UIToggleGroup CleanToggles()
	{
		toggles = toggles.Where((UIToggle toggle) => toggle != null).Distinct().ToList();
		return this;
	}

	public void AutoSortToggles()
	{
		SortToggles(autoSort);
	}

	public void SortToggles(SortMode toggleSortMode)
	{
		CleanToggles();
		switch (toggleSortMode)
		{
		case SortMode.Disabled:
			break;
		case SortMode.Hierarchy:
			toggles = toggles.OrderBy((UIToggle t) => t.rectTransform.GetSiblingIndex()).ToList();
			break;
		case SortMode.GameObjectName:
			toggles = toggles.OrderBy((UIToggle t) => t.gameObject.name).ToList();
			break;
		case SortMode.ToggleName:
			toggles = toggles.OrderBy((UIToggle t) => t.Id.Name).ToList();
			break;
		default:
			throw new ArgumentOutOfRangeException("toggleSortMode", toggleSortMode, null);
		}
	}

	public void AddToggle(UIToggle toggle)
	{
		if (!(toggle == null) && !(toggle == this) && !toggles.Contains(toggle))
		{
			toggles.Add(toggle);
			toggle.toggleGroup = this;
			OnToggleAddedCallback?.Invoke(toggle);
			if (toggleGroupInitialized)
			{
				AutoSortToggles();
				UpdateGroupValue(animateChange: true);
			}
		}
	}

	public void RemoveToggle(UIToggle toggle)
	{
		CleanToggles();
		if (!(toggle == null) && toggles.Contains(toggle))
		{
			toggles.Remove(toggle);
			toggle.toggleGroup = null;
			OnToggleRemovedCallback?.Invoke(toggle);
			UpdateGroupValue(animateChange: true);
		}
	}

	public void ToggleChangedValue(UIToggle toggle, bool animateChange = false, bool triggerValueChanged = true)
	{
		if (toggle == null)
		{
			return;
		}
		if (!toggles.Contains(toggle))
		{
			toggle.RemoveFromToggleGroup();
			return;
		}
		switch (mode)
		{
		case ControlMode.Passive:
			toggle.UpdateValueFromGroup(toggle.isOn, animateChange, triggerValueChanged);
			break;
		case ControlMode.OneToggleOn:
			if (toggle.isOn && numberOfTogglesOn > 1)
			{
				foreach (UIToggle item in toggles.Where((UIToggle t) => t != toggle && t.isOn))
				{
					item.UpdateValueFromGroup(newValue: false, animateChange, triggerValueChanged);
				}
			}
			toggle.UpdateValueFromGroup(toggle.isOn, animateChange, triggerValueChanged);
			break;
		case ControlMode.OneToggleOnEnforced:
			if (allTogglesAreOff)
			{
				toggle.UpdateValueFromGroup(newValue: true, animateChange, triggerValueChanged);
			}
			else if (toggle.isOn & (numberOfTogglesOn > 1))
			{
				foreach (UIToggle item2 in toggles.Where((UIToggle t) => t != toggle && t.isOn))
				{
					item2.UpdateValueFromGroup(newValue: false, animateChange, triggerValueChanged);
				}
				toggle.UpdateValueFromGroup(newValue: true, animateChange, triggerValueChanged);
			}
			else
			{
				toggle.UpdateValueFromGroup(toggle.isOn, animateChange, triggerValueChanged);
			}
			break;
		case ControlMode.AnyToggleOnEnforced:
			if (!toggle.isOn & allTogglesAreOff)
			{
				toggle.UpdateValueFromGroup(newValue: true, animateChange, triggerValueChanged);
			}
			else
			{
				toggle.UpdateValueFromGroup(toggle.isOn, animateChange, triggerValueChanged);
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		UpdateGroupValue(animateChange);
		OnToggleTriggeredCallback?.Invoke(toggle);
	}

	protected internal override void UpdateValueFromGroup(bool newValue, bool animateChange, bool triggerValueChanged = true)
	{
		switch (mode)
		{
		case ControlMode.Passive:
			if (newValue)
			{
				foreach (UIToggle toggle in toggles)
				{
					toggle.UpdateValueFromGroup(newValue: true, animateChange, triggerValueChanged);
				}
				break;
			}
			foreach (UIToggle toggle2 in toggles)
			{
				toggle2.UpdateValueFromGroup(newValue: false, animateChange, triggerValueChanged);
			}
			break;
		case ControlMode.OneToggleOn:
			if (newValue)
			{
				foreach (UIToggle toggle3 in toggles)
				{
					toggle3.UpdateValueFromGroup(newValue: false, animateChange, triggerValueChanged);
				}
			}
			else
			{
				toggles[0].UpdateValueFromGroup(newValue: true, animateChange, triggerValueChanged);
			}
			break;
		case ControlMode.AnyToggleOnEnforced:
		{
			if (newValue)
			{
				foreach (UIToggle toggle4 in toggles)
				{
					toggle4.UpdateValueFromGroup(newValue: true, animateChange, triggerValueChanged);
				}
				break;
			}
			UIToggle firstToggle = toggles[0];
			firstToggle.UpdateValueFromGroup(newValue: true, animateChange, triggerValueChanged);
			foreach (UIToggle item in toggles.Where((UIToggle t) => t != firstToggle))
			{
				item.UpdateValueFromGroup(newValue: false, animateChange, triggerValueChanged);
			}
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		case ControlMode.OneToggleOnEnforced:
			break;
		}
		UpdateGroupValue(animateChange);
	}

	public void RefreshAllToggleValues(bool animateChange = true, bool triggerValueChanged = true)
	{
		AutoSortToggles();
		if (toggles.Count == 0)
		{
			return;
		}
		bool flag;
		switch (mode)
		{
		case ControlMode.Passive:
			flag = false;
			foreach (UIToggle toggle in toggles)
			{
				toggle.UpdateValueFromGroup(toggle.isOn, animateChange: false, triggerValueChanged);
			}
			break;
		case ControlMode.OneToggleOn:
			flag = numberOfTogglesOn > 1;
			if (flag)
			{
				break;
			}
			foreach (UIToggle toggle2 in toggles)
			{
				toggle2.UpdateValueFromGroup(toggle2.isOn, animateChange: false, triggerValueChanged);
			}
			break;
		case ControlMode.OneToggleOnEnforced:
		{
			flag = numberOfTogglesOn == 0;
			if (numberOfTogglesOn <= 1)
			{
				break;
			}
			bool flag2 = false;
			foreach (UIToggle item in toggles.Where((UIToggle t) => t.isOn))
			{
				if (!flag2)
				{
					flag2 = true;
				}
				else
				{
					item.UpdateValueFromGroup(newValue: false, animateChange: false, triggerValueChanged);
				}
			}
			break;
		}
		case ControlMode.AnyToggleOnEnforced:
			flag = numberOfTogglesOn == 0;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (flag)
		{
			UIToggle firstToggle = GetFirstToggle();
			foreach (UIToggle item2 in toggles.Where((UIToggle t) => t != firstToggle))
			{
				item2.UpdateValueFromGroup(newValue: false, animateChange, triggerValueChanged);
			}
			if (firstToggle != null)
			{
				firstToggle.UpdateValueFromGroup(newValue: true, animateChange, triggerValueChanged);
			}
		}
		UpdateGroupValue(animateChange);
	}

	public UIToggle GetFirstToggle()
	{
		if (!(FirstToggle != null) || !toggles.Contains(FirstToggle))
		{
			if (toggles.Count != 0)
			{
				return toggles[0];
			}
			return null;
		}
		return FirstToggle;
	}

	private void SetAllTogglesOff(bool animateChange = false, bool triggerValueChanged = true)
	{
		foreach (UIToggle toggle in toggles)
		{
			toggle.UpdateValueFromGroup(newValue: false, animateChange, triggerValueChanged);
		}
	}

	protected override void ToggleValue()
	{
		if (!IsActive() || !IsInteractable())
		{
			return;
		}
		switch (mode)
		{
		case ControlMode.Passive:
			switch (toggleGroupValue)
			{
			case Value.Off:
			case Value.MixedValues:
				foreach (UIToggle toggle in toggles)
				{
					toggle.UpdateValueFromGroup(newValue: true, animateChange: true);
				}
				break;
			case Value.On:
				foreach (UIToggle toggle2 in toggles)
				{
					toggle2.UpdateValueFromGroup(newValue: false, animateChange: true);
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			break;
		case ControlMode.OneToggleOn:
			switch (toggleGroupValue)
			{
			case Value.Off:
				toggles[0].UpdateValueFromGroup(newValue: true, animateChange: true);
				break;
			case Value.On:
			case Value.MixedValues:
				foreach (UIToggle toggle3 in toggles)
				{
					toggle3.UpdateValueFromGroup(newValue: false, animateChange: true);
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			break;
		case ControlMode.AnyToggleOnEnforced:
			switch (toggleGroupValue)
			{
			case Value.On:
			{
				UIToggle firstToggle = toggles[0];
				firstToggle.UpdateValueFromGroup(newValue: true, animateChange: true);
				foreach (UIToggle item in toggles.Where((UIToggle item) => item != firstToggle))
				{
					item.UpdateValueFromGroup(newValue: false, animateChange: true);
				}
				break;
			}
			case Value.Off:
			case Value.MixedValues:
				foreach (UIToggle toggle4 in toggles)
				{
					toggle4.UpdateValueFromGroup(newValue: true, animateChange: true);
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case ControlMode.OneToggleOnEnforced:
			break;
		}
		UpdateGroupValue(animateChange: true);
		base.behaviours.GetBehaviour(UIBehaviour.Name.PointerClick)?.Execute();
		OnToggleTriggeredCallback?.Invoke(this);
	}

	public void UpdateGroupValue(bool animateChange, bool triggerValueChanged = true)
	{
		if (toggles.Count != 0)
		{
			if (allTogglesAreOn)
			{
				toggleGroupValue = Value.On;
			}
			else if (allTogglesAreOff)
			{
				toggleGroupValue = Value.Off;
			}
			else
			{
				toggleGroupValue = Value.MixedValues;
			}
			hasMixedValues = toggleGroupValue == Value.MixedValues;
			bool num = isOn;
			bool flag = anyOfTogglesOn;
			if (num != flag)
			{
				this.SetIsOn(flag, animateChange, triggerValueChanged);
			}
		}
	}
}
