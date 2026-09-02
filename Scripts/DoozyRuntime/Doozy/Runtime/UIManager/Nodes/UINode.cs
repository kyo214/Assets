using System;
using System.Collections.Generic;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.Internal;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Nodes.Listeners;
using Doozy.Runtime.UIManager.Nodes.PortData;

namespace Doozy.Runtime.UIManager.Nodes;

[Serializable]
[NodyMenuPath("UI Manager", "UI")]
public sealed class UINode : SimpleNode
{
	public List<UIViewShowHideOption> OnEnterShowViews = new List<UIViewShowHideOption>();

	public List<UIViewShowHideOption> OnEnterHideViews = new List<UIViewShowHideOption>();

	public List<UIViewShowHideOption> OnExitShowViews = new List<UIViewShowHideOption>();

	public List<UIViewShowHideOption> OnExitHideViews = new List<UIViewShowHideOption>();

	public bool OnEnterHideAllViews;

	public bool OnExitHideAllViews;

	private List<StreamNodyListener> streamListeners { get; set; }

	private BackButtonNodyListener backButtonListener { get; set; }

	private UIButtonNodyListener uiButtonListener { get; set; }

	private UIToggleNodyListener uiToggleListener { get; set; }

	private UIViewNodyListener uiViewListener { get; set; }

	private FloatReaction timerReaction { get; set; }

	public override bool showPassthroughInEditor => true;

	public override bool showClearGraphHistoryInEditor => true;

	public bool canGoBack => base.firstInputPort.GetValue<GoBackInputPortData>().CanGoBack;

	public UINode()
	{
		AddInputPort().SetValue(new GoBackInputPortData()).SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
		AddOutputPort().SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
		base.passthrough = false;
	}

	public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
	{
		base.OnEnter(previousNode, previousPort);
		if (OnEnterHideAllViews)
		{
			UIView.HideAllViews();
		}
		OnEnterShowViews.ForEach((UIViewShowHideOption v) =>
		{
			v.Show(base.flowGraph.controller.playerIndex);
		});
		OnEnterHideViews.ForEach((UIViewShowHideOption v) =>
		{
			v.Hide(base.flowGraph.controller.playerIndex);
		});
		StartListeners();
		StartTimer();
	}

	public override void OnExit()
	{
		base.OnExit();
		StopTimer();
		StopListeners();
		if (OnExitHideAllViews)
		{
			UIView.HideAllViews(instant: false, base.flowGraph.controller.playerIndex);
		}
		OnExitShowViews.ForEach((UIViewShowHideOption v) =>
		{
			v.Show(base.flowGraph.controller.playerIndex);
		});
		OnExitHideViews.ForEach((UIViewShowHideOption v) =>
		{
			v.Hide(base.flowGraph.controller.playerIndex);
		});
	}

	private void StartTimer()
	{
		FlowPort targetPort = null;
		float num = 10000f;
		foreach (FlowPort outputPort in base.outputPorts)
		{
			UIOutputPortData value = outputPort.GetValue<UIOutputPortData>();
			if (value.Trigger == UIOutputPortData.TriggerCondition.TimeDelay && num > value.TimeDelay)
			{
				num = value.TimeDelay;
				targetPort = outputPort;
			}
		}
		if (targetPort == null)
		{
			return;
		}
		if (num <= 0f)
		{
			GoToNextNode(targetPort);
			return;
		}
		timerReaction = Reaction.Get<FloatReaction>().SetEase(Ease.Linear).SetDuration(num)
			.SetOnFinishCallback(() =>
			{
				GoToNextNode(targetPort);
			});
		timerReaction.Play();
	}

	private void StopTimer()
	{
		timerReaction?.Recycle();
	}

	private void StartListeners()
	{
		if (backButtonListener == null)
		{
			BackButtonNodyListener backButtonNodyListener = (backButtonListener = new BackButtonNodyListener(this, OnBackButton));
		}
		backButtonListener.Start();
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
		if (streamListeners == null)
		{
			List<StreamNodyListener> list = (streamListeners = new List<StreamNodyListener>());
		}
		foreach (FlowPort port in base.outputPorts)
		{
			UIOutputPortData value = port.GetValue<UIOutputPortData>();
			if (value.Trigger == UIOutputPortData.TriggerCondition.Signal)
			{
				StreamNodyListener streamNodyListener = new StreamNodyListener(this, value.SignalPayload, () =>
				{
					GoToNextNode(port);
				});
				streamListeners.Add(streamNodyListener);
				streamNodyListener.Start();
			}
		}
	}

