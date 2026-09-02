using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.Listeners.Internal;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Listeners;

[AddComponentMenu("UI/Components/Listeners/UIToggle Listener")]
public class UIToggleListener : BaseListener, IUseMultiplayerInfo
{
	[SerializeField]
	private UIToggleId ToggleId;

	[SerializeField]
	private CommandToggle Command;

	[SerializeField]
	private MultiplayerInfo MultiplayerInfo;

	public static UIManagerInputSettings inputSettings => SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance;

	public static bool multiplayerMode => inputSettings.multiplayerMode;

	public UIToggleId toggleId => ToggleId;

	public CommandToggle command => Command;

	public UnityAction<UIToggleSignalData> signalCallback { get; }

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
		UIToggle.stream.ConnectReceiver(base.receiver);
	}

	protected override void DisconnectReceiver()
	{
		UIToggle.stream.DisconnectReceiver(base.receiver);
	}

	protected override void ProcessSignal(Signal signal)
	{
		if (signal.hasValue && signal.valueAsObject is UIToggleSignalData arg && (Command == CommandToggle.Any || Command == arg.state) && ToggleId.Category.Equals(arg.toggleCategory) && ToggleId.Name.Equals(arg.toggleName) && (!multiplayerMode || playerIndex == arg.playerIndex))
		{
			signalCallback?.Invoke(arg);
			Callback?.Execute();
		}
	}
}
