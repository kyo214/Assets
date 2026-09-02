using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGSyncDuplicateEntitiesMonitor
{
	private readonly BGField field;

	private readonly HashSet<object> processedValues = new HashSet<object>();

	private readonly bool isString;

	private BGSyncDuplicateEntitiesMonitor(BGField field)
	{
		this.field = field;
		isString = field is BGFieldString;
	}

	public static BGSyncDuplicateEntitiesMonitor Get(BGSyncIdConfig idConfig, BGMetaEntity meta)
	{
		BGSyncIdConfig.BGSyncIdConfigMeta bGSyncIdConfigMeta = idConfig?.GetMetaConfig(meta.Id);
		if (bGSyncIdConfigMeta == null)
		{
			return null;
		}
		if (bGSyncIdConfigMeta.configType != BGSyncIdConfig.IdConfigEnum.Field)
		{
			return null;
		}
		BGField bGField = meta.GetField(bGSyncIdConfigMeta.FieldId, errorIfNotFound: false);
		if (bGField == null)
		{
			return null;
		}
		return new BGSyncDuplicateEntitiesMonitor(bGField);
	}

	public void Add(BGEntity entity)
	{
		object value = field.GetValue(entity.Index);
		if (value != null)
		{
			processedValues.Add(value);
		}
	}

	public bool Process(BGEntity entity, BGLogger logger, bool printWarnings)
	{
		if (entity == null)
		{
			BGSyncUtil.AppendWarning(logger, printWarnings, "Duplicate monitor: Entity is $!", "null");
			return false;
		}
		object value = field.GetValue(entity.Index);
		if (value == null || (isString && (string)value == string.Empty))
		{
			BGSyncUtil.AppendWarning(logger, printWarnings, "Row # $, of meta=$ is skipped while exporting, cause ID value is not set!", entity.Index, field.Meta.Name);
			return false;
		}
		if (processedValues.Add(value))
		{
			return true;
		}
		BGSyncUtil.AppendWarning(logger, printWarnings, "Row # $, of meta=$ is skipped while exporting, cause ID value=$ is a duplicate!", entity.Index, field.Meta.Name, value);
		return false;
	}
}