	private void StopListeners()
	{
		backButtonListener?.Stop();
		uiButtonListener?.Stop();
		uiToggleListener?.Stop();
		uiViewListener?.Stop();
		streamListeners?.ForEach((StreamNodyListener listener) =>
		{
			listener.Stop();
		});
		streamListeners?.Clear();
	}

	private void OnBackButton(Signal signal)
	{
		if (base.multiplayerMode && signal.hasValue && signal.valueAsObject is InputSignalData inputSignalData)
		{
			if (canGoBack)
			{
				base.flowGraph.GoBack(inputSignalData.playerIndex);
				return;
			}
			{
				foreach (FlowPort outputPort in base.outputPorts)
				{
					UIOutputPortData value = outputPort.GetValue<UIOutputPortData>();
					if (value.Trigger == UIOutputPortData.TriggerCondition.UIButton && value.isBackButton)
					{
						GoToNextNode(outputPort);
						break;
					}
				}
				return;
			}
		}
		if (canGoBack)
		{
			base.flowGraph.GoBack();
			return;
		}
		foreach (FlowPort outputPort2 in base.outputPorts)
		{
			UIOutputPortData value2 = outputPort2.GetValue<UIOutputPortData>();
			if (value2.Trigger == UIOutputPortData.TriggerCondition.UIButton && value2.isBackButton)
			{
				GoToNextNode(outputPort2);
				break;
			}
		}
	}

	private void OnUIButtonSignal(UIButtonSignalData data)
	{
		foreach (FlowPort outputPort in base.outputPorts)
		{
			UIOutputPortData value = outputPort.GetValue<UIOutputPortData>();
			if (value.Trigger != UIOutputPortData.TriggerCondition.UIButton)
			{
				continue;
			}
			if (value.isBackButton && data.isBackButton)
			{
				if (!base.multiplayerMode || base.playerIndex == data.playerIndex)
				{
					if (canGoBack)
					{
						base.flowGraph.GoBack(data.playerIndex);
					}
					else
					{
						GoToNextNode(outputPort);
					}
					break;
				}
			}
			else if (value.ButtonId.Category.Equals(data.buttonCategory) && value.ButtonId.Name.Equals(data.buttonName) && (!base.multiplayerMode || base.playerIndex == data.playerIndex))
			{
				GoToNextNode(outputPort);
				break;
			}
		}
	}

	private void OnUIToggleSignal(UIToggleSignalData data)
	{
		foreach (FlowPort outputPort in base.outputPorts)
		{
			UIOutputPortData value = outputPort.GetValue<UIOutputPortData>();
			if (value.Trigger == UIOutputPortData.TriggerCondition.UIToggle && (value.CommandToggle == CommandToggle.Any || value.CommandToggle == data.state) && value.ToggleId.Category.Equals(data.toggleCategory) && value.ToggleId.Name.Equals(data.toggleName) && (!base.multiplayerMode || base.playerIndex == data.playerIndex))
			{
				GoToNextNode(outputPort);
				break;
			}
		}
	}

	private void OnUIViewSignal(UIViewSignalData data)
	{
		foreach (FlowPort outputPort in base.outputPorts)
		{
			UIOutputPortData value = outputPort.GetValue<UIOutputPortData>();
			if (value.Trigger != UIOutputPortData.TriggerCondition.UIView)
			{
				continue;
			}
			switch (value.CommandShowHide)
			{
			case CommandShowHide.Show:
			{
				ShowHideExecute execute = data.execute;
				if (execute == ShowHideExecute.Hide || (uint)(execute - 3) <= 2u)
				{
					continue;
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
					continue;
				}
				break;
			}
			if (value.ViewId.Category.Equals(data.viewCategory) && value.ViewId.Name.Equals(data.viewName) && (!base.multiplayerMode || base.playerIndex == data.playerIndex))
			{
				GoToNextNode(outputPort);
				break;
			}
		}
	}

	public override FlowPort AddOutputPort(PortCapacity capacity = PortCapacity.Single)
	{
		return base.AddOutputPort(capacity).SetValue(new UIOutputPortData());
	}
}
