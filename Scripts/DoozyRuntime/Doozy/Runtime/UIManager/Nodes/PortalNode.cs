using System;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.Internal;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Nodes.Listeners;

namespace Doozy.Runtime.UIManager.Nodes;

[Serializable]
[NodyMenuPath("UI Manager", "Portal")]
public sealed class PortalNode : GlobalNode
{
	public enum TriggerCondition
	{
		Signal = 0,
		UIButton = 1,
		UIToggle = 2,
		UIView = 3
	}

	public TriggerCondition Trigger;

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

	private StreamNodyListener streamListener { get; set; }

	private UIButtonNodyListener uiButtonListener { get; set; }

	private UIToggleNodyListener uiToggleListener { get; set; }

	private UIViewNodyListener uiViewListener { get; set; }

	public override bool showClearGraphHistoryInEditor => true;

	public PortalNode()
	{
		Trigger = TriggerCondition.Signal;
		SignalPayload = new SignalPayload();
		ButtonId = new UIButtonId();
		ToggleId = new UIToggleId();
		CommandToggle = CommandToggle.Any;
		ViewId = new UIViewId();
		CommandShowHide = CommandShowHide.Show;
		AddOutputPort().SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
	}

	public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
	{
		base.OnEnter(previousNode, previousPort);
		GoToNextNode(base.firstOutputPort);
	}

	public override void OnExit()
	{
		base.OnExit();
		base.nodeState = NodeState.Running;
	}

	public override void Start()
	{
		base.Start();
		StartListeners();
	}

	public override void Stop()
	{
		base.Stop();
		StopListeners();
	}

	private void StartListeners()
	{
		if (streamListener == null)
		{
			StreamNodyListener streamNodyListener = (streamListener = new StreamNodyListener(this, SignalPayload, OnSignal));
		}
		streamListener.Start();
		if (uiButtonListener == null)
		{
			UIButtonNodyListener uIButtonNodyListener = (uiButtonListener = new UIButtonNodyListener(this, OnUIButtonSignal));
		}
		uiButtonListener.Start();
		if (uiToggleListener == null)
		{
			UIToggleNodyListener uIToggleNodyListener = (uiToggleListener = new UIToggleNodyListener(this, OnUIToggleSignal));
		}
		uiToggleListener.Start();
		if (uiViewListener == null)
		{
			UIViewNodyListener uIViewNodyListener = (uiViewListener = new UIViewNodyListener(this, OnUIViewSignal));
		}
		uiViewListener.Start();
	}

	private void StopListeners()
	{
		streamListener?.Stop();
		uiButtonListener?.Stop();
		uiToggleListener?.Stop();
		uiViewListener?.Stop();
	}

	private void OnSignal()
	{
		if (Trigger == TriggerCondition.Signal)
		{
			base.flowGraph.SetActiveNode(this);
		}
	}

	private void OnUIButtonSignal(UIButtonSignalData data)
	{
		if (Trigger != TriggerCondition.UIButton)
		{
			return;
		}
		if (isBackButton && data.isBackButton)
		{
			if (!base.multiplayerMode || base.playerIndex == data.playerIndex)
			{
				base.flowGraph.SetActiveNode(this);
			}
		}
		else if (ButtonId.Category.Equals(data.buttonCategory) && ButtonId.Name.Equals(data.buttonName) && (!base.multiplayerMode || base.playerIndex == data.playerIndex))
		{
			base.flowGraph.SetActiveNode(this);
		}
	}

	private void OnUIToggleSignal(UIToggleSignalData data)
	{
		if (Trigger == TriggerCondition.UIToggle && CommandToggle == data.state && ToggleId.Category.Equals(data.toggleCategory) && ToggleId.Name.Equals(data.toggleName) && (!base.multiplayerMode || base.playerIndex == data.playerIndex))
		{
			base.flowGraph.SetActiveNode(this);
		}
	}

	private void OnUIViewSignal(UIViewSignalData data)
	{
		if (Trigger != TriggerCondition.UIView)
		{
			return;
		}
		switch (CommandShowHide)
		{
		case CommandShowHide.Show:
		{
			ShowHideExecute execute = data.execute;
			if (execute == ShowHideExecute.Hide || (uint)(execute - 3) <= 2u)
			{
				return;
			}
			break;
		}
		case CommandShowHide.Hide:
			switch (data.execute)
			{
			case ShowHideExecute.Show:
			case ShowHideExecute.InstantShow:
			case ShowHideExecute.ReverseShow:
			case ShowHideExecute.ReverseHide:
				return;
			}
			break;
		}
		if (ViewId.Category.Equals(data.viewCategory) && ViewId.Name.Equals(data.viewName) && (!base.multiplayerMode || base.playerIndex == data.playerIndex))
		{
			base.flowGraph.SetActiveNode(this);
		}
	}

	public string InfoString()
	{
		return Trigger switch
		{
			TriggerCondition.Signal => $"{SignalPayload}", 
			TriggerCondition.UIButton => ButtonId.Name.Equals("Back") ? "'Back'" : $"{ButtonId}", 
			TriggerCondition.UIToggle => $"({CommandToggle}) {ToggleId}", 
			TriggerCondition.UIView => $"({CommandShowHide}) " + ((ViewId.Category.IsNullOrEmpty() & ViewId.Name.IsNullOrEmpty()) ? "All Views" : (ViewId.Name.IsNullOrEmpty() ? (ViewId.Category + " category") : $"{ViewId}")), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
