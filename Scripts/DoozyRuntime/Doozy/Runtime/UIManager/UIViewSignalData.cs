using System;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIManager.ScriptableObjects;

namespace Doozy.Runtime.UIManager;

[Serializable]
public struct UIViewSignalData
{
	public static UIManagerInputSettings inputSettings => SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance;

	public static bool multiplayerMode => inputSettings.multiplayerMode;

	public string viewCategory { get; private set; }

	public string viewName { get; private set; }

	public ShowHideExecute execute { get; private set; }

	public int playerIndex { get; private set; }

	public bool globalCommand => viewCategory.IsNullOrEmpty();

	public bool categoryCommand => viewName.IsNullOrEmpty();

	public UIViewSignalData(string viewCategory, string viewName, ShowHideExecute execute, int playerIndex)
	{
		this.viewCategory = viewCategory;
		this.viewName = viewName;
		this.execute = execute;
		this.playerIndex = playerIndex;
	}

	public override string ToString()
	{
		string text = ((multiplayerMode && playerIndex != inputSettings.defaultPlayerIndex) ? $"Player {playerIndex} > " : string.Empty);
		text = text + "(" + ObjectNames.NicifyVariableName(execute.ToString()) + ") ";
		if (!globalCommand)
		{
			if (!categoryCommand)
			{
				return text + " " + viewCategory + " / " + viewName;
			}
			return text + " " + viewCategory + " category";
		}
		return text + " All Views";
	}
}
