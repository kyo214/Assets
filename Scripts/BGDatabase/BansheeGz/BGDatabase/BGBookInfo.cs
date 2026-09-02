using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGBookInfo : ICloneable, BGMergerEntity.ParseResultI
{
	public const string IdHeader = "_id";

	private readonly BGIdDictionary<BGEntitySheetInfo> metaId2EntitySheet = new BGIdDictionary<BGEntitySheetInfo>();

	private readonly List<BGEntitySheetInfo> entitySheets = new List<BGEntitySheetInfo>();

	private BGMetaSheetInfo metaSheet;

	private BGFieldSheetInfo fieldSheet;

	private BGAddonSheetInfo addonSheet;

	public BGMetaSheetInfo MetaSheet
	{
		get
		{
			return metaSheet;
		}
		set
		{
			metaSheet = value;
		}
	}

	public BGFieldSheetInfo FieldSheet
	{
		get
		{
			return fieldSheet;
		}
		set
		{
			fieldSheet = value;
		}
	}

	public BGAddonSheetInfo AddonSheet
	{
		get
		{
			return addonSheet;
		}
		set
		{
			addonSheet = value;
		}
	}

	public int EntitySheetCount => metaId2EntitySheet.Count;

	public bool HasFieldInEntitySheet(BGId metaId, BGId fieldId)
	{
		return GetEntitySheet(metaId)?.HasField(fieldId) ?? false;
	}

	public bool HasEntitySheet(BGId metaId)
	{
		return metaId2EntitySheet.ContainsKey(metaId);
	}

	public BGEntitySheetInfo GetEntitySheet(BGId metaId)
	{
		if (metaId2EntitySheet.TryGetValue(metaId, out var value))
		{
			return value;
		}
		return null;
	}

	public BGEntitySheetInfo GetEntitySheet(int index)
	{
		return entitySheets[index];
	}

	public void AddEntitySheet(BGId metaId, BGEntitySheetInfo entitySheet)
	{
		metaId2EntitySheet[metaId] = entitySheet;
		entitySheets.Add(entitySheet);
	}

	public void ForEachEntitySheet(Action<BGEntitySheetInfo> action)
	{
		foreach (BGEntitySheetInfo entitySheet in entitySheets)
		{
			action(entitySheet);
		}
	}

	public object Clone()
	{
		BGBookInfo bGBookInfo = new BGBookInfo();
		foreach (BGEntitySheetInfo entitySheet in entitySheets)
		{
			bGBookInfo.AddEntitySheet(entitySheet.MetaId, entitySheet.Clone() as BGEntitySheetInfo);
		}
		if (metaSheet != null)
		{
			bGBookInfo.metaSheet = (BGMetaSheetInfo)metaSheet.Clone();
		}
		if (fieldSheet != null)
		{
			bGBookInfo.fieldSheet = (BGFieldSheetInfo)fieldSheet.Clone();
		}
		if (addonSheet != null)
		{
			bGBookInfo.addonSheet = (BGAddonSheetInfo)addonSheet.Clone();
		}
		return bGBookInfo;
	}

	protected virtual void Clear()
	{
		metaId2EntitySheet.Clear();
		entitySheets.Clear();
	}
}
