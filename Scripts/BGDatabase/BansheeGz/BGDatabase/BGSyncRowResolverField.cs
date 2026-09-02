using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGSyncRowResolverField : BGSyncRowResolver
{
	private class MetaData
	{
		private readonly BGMetaEntity meta;

		private readonly BGId fieldId;

		private readonly BGLogger logger;

		private readonly bool printWarnings;

		private Dictionary<object, BGId> value2Entity;

		private bool value2EntityInited;

		private Dictionary<BGId, object> entity2Value;

		private bool entity2ValueInited;

		public BGMetaEntity Meta => meta;

		public BGId MetaId => meta?.Id ?? BGId.Empty;

		public MetaData(BGMetaEntity meta, BGId fieldId, BGLogger logger, bool printWarnings)
		{
			this.meta = meta;
			this.fieldId = fieldId;
			this.logger = logger;
			this.printWarnings = printWarnings;
		}

		public BGRowRef StringToRowRef(string value)
		{
			if (!value2EntityInited)
			{
				InitValueToRow();
			}
			if (value2Entity != null && value2Entity.TryGetValue(value.Trim(), out var value2))
			{
				return new BGRowRef(meta.Id, value2);
			}
			return null;
		}

		public string RowIdToString(BGId rowId)
		{
			if (!entity2ValueInited)
			{
				InitRowToValue();
			}
			if (entity2Value != null && entity2Value.TryGetValue(rowId, out var value))
			{
				return value?.ToString();
			}
			return null;
		}

		private void InitValueToRow()
		{
			value2EntityInited = true;
			BGField bGField = meta?.GetField(fieldId, errorIfNotFound: false);
			if (bGField == null)
			{
				return;
			}
			value2Entity = new Dictionary<object, BGId>();
			for (int i = 0; i < meta.CountEntities; i++)
			{
				BGEntity entity = bGField.Meta.GetEntity(i);
				object value = bGField.GetValue(i);
				if (value != null)
				{
					try
					{
						value2Entity.Add(value, entity.Id);
					}
					catch (ArgumentException)
					{
						BGSyncUtil.AppendWarning(logger, printWarnings, "RowResolver: duplicate ID value is detected! Row ID=$, field=$, duplicate ID value=$", entity.Id, bGField.FullName, value);
					}
				}
			}
		}

		private void InitRowToValue()
		{
			entity2ValueInited = true;
			BGField bGField = meta?.GetField(fieldId, errorIfNotFound: false);
			if (bGField != null)
			{
				entity2Value = new Dictionary<BGId, object>();
				for (int i = 0; i < meta.CountEntities; i++)
				{
					BGEntity entity = bGField.Meta.GetEntity(i);
					object value = bGField.GetValue(i);
					entity2Value[entity.Id] = value;
				}
			}
		}
	}

	private readonly MetaData metaData1;

	private readonly MetaData metaData2;

	private readonly BGId fieldId;

	public BGId MetaId => metaData1.Meta?.Id ?? metaData2.Meta.Id;

	public string MetaName => metaData1.Meta?.Name ?? metaData2.Meta.Name;

	public BGSyncRowResolverField(BGMetaEntity meta1, BGMetaEntity meta2, BGId fieldId, BGLogger logger, bool printWarnings)
	{
		metaData1 = new MetaData(meta1, fieldId, logger, printWarnings);
		metaData2 = new MetaData(meta2, fieldId, logger, printWarnings);
		this.fieldId = fieldId;
	}

	public BGRowRef FromString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		value = value.Trim();
		BGRowRef bGRowRef = metaData1.StringToRowRef(value);
		return bGRowRef ?? metaData2.StringToRowRef(value);
	}

	public string ToString(BGId rowId)
	{
		if (rowId.IsEmpty)
		{
			return null;
		}
		string text = metaData1.RowIdToString(rowId);
		return text ?? metaData2.RowIdToString(rowId);
	}

	public override string ToString()
	{
		return "Resolver by field, table=" + MetaName + ", ID field=" + fieldId.ToString();
	}
}
