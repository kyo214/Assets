using System.Collections;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Animators;

public abstract class BaseUISelectableAnimator : BaseTargetComponentAnimator<UISelectable>
{
	public bool IsOn;

	public CommandToggle ToggleCommand = CommandToggle.Any;

	public bool controllerIsButton
	{
		get
		{
			if (base.hasController)
			{
				return base.controller.isButton;
			}
			return false;
		}
	}

	public bool controllerIsToggle
	{
		get
		{
			if (base.hasController)
			{
				return base.controller.isToggle;
			}
			return false;
		}
	}

	protected override void ConnectToController()
	{
		if (!(base.controller == null))
		{
			UISelectable uISelectable = base.controller;
			if (uISelectable.OnSelectionStateChangedCallback == null)
			{
				uISelectable.OnSelectionStateChangedCallback = new UISelectionStateEvent();
			}
			base.controller.OnSelectionStateChangedCallback.AddListener(OnSelectionStateChanged);
			StartCoroutine(UpdateStateLater());
		}
	}

	protected override void DisconnectFromController()
	{
		if (!(base.controller == null))
		{
			UISelectable uISelectable = base.controller;
			if (uISelectable.OnSelectionStateChangedCallback == null)
			{
				uISelectable.OnSelectionStateChangedCallback = new UISelectionStateEvent();
			}
			base.controller.OnSelectionStateChangedCallback.RemoveListener(OnSelectionStateChanged);
		}
	}

	protected virtual void OnSelectionStateChanged(UISelectionState state)
	{
		if (base.controller == null)
		{
			return;
		}
		if (controllerIsToggle)
		{
			switch (ToggleCommand)
			{
			case CommandToggle.On:
				if (base.controller.isOn)
				{
					break;
				}
				return;
			case CommandToggle.Off:
				if (base.controller.isOn)
				{
					return;
				}
				break;
			}
		}
		if (IsStateEnabled(state) && Application.isPlaying)
		{
			StopAllReactions();
			Play(state);
		}
	}

	public abstract bool IsStateEnabled(UISelectionState state);

	public abstract void Play(UISelectionState state);

	private IEnumerator UpdateStateLater()
	{
		yield return new WaitForEndOfFrame();
		OnSelectionStateChanged(base.controller.currentUISelectionState);
	}
}
