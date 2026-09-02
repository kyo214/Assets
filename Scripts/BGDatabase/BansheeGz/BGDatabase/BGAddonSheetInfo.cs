using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGAddonSheetInfo(int sheetNumber) : BGSheetInfoA(sheetNumber)
{
	private readonly Dictionary<string, int> addonType2Row = new Dictionary<string, int>();

	public int IndexType = -1;

	public int IndexConfig = -1;

	public int AddonCount => addonType2Row.Count;

	public void AddAddon(string type, int rowIndex)
	{
		if (!HasAddon(type))
		{
			addonType2Row[type] = rowIndex;
		}
	}

	public bool HasAddon(string type)
	{
		return addonType2Row.ContainsKey(type);
	}

	public int GetAddonRow(string type)
	{
		if (!addonType2Row.ContainsKey(type))
		{
			return -1;
		}
		return BGUtil.Get(addonType2Row, type);
	}

	public override void Clear()
	{
		addonType2Row.Clear();
		IndexType = -1;
		IndexConfig = -1;
	}

	public override object Clone()
	{
		BGAddonSheetInfo bGAddonSheetInfo = new BGAddonSheetInfo(SheetNumber)
		{
			IndexType = IndexType,
			IndexConfig = IndexConfig
		};
		foreach (KeyValuePair<string, int> item in addonType2Row)
		{
			bGAddonSheetInfo.addonType2Row.Add(item.Key, item.Value);
		}
		return bGAddonSheetInfo;
	}

	public void ForEachAddon(Action<string, int> action)
	{
		foreach (KeyValuePair<string, int> item in addonType2Row)
		{
			action(item.Key, item.Value);
		}
	}

	public void RemoveAddon(string type)
	{
		addonType2Row.Remove(type);
	}
}
