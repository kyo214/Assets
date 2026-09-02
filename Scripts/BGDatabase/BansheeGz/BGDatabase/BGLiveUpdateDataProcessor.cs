using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGLiveUpdateDataProcessor
{
	public class BGLiveUpdateData
	{
		private readonly BGMetaEntity meta;

		private readonly BGField[] fields;

		private readonly List<string[]> data = new List<string[]>();

		private readonly List<BGId> entityIds = new List<BGId>();

		public int RowsCount => data.Count;

		public BGMetaEntity Meta => meta;

		public BGField[] Fields => fields;

		public BGLiveUpdateData(BGMetaEntity meta, BGField[] fields)
		{
			this.meta = meta;
			this.fields = fields;
		}

		public void Add(BGId entityId, string[] values, BGLiveUpdateLog log, int rowIndex)
		{
			if (values == null || fields.Length != values.Length)
			{
				return;
			}
			if (entityId.IsEmpty)
			{
				bool flag = false;
				foreach (string text in values)
				{
					if (!string.IsNullOrEmpty(text) && !text.Trim().Equals(string.Empty))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					log?.AddDetail("WARNING! No values found for row # $! skipping the row", rowIndex);
					return;
				}
				entityId = BGId.NewId;
			}
			data.Add(values);
			entityIds.Add(entityId);
		}

		public void ForEachRow(Action<BGId, string[]> action)
		{
			for (int i = 0; i < data.Count; i++)
			{
				string[] arg = data[i];
				BGId arg2 = entityIds[i];
				action(arg2, arg);
			}
		}
	}

	private readonly BGAddonLiveUpdate addon;

	private readonly BGRepo defaultRepo;

	public BGLiveUpdateDataProcessor(BGAddonLiveUpdate addon, BGRepo defaultRepo)
	{
		this.addon = addon;
		this.defaultRepo = defaultRepo;
	}

	internal void Process(BGLiveUpdateData data)
	{
		if (data == null)
		{
			return;
		}
		addon.Log.AddDetail("$ entity rows found for '$' table", data.RowsCount, data.Meta.Name);
		BGLiveUpdateValueResolver valueResolver = addon.ValueResolver;
		data.ForEachRow((BGId entityId, string[] values) =>
		{
			BGMetaEntity meta = data.Meta;
			BGEntity bGEntity;
			if (entityId.IsEmpty)
			{
				bGEntity = meta.NewEntity();
			}
			else
			{
				if (meta.HasEntity(entityId))
				{
					addon.Log.AddDetail("Duplicate entity with id $, skipping", entityId);
					return;
				}
				bGEntity = meta.NewEntity(entityId);
			}
			for (int i = 0; i < values.Length; i++)
			{
				string text = values[i];
				BGField bGField = data.Fields[i];
				try
				{
					if (valueResolver != null)
					{
						try
						{
							text = valueResolver.Resolve(bGField, text);
						}
						catch (Exception exception)
						{
							Debug.Log("Value resolver thrown exception while resolving value. Field=" + bGField.FullName + ", value=" + text);
							Debug.LogException(exception);
						}
					}
					BGUtil.FromString(bGField, bGEntity.Index, text);
					addon.Log.AddCellSuccess(meta.Id, "Index $. Field $. Value $", i, bGField.Name, text);
				}
				catch (Exception)
				{
					try
					{
						if (!TryToFix(bGField, bGEntity.Index, text))
						{
							AssignDefault(bGField, meta.Id, bGEntity.Id, i, text);
						}
						else
						{
							addon.Log.AddCellSuccess(meta.Id, "Index $. Field $. Value (fixed)=$", i, bGField.Name, text);
						}
					}
					catch
					{
						AssignDefault(bGField, meta.Id, bGEntity.Id, i, text);
					}
				}
			}
		});
	}

	private void AssignDefault(BGField field, BGId metaId, BGId entityId, int i, string fieldValue)
	{
		bool flag = AssignDefault(field, metaId, entityId);
		addon.Log.AddCellFailed(metaId, entityId, field.Id, "Index $. Field $. Invalid value $. Fallback value was" + (flag ? "" : " NOT") + " assigned", i, field.Name, fieldValue);
	}

	private bool AssignDefault(BGField field, BGId metaId, BGId entityId)
	{
		try
		{
			BGMetaEntity bGMetaEntity = defaultRepo[metaId];
			BGField bGField = bGMetaEntity?.GetField(field.Id, errorIfNotFound: false);
			if (bGField == null)
			{
				return false;
			}
			BGEntity entity = bGMetaEntity.GetEntity(entityId);
			if (entity == null)
			{
				return false;
			}
			field.CopyValue(bGField, entityId, entity.Index, entityId);
		}
		catch
		{
			return false;
		}
		return true;
	}

	private bool TryToFix(BGField field, int entityIndex, string fieldValue)
	{
		if (!string.IsNullOrEmpty(fieldValue) && (field is BGFieldLong || field is BGFieldInt || field is BGFieldFloat || field is BGFieldDouble || field is BGFieldDecimal || field is BGFieldLongNullable || field is BGFieldIntNullable || field is BGFieldFloatNullable || field is BGFieldDoubleNullable || field is BGFieldListFloat || field is BGFieldListDouble) && fieldValue.IndexOf(',') != -1)
		{
			fieldValue = fieldValue.Replace(",", "");
			BGUtil.FromString(field, entityIndex, fieldValue);
			return true;
		}
		return false;
	}
}
