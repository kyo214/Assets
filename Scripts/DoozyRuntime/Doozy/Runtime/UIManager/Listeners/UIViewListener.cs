using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.Listeners.Internal;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Listeners;

[AddComponentMenu("UI/Containers/Listeners/UIView Listener")]
public class UIViewListener : BaseListener, IUseMultiplayerInfo
{
	[SerializeField]
	private UIViewId ViewId;

	[SerializeField]
	private CommandShowHide Command;

	[SerializeField]
	private MultiplayerInfo MultiplayerInfo;

	public static UIManagerInputSettings inputSettings => SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance;

	public static bool multiplayerMode => inputSettings.multiplayerMode;

	public UIViewId viewId => ViewId;

	public CommandShowHide command => Command;

	public UnityAction<UIViewSignalData> signalCallback { get; }

	public MultiplayerInfo multiplayerInfo => MultiplayerInfo;

	public bool hasMultiplayerInfo => multiplayerInfo != null;

	public int playerIndex
	{
		get
		{
			if (!(multiplayerMode & hasMultiplayerInfo))
			{
				return inputSettings.defaultPlayerIndex;
			}
			return multiplayerInfo.playerIndex;
		}
	}

	public void SetMultiplayerInfo(MultiplayerInfo reference)
	{
		MultiplayerInfo = reference;
	}

	private void OnEnable()
	{
		ConnectReceiver();
	}

	private void OnDisable()
	{
		DisconnectReceiver();
	}

	protected override void ConnectReceiver()
	{
		UIView.stream.ConnectReceiver(base.receiver);
	}

	protected override void DisconnectReceiver()
	{
		UIView.stream.DisconnectReceiver(base.receiver);
	}

	protected override void ProcessSignal(Signal signal)
	{
		if (!signal.hasValue || !(signal.valueAsObject is UIViewSignalData arg))
		{
			return;
		}
		switch (Command)
		{
		case CommandShowHide.Show:
		{
			ShowHideExecute execute = arg.execute;
			if (execute == ShowHideExecute.Hide || (uint)(execute - 3) <= 2u)
			{
				return;
			}
			break;
		}
		case CommandShowHide.Hide:
			switch (arg.execute)
			{
			case ShowHideExecute.Show:
			case ShowHideExecute.InstantShow:
			case ShowHideExecute.ReverseShow:
			case ShowHideExecute.ReverseHide:
				return;
			}
			break;
		}
		if ((ViewId.Category.IsNullOrEmpty() || (ViewId.Category.Equals(arg.viewCategory) && (ViewId.Name.IsNullOrEmpty() || ViewId.Name.Equals(arg.viewName)))) && (!multiplayerMode || playerIndex == arg.playerIndex))
		{
			signalCallback?.Invoke(arg);
			Callback?.Execute();
		}
	}
}
