using System;
using System.Collections.Generic;
using UnityEngine;

namespace DG.DemiLib.External;

public class DeHierarchyComponent : MonoBehaviour
{
	public enum HColor
	{
		None = 0,
		Blue = 1,
		Green = 2,
		Orange = 3,
		Purple = 4,
		Red = 5,
		Yellow = 6,
		BrightGrey = 7,
		DarkGrey = 8,
		Black = 9,
		White = 10,
		Pink = 11
	}

	public enum IcoType
	{
		Dot = 0,
		Star = 1,
		Cog = 2,
		Comment = 3,
		UI = 4,
		Play = 5,
		Heart = 6,
		Skull = 7,
		Camera = 8,
		Light = 9
	}

	public enum SeparatorType
	{
		None = 0,
		Top = 1,
		Bottom = 2
	}

	[Serializable]
	public class CustomizedItem
	{
		public GameObject gameObject;

		public HColor hColor = HColor.BrightGrey;

		public IcoType icoType;

		public SeparatorType separatorType;

		public HColor separatorHColor = HColor.Black;

		public CustomizedItem(GameObject gameObject, HColor hColor)
		{
			this.gameObject = gameObject;
			this.hColor = hColor;
		}

		public CustomizedItem(GameObject gameObject, IcoType icoType)
		{
			this.gameObject = gameObject;
			this.icoType = icoType;
		}

		public CustomizedItem(GameObject gameObject, SeparatorType separatorType, HColor separatorHColor)
		{
			this.gameObject = gameObject;
			this.separatorType = separatorType;
			this.separatorHColor = separatorHColor;
		}

		public Color GetColor()
		{
			return DeHierarchyComponent.GetColor(hColor);
		}

		public Color GetSeparatorColor()
		{
			return DeHierarchyComponent.GetColor(separatorHColor);
		}
	}

	public List<CustomizedItem> customizedItems = new List<CustomizedItem>();

	public List<int> MissingItemsIndexes()
	{
		List<int> list = null;
		for (int num = customizedItems.Count - 1; num > -1; num--)
		{
			if (customizedItems[num].gameObject == null)
			{
				if (list == null)
				{
					list = new List<int>();
				}
				list.Add(num);
			}
		}
		return list;
	}

	public void StoreItemColor(GameObject go, HColor hColor)
	{
		for (int i = 0; i < customizedItems.Count; i++)
		{
			if (!(customizedItems[i].gameObject != go))
			{
				customizedItems[i].hColor = hColor;
				return;
			}
		}
		customizedItems.Add(new CustomizedItem(go, hColor));
	}

	public void StoreItemIcon(GameObject go, IcoType icoType)
	{
		for (int i = 0; i < customizedItems.Count; i++)
		{
			if (!(customizedItems[i].gameObject != go))
			{
				customizedItems[i].icoType = icoType;
				return;
			}
		}
		customizedItems.Add(new CustomizedItem(go, icoType));
	}

	public void StoreItemSeparator(GameObject go, SeparatorType? separatorType, HColor? separatorHColor)
	{
		for (int i = 0; i < customizedItems.Count; i++)
		{
			if (!(customizedItems[i].gameObject != go))
			{
				if (separatorType.HasValue)
				{
					customizedItems[i].separatorType = separatorType.Value;
				}
				if (separatorHColor.HasValue)
				{
					customizedItems[i].separatorHColor = separatorHColor.Value;
				}
				return;
			}
		}
		customizedItems.Add(new CustomizedItem(go, separatorType.HasValue ? separatorType.Value : SeparatorType.None, separatorHColor.HasValue ? separatorHColor.Value : HColor.None));
	}

	public bool RemoveItemData(GameObject go)
	{
		int num = -1;
		for (int i = 0; i < customizedItems.Count; i++)
		{
			if (customizedItems[i].gameObject == go)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			customizedItems.RemoveAt(num);
			return true;
		}
		return false;
	}

	public bool ResetSeparator(GameObject go)
	{
		for (int i = 0; i < customizedItems.Count; i++)
		{
			if (!(customizedItems[i].gameObject != go))
			{
				customizedItems[i].separatorType = SeparatorType.None;
				customizedItems[i].separatorHColor = HColor.None;
				if (customizedItems[i].hColor == HColor.None)
				{
					customizedItems.RemoveAt(i);
				}
				return true;
			}
		}
		return false;
	}

	public CustomizedItem GetItem(GameObject go)
	{
		for (int i = 0; i < customizedItems.Count; i++)
		{
			if (customizedItems[i].gameObject == go)
			{
				return customizedItems[i];
			}
		}
		return null;
	}

	public static Color GetColor(HColor color)
	{
		return color switch
		{
			HColor.Red => new Color(0.82f, 0f, 0f), 
			HColor.Orange => new Color(1f, 0.44f, 0f), 
			HColor.Yellow => new Color(0.99f, 0.84f, 0.12f), 
			HColor.Green => new Color(0.05060553f, 117f / 136f, 0.2237113f, 1f), 
			HColor.Blue => new Color(0.21f, 0.62f, 1f), 
			HColor.Purple => new Color(0.64f, 0.27f, 1f), 
			HColor.Pink => new Color(1f, 0.21f, 0.82f), 
			HColor.BrightGrey => new Color(0.6470588f, 0.6470588f, 0.6470588f, 1f), 
			HColor.DarkGrey => new Color(0.3308824f, 0.3308824f, 0.3308824f, 1f), 
			HColor.Black => Color.black, 
			HColor.White => Color.white, 
			_ => Color.white, 
		};
	}
}
