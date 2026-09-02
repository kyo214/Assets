using System;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.ScriptableObjects;

namespace Doozy.Runtime.UIManager;

[Serializable]
public struct UIButtonSignalData
{
	public static UIManagerInputSettings inputSettings => SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance;

	public static bool multiplayerMode => inputSettings.multiplayerMode;

	public string buttonCategory { get; private set; }

	public string buttonName { get; private set; }

	public ButtonTrigger trigger { get; private set; }

	public int playerIndex { get; private set; }

	public UIButton button { get; private set; }

	public bool isBackButton => buttonName.Equals("Back");

	public UIButtonSignalData(string buttonCategory, string buttonName, ButtonTrigger trigger, UIButton button = null)
		: this(buttonCategory, buttonName, trigger, inputSettings.defaultPlayerIndex, button)
	{
	}

	public UIButtonSignalData(string buttonCategory, string buttonName, ButtonTrigger trigger, int playerIndex, UIButton button = null)
	{
		this.buttonCategory = buttonCategory;
		this.buttonName = buttonName;
		this.trigger = trigger;
		this.playerIndex = playerIndex;
		this.button = button;
	}

	public override string ToString()
	{
		string text = ((multiplayerMode && playerIndex != inputSettings.defaultPlayerIndex) ? $"Player {playerIndex} > " : string.Empty);
		text = text + "(" + ObjectNames.NicifyVariableName(trigger.ToString()) + ") ";
		return text + " " + buttonCategory + " / " + buttonName;
	}
}
