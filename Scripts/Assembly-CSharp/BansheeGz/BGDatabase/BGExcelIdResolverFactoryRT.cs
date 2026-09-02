using System;

namespace BansheeGz.BGDatabase;

public class BGExcelIdResolverFactoryRT
{
	private readonly BGRepo mainRepo;

	private readonly BGSyncIdConfig idConfig;

	public BGSyncIdConfig IdConfig => idConfig;

	public BGExcelIdResolverFactoryRT(BGRepo mainRepo, BGSyncIdConfig idConfig)
	{
		this.mainRepo = mainRepo;
		this.idConfig = idConfig;
	}

	public BGExcelIdResolverART Create(BGId metaId, BGLogger logger, bool printWarnings)
	{
		if (idConfig == null)
		{
			return new BGExcelIdResolverIdRT(logger);
		}
		BGSyncIdConfig.BGSyncIdConfigMeta metaConfig = idConfig.GetMetaConfig(metaId);
		if (metaConfig == null)
		{
			return new BGExcelIdResolverIdRT(logger);
		}
		switch (metaConfig.configType)
		{
		case BGSyncIdConfig.IdConfigEnum.IdColumn:
			return new BGExcelIdResolverIdRT(logger);
		case BGSyncIdConfig.IdConfigEnum.NoId:
			return new BGExcelIdResolverNoIdRT(logger);
		case BGSyncIdConfig.IdConfigEnum.Index:
			return new BGExcelIdResolverIndexRT(logger, mainRepo?.GetMeta(metaId));
		case BGSyncIdConfig.IdConfigEnum.Field:
		{
			if (mainRepo == null)
			{
				return new BGExcelIdResolverIdRT(logger);
			}
			BGMetaEntity meta = mainRepo.GetMeta(metaId);
			if (meta == null)
			{
				return new BGExcelIdResolverIdRT(logger);
			}
			BGField field = meta.GetField(metaConfig.FieldId, errorIfNotFound: false);
			if (field == null || !BGSyncIdConfig.IsSupported(field))
			{
				return new BGExcelIdResolverIdRT(logger);
			}
			if (!(field is BGFieldString field2))
			{
				if (field is BGFieldInt field3)
				{
					return new BGExcelIdResolverFieldIntRT(logger, field3)
					{
						PrintWarnings = printWarnings
					};
				}
				throw new BGException("Can not create id resolver, field type is $, field=$", field.GetType().FullName, field.FullName);
			}
			return new BGExcelIdResolverFieldStringRT(logger, field2)
			{
				PrintWarnings = printWarnings
			};
		}
		default:
			throw new ArgumentOutOfRangeException("metaConfig.configType", "Unsupported config type=" + metaConfig.configType);
		}
	}
}
