using System;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Signals;

namespace Doozy.Runtime.UIManager.Nodes.PortData;

[Serializable]
public class UIOutputPortData
{
	public enum TriggerCondition
	{
		TimeDelay = 0,
		Signal = 1,
		UIButton = 2,
		UIToggle = 3,
		UIView = 4
	}

	public TriggerCondition Trigger;

	public float TimeDelay;

	public SignalPayload SignalPayload;

	public UIButtonId ButtonId;

	public UIToggleId ToggleId;

	public CommandToggle CommandToggle;

	public UIViewId ViewId;

	public CommandShowHide CommandShowHide;

	public bool isBackButton
	{
		get
		{
			if (Trigger == TriggerCondition.UIButton)
			{
				return ButtonId.Name.Equals("Back");
			}
			return false;
		}
	}

	public bool viewsCategory
	{
		get
		{
			if (Trigger == TriggerCondition.UIView)
			{
				return ViewId.Name.IsNullOrEmpty();
			}
			return false;
		}
	}

	public bool allViews
	{
		get
		{
			if (Trigger == TriggerCondition.UIView && ViewId.Category.IsNullOrEmpty())
			{
				return ViewId.Name.IsNullOrEmpty();
			}
			return false;
		}
	}

	public UIOutputPortData()
	{
		Trigger = TriggerCondition.TimeDelay;
		TimeDelay = 3f;
		SignalPayload = new SignalPayload();
		ButtonId = new UIButtonId();
		ToggleId = new UIToggleId();
		CommandToggle = CommandToggle.Any;
		ViewId = new UIViewId();
		CommandShowHide = CommandShowHide.Show;
	}

	public override string ToString()
	{
		return Trigger switch
		{
			TriggerCondition.TimeDelay => $"{TimeDelay.Round(2)} s", 
			TriggerCondition.Signal => $"{SignalPayload}", 
			TriggerCondition.UIButton => ButtonId.Name.Equals("Back") ? "'Back'" : $"{ButtonId}", 
			TriggerCondition.UIToggle => $"({CommandToggle}) {ToggleId}", 
			TriggerCondition.UIView => $"({CommandShowHide}) " + ((ViewId.Category.IsNullOrEmpty() & ViewId.Name.IsNullOrEmpty()) ? "All Views" : (ViewId.Name.IsNullOrEmpty() ? (ViewId.Category + " category") : $"{ViewId}")), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
