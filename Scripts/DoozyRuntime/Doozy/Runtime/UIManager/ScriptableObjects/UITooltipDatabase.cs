using System;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.UIManager.ScriptableObjects.Internal;

namespace Doozy.Runtime.UIManager.ScriptableObjects;

[Serializable]
public class UITooltipDatabase : PrefabLinkDatabase<UITooltipDatabase, UITooltipLink>
{
	public override string defaultLinkName => "None";

	public override string databaseName => "UITooltip";

	[RestoreData("UITooltipDatabase")]
	public static UITooltipDatabase Get()
	{
		return SingletonRuntimeScriptableObject<UITooltipDatabase>.instance;
	}

	[RefreshData("UITooltipDatabase")]
	public static void RefreshData()
	{
		SingletonRuntimeScriptableObject<UITooltipDatabase>.instance.RefreshDatabase();
	}
}
