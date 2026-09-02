using System;
using System.Collections;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Components;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Components/UITab")]
[SelectionBase]
public class UITab : UIToggle
{
	[SerializeField]
	private UIContainer TargetContainer;

	public UIContainer targetContainer
	{
		get
		{
			return TargetContainer;
		}
		set
		{
			if (!(TargetContainer == value))
			{
				DisconnectFromContainer();
				TargetContainer = value;
				ConnectToContainer(updateIsOn: true);
			}
		}
	}

	private bool isConnectedToContainer { get; set; }

	private void ConnectToContainer(bool updateIsOn)
	{
		if (TargetContainer == null)
		{
			isConnectedToContainer = false;
		}
		else if (!isConnectedToContainer)
		{
			TargetContainer.OnShowCallback.Event.AddListener(UpdateIsOnFromContainer);
			TargetContainer.OnHideCallback.Event.AddListener(UpdateIsOnFromContainer);
			if (updateIsOn)
			{
				UpdateIsOnFromContainer();
			}
			isConnectedToContainer = true;
		}
	}

	private void DisconnectFromContainer()
	{
		if (!(TargetContainer == null))
		{
			TargetContainer.OnShowCallback.Event.RemoveListener(UpdateIsOnFromContainer);
			TargetContainer.OnHideCallback.Event.RemoveListener(UpdateIsOnFromContainer);
			TargetContainer = null;
			isOn = false;
			isConnectedToContainer = false;
		}
	}

	private void UpdateIsOnFromContainer()
	{
		switch (TargetContainer.visibilityState)
		{
		case VisibilityState.Visible:
		case VisibilityState.IsShowing:
			if (!isOn)
			{
				isOn = true;
			}
			break;
		case VisibilityState.Hidden:
		case VisibilityState.IsHiding:
			if (isOn)
			{
				isOn = false;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	protected override void Awake()
	{
		isConnectedToContainer = false;
		base.Awake();
		ConnectToContainer(updateIsOn: false);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		StartCoroutine("UpdateConnection");
	}

	internal override void ValueChanged(bool previousValue, bool newValue, bool animateChange, bool triggerValueChanged)
	{
		base.ValueChanged(previousValue, newValue, animateChange, triggerValueChanged);
		if (!isConnectedToContainer)
		{
			return;
		}
		if (isOn)
		{
			if (animateChange)
			{
				TargetContainer.Show();
			}
			else
			{
				TargetContainer.InstantShow();
			}
		}
		else if (animateChange)
		{
			TargetContainer.Hide();
		}
		else
		{
			TargetContainer.InstantHide();
		}
	}

	private IEnumerable UpdateConnection()
	{
		yield return null;
		ConnectToContainer(updateIsOn: false);
		if (base.inToggleGroup)
		{
			ValueChanged(isOn, isConnectedToContainer && isOn, animateChange: false, triggerValueChanged: false);
		}
		else
		{
			UpdateIsOnFromContainer();
		}
	}
}
