using System;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace BansheeGz.BGDatabase;

public abstract class BGExcelIdResolverFieldART<T> : BGExcelIdResolverART, BGExcelIdFieldResolverIRT
{
	private readonly BGField<T> field;

	private readonly Dictionary<T, BGId> value2Id = new Dictionary<T, BGId>();

	public bool PrintWarnings;

	public BGField Field => this.field;

	public BGExcelIdResolverFieldART(BGLogger logger, BGField<T> field)
		: base(logger)
	{
		this.field = field;
		int countEntities = field.Meta.CountEntities;
		for (int i = 0; i < countEntities; i++)
		{
			T val = field[i];
			if (value2Id.TryGetValue(val, out var value))
			{
				BGSyncUtil.AppendWarning(logger, PrintWarnings, "Field row resolver: row # $ is ignored, cause duplicate ID value is detected! Row ID=$, field=$, duplicate ID value=$", i, value, field.FullName, val);
			}
			else
			{
				value2Id.Add(val, field.Meta.GetEntity(i).Id);
			}
		}
	}

	public override BGId ResolveId(BGExcelSheetReaderEntityRT reader, BGEntitySheetInfo info, IRow row)
	{
		BGId entityId = BGId.Empty;
		int fieldColumn = info.GetFieldColumn(field.Id);
		if (fieldColumn >= 0)
		{
			reader.ReadNotNull(row, fieldColumn, (string s) =>
			{
				T val = default;
				try
				{
					val = Convert(s);
				}
				catch (Exception ex)
				{
					BGSyncUtil.AppendWarning(logger, PrintWarnings, "Row # " + row.RowNum + " is skipped, cause ID value can not be extracted from value=" + s + ", error=" + ex.Message);
					return;
				}
				if (!value2Id.TryGetValue(val, out entityId))
				{
					entityId = BGId.NewId;
				}
			});
		}
		return entityId;
	}

	protected abstract T Convert(string value);
}
