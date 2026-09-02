using System;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.ScriptableObjects;
using UnityEngine;

namespace Doozy.Runtime.UIManager.ScriptableObjects;

[Serializable]
[CreateAssetMenu(menuName = "Doozy/Links/UIPopup Link", fileName = "UIPopup Link", order = -900)]
public class UIPopupLink : PrefabLink
{
	private const string PREFIX = "UIPopup - ";

	public UIPopupLink()
		: this(null)
	{
	}

	public UIPopupLink(GameObject prefab, string prefabName = null)
		: base(prefab, prefabName)
	{
	}

	public override void Validate()
	{
		if (!base.hasPrefab)
		{
			base.prefabName = string.Empty;
			base.name = "UIPopupLink";
			SingletonRuntimeScriptableObject<UIPopupDatabase>.instance.Remove(this);
			return;
		}
		if (base.prefabName.Equals("None"))
		{
			SingletonRuntimeScriptableObject<UIPopupDatabase>.instance.Remove(this);
			Debug.LogError("[UIPopupLink]: [" + base.prefabName + "] - The prefabName cannot be the same as the default popup name (None). Rename the prefab to something else.");
			return;
		}
		bool flag = false;
		if (!base.prefabName.Equals(base.prefab.name))
		{
			base.prefabName = base.prefab.name.RemoveWhitespaces().RemoveAllSpecialCharacters();
			flag = true;
		}
		if (!base.name.Equals("UIPopup - " + base.prefab.name))
		{
			base.name = "UIPopup - " + base.prefab.name;
			flag = true;
		}
	}
}
