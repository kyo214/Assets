using System;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.ScriptableObjects;
using UnityEngine;

namespace Doozy.Runtime.UIManager.ScriptableObjects;

[Serializable]
[CreateAssetMenu(menuName = "Doozy/Links/UITooltip Link", fileName = "UITooltip Link", order = -900)]
public class UITooltipLink : PrefabLink
{
	private const string PREFIX = "UITooltip - ";

	public UITooltipLink()
		: this(null)
	{
	}

	public UITooltipLink(GameObject prefab, string prefabName = null)
		: base(prefab, prefabName)
	{
	}

	public override void Validate()
	{
		if (!base.hasPrefab)
		{
			base.prefabName = string.Empty;
			base.name = "UIPopupLink";
			SingletonRuntimeScriptableObject<UITooltipDatabase>.instance.Remove(this);
			return;
		}
		if (base.prefabName.Equals("None"))
		{
			SingletonRuntimeScriptableObject<UITooltipDatabase>.instance.Remove(this);
			Debug.LogError("[UITooltipLink]: [" + base.prefabName + "] - The prefabName cannot be the same as the default tooltip name (None).Rename the prefab to something else.");
			return;
		}
		bool flag = false;
		if (!base.prefabName.Equals(base.prefab.name))
		{
			base.prefabName = base.prefab.name.RemoveWhitespaces().RemoveAllSpecialCharacters();
			flag = true;
		}
		if (!base.name.Equals("UITooltip - " + base.prefab.name))
		{
			base.name = "UITooltip - " + base.prefab.name;
			flag = true;
		}
	}
}
