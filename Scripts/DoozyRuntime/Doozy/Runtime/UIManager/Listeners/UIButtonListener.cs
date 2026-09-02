using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.Listeners.Internal;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Listeners;

[AddComponentMenu("UI/Components/UIButton Listener")]
public class UIButtonListener : BaseListener, IUseMultiplayerInfo
{
	[SerializeField]
	private UIButtonId ButtonId;

	[SerializeField]
	private MultiplayerInfo MultiplayerInfo;

	public static UIManagerInputSettings inputSettings => SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance;

	public static bool multiplayerMode => inputSettings.multiplayerMode;

	public UIButtonId buttonId => ButtonId;

	public UnityAction<UIButtonSignalData> signalCallback { get; }

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
		UIButton.stream.ConnectReceiver(base.receiver);
	}

	protected override void DisconnectReceiver()
	{
		UIButton.stream.DisconnectReceiver(base.receiver);
	}

	protected override void ProcessSignal(Signal signal)
	{
		if (signal.hasValue && signal.valueAsObject is UIButtonSignalData arg && ButtonId.Category.Equals(arg.buttonCategory) && ButtonId.Name.Equals(arg.buttonName) && (!multiplayerMode || playerIndex == arg.playerIndex))
		{
			signalCallback?.Invoke(arg);
			Callback?.Execute();
		}
	}
}
