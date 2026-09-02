using System;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.UIManager.ScriptableObjects.Internal;

namespace Doozy.Runtime.UIManager.ScriptableObjects;

[Serializable]
public class UIPopupDatabase : PrefabLinkDatabase<UIPopupDatabase, UIPopupLink>
{
	public override string defaultLinkName => "None";

	public override string databaseName => "UIPopup";

	[RestoreData("UIPopupDatabase")]
	public static UIPopupDatabase RestoreData()
	{
		return SingletonRuntimeScriptableObject<UIPopupDatabase>.instance;
	}

	[RefreshData("UIPopupDatabase")]
	public static void RefreshData()
	{
		SingletonRuntimeScriptableObject<UIPopupDatabase>.instance.RefreshDatabase();
	}
}
