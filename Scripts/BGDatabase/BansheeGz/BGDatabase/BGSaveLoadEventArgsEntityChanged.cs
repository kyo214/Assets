using System;
using System.Collections.Generic;
using System.Text;

namespace BansheeGz.BGDatabase;

public class BGSaveLoadEventArgsEntityChanged : BGEventArgsA
{
	public class FieldChangedData
	{
		internal BGField field;

		internal object oldValue;

		internal object newValue;

		public FieldChangedData(BGField field, object oldValue, object newValue)
		{
			this.field = field;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
	}

	private static readonly BGObjectPool<BGSaveLoadEventArgsEntityChanged> pool = new BGObjectPool<BGSaveLoadEventArgsEntityChanged>(() => new BGSaveLoadEventArgsEntityChanged());

	private BGMetaEntity meta;

	private BGEntity entity;

	private readonly List<FieldChangedData> fieldsData = new List<FieldChangedData>();

	protected override BGObjectPool Pool => pool;

	public BGMetaEntity Meta => meta;

	public BGEntity Entity => entity;

	public List<FieldChangedData> FieldsData => fieldsData;

	private BGSaveLoadEventArgsEntityChanged()
	{
	}

	public override void Clear()
	{
		meta = null;
		entity = null;
		fieldsData.Clear();
	}

	public FieldChangedData GetFieldData(string fieldName)
	{
		for (int i = 0; i < FieldsData.Count; i++)
		{
			FieldChangedData fieldChangedData = FieldsData[i];
			if (fieldChangedData.field != null && string.Equals(fieldChangedData.field.Name, fieldName, StringComparison.Ordinal))
			{
				return fieldChangedData;
			}
		}
		return null;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < FieldsData.Count; i++)
		{
			FieldChangedData fieldChangedData = FieldsData[i];
			if (fieldChangedData?.field != null)
			{
				if (i != 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("[" + fieldChangedData.field.Name + ": " + fieldChangedData.oldValue?.ToString() + "->" + fieldChangedData.newValue?.ToString() + "]");
			}
		}
		return "BGSaveLoadEventArgsEntityChanged: " + ((entity == null) ? "[no entity]" : entity.FullName) + ", fields: " + stringBuilder;
	}

	public static BGSaveLoadEventArgsEntityChanged Get(BGMetaEntity meta, BGEntity entity, List<FieldChangedData> fieldData)
	{
		BGSaveLoadEventArgsEntityChanged bGSaveLoadEventArgsEntityChanged = pool.Get();
		bGSaveLoadEventArgsEntityChanged.Clear();
		bGSaveLoadEventArgsEntityChanged.meta = meta;
		bGSaveLoadEventArgsEntityChanged.entity = entity;
		if (fieldData != null)
		{
			foreach (FieldChangedData fieldDatum in fieldData)
			{
				bGSaveLoadEventArgsEntityChanged.fieldsData.Add(fieldDatum);
			}
		}
		return bGSaveLoadEventArgsEntityChanged;
	}
}
