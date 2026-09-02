using System;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.ScriptableObjects;

namespace Doozy.Runtime.UIManager;

[Serializable]
public struct UIToggleSignalData
{
	public static UIManagerInputSettings inputSettings => SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance;

	public static bool multiplayerMode => inputSettings.multiplayerMode;

	public string toggleCategory { get; private set; }

	public string toggleName { get; private set; }

	public CommandToggle state { get; private set; }

	public int playerIndex { get; private set; }

	public UIToggle toggle { get; private set; }

	public UIToggleSignalData(string toggleCategory, string toggleName, CommandToggle state, int playerIndex, UIToggle toggle)
	{
		this.toggleCategory = toggleCategory;
		this.toggleName = toggleName;
		this.state = state;
		this.playerIndex = playerIndex;
		this.toggle = toggle;
	}

	public override string ToString()
	{
		string text = ((multiplayerMode && playerIndex != inputSettings.defaultPlayerIndex) ? $"Player {playerIndex} > " : string.Empty);
		text = text + "(" + ObjectNames.NicifyVariableName(state.ToString()) + ") ";
		return text + " " + toggleCategory + " / " + toggleName;
	}
}
