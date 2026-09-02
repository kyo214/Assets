using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Components;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Components/UITag")]
public class UITag : MonoBehaviour
{
	public UITagId Id;

	[ClearOnReload]
	public static HashSet<UITag> database { get; private set; } = new HashSet<UITag>();

	[ExecuteOnReload]
	private static void OnReload()
	{
		if (database == null)
		{
			database = new HashSet<UITag>();
		}
	}

	protected UITag()
	{
		Id = new UITagId();
	}

	protected virtual void Awake()
	{
		database.Add(this);
	}

	protected virtual void OnEnable()
	{
		database.Remove(null);
	}

	protected virtual void OnDestroy()
	{
		database.Remove(this);
		database.Remove(null);
	}

	public static UITag GetFirstTag(string category, string name)
	{
		return database.FirstOrDefault((UITag tag) => tag.Id.Category == category && tag.Id.Name == name);
	}

	public static IEnumerable<UITag> GetTags(string category, string name)
	{
		return database.Where((UITag item) => item.Id.Category.Equals(category) && item.Id.Name.Equals(name));
	}

	public static IEnumerable<UITag> GetAllTagsInCategory(string category)
	{
		return database.Where((UITag item) => item.Id.Category.Equals(category));
	}
}
