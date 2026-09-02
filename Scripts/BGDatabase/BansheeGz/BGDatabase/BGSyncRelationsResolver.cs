using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGSyncRelationsResolver
{
	private readonly BGSyncRelationsConfig config;

	private readonly BGSyncIdConfig idConfig;

	private readonly BGLogger logger;

	private readonly bool printWarnings;

	private BGRepo repo;

	private readonly BGRepo mainRepo;

	private readonly Dictionary<BGId, BGSyncRowResolver> metaId2Resolver = new Dictionary<BGId, BGSyncRowResolver>();

	public BGRepo Repo
	{
		set
		{
			repo = value;
		}
	}

	public BGSyncRelationsResolver(BGSyncRelationsConfig config, BGSyncIdConfig idConfig, BGRepo mainRepo)
	{
		this.config = config;
		this.idConfig = idConfig;
		this.mainRepo = mainRepo;
	}

	public BGSyncRelationsResolver(BGSyncRelationsConfig config, BGSyncIdConfig idConfig, BGRepo mainRepo, BGLogger logger, bool printWarnings)
	{
		this.config = config;
		this.idConfig = idConfig;
		this.mainRepo = mainRepo;
		this.logger = logger;
		this.printWarnings = printWarnings;
	}

	public BGSyncRelationResolver GetResolver(BGField relation)
	{
		if (!(relation is BGAbstractRelationI))
		{
			throw new BGException("field $ is not relation", relation.FullName);
		}
		if (config == null)
		{
			return GetDefaultResolver(relation);
		}
		BGSyncRelationResolver bGSyncRelationResolver = null;
		if (!(relation is BGRelationI { ToId: var toId }))
		{
			if (relation is BGManyTablesRelationI { ToIds: var toIds })
			{
				List<BGSyncRowResolver> list = new List<BGSyncRowResolver>(toIds.Count);
				foreach (BGId item in toIds)
				{
					list.Add(GetRowResolver(item));
				}
				if (!(relation is BGFieldManyRelationsSingle relation2))
				{
					if (relation is BGFieldManyRelationsMultiple relation3)
					{
						return new BGSyncRelationResolverFieldMTMV(list, relation3, mainRepo);
					}
					throw new Exception("Unknown relation type=" + relation.GetType().FullName);
				}
				return new BGSyncRelationResolverFieldMTSV(list, relation2, mainRepo);
			}
			throw new Exception("Unknown relation type=" + relation.GetType().FullName);
		}
		BGSyncRowResolver rowResolver = GetRowResolver(toId);
		if (rowResolver is BGSyncRowResolverId)
		{
			return GetDefaultResolver(relation);
		}
		if (relation is BGFieldRelationMultiple relation4)
		{
			return new BGSyncRelationResolverFieldSTMV(rowResolver, relation4, mainRepo);
		}
		return new BGSyncRelationResolverFieldSTSV(rowResolver, relation, mainRepo);
	}

	private BGSyncRelationResolver GetDefaultResolver(BGField relation)
	{
		if (!(relation is BGFieldRelationMultiple relation2))
		{
			if (!(relation is BGRelationI))
			{
				if (!(relation is BGFieldManyRelationsMultiple relation3))
				{
					if (relation is BGFieldManyRelationsSingle relation4)
					{
						return new BGSyncRelationResolverByIdMTSV(relation4, mainRepo);
					}
					throw new Exception("Unknown relation type " + relation.GetType().FullName);
				}
				return new BGSyncRelationResolverByIdMTMV(relation3, mainRepo);
			}
			return new BGSyncRelationResolverByIdSTSV(relation, mainRepo);
		}
		return new BGSyncRelationResolverByIdSTMV(relation2, mainRepo);
	}

	private BGSyncRowResolver GetRowResolver(BGId metaId)
	{
		if (metaId2Resolver.TryGetValue(metaId, out var value))
		{
			return value;
		}
		BGSyncRelationsConfig.BGSyncRelationConfigMeta metaConfig = config.GetMetaConfig(metaId);
		if (metaConfig == null)
		{
			switch (config.DefaultConfig)
			{
			case BGSyncRelationsConfig.DefaultRelationConfigEnum.Name:
				value = new BGSyncRowResolverField(GetMeta1(metaId), GetMeta2(metaId), GetNameFieldId(metaId), logger, printWarnings);
				break;
			case BGSyncRelationsConfig.DefaultRelationConfigEnum.IdConfig:
			{
				BGSyncIdConfig.BGSyncIdConfigMeta bGSyncIdConfigMeta = idConfig?.GetMetaConfig(metaId);
				if (bGSyncIdConfigMeta != null && bGSyncIdConfigMeta.configType == BGSyncIdConfig.IdConfigEnum.Field && HasField(metaId, bGSyncIdConfigMeta.FieldId))
				{
					value = new BGSyncRowResolverField(GetMeta1(metaId), GetMeta2(metaId), bGSyncIdConfigMeta.FieldId, logger, printWarnings);
				}
				break;
			}
			}
		}
		else if (metaConfig.configType == BGSyncRelationsConfig.RelationConfigEnum.Field && HasField(metaId, metaConfig.FieldId))
		{
			value = new BGSyncRowResolverField(GetMeta1(metaId), GetMeta2(metaId), metaConfig.FieldId, logger, printWarnings);
		}
		if (value == null)
		{
			value = new BGSyncRowResolverId(metaId, mainRepo[metaId].Name);
		}
		metaId2Resolver.Add(metaId, value);
		return value;
	}

	private bool HasField(BGId metaId, BGId fieldId)
	{
		BGMetaEntity meta = GetMeta1(metaId);
		if (meta != null && meta.HasField(fieldId))
		{
			return true;
		}
		BGMetaEntity meta2 = GetMeta2(metaId);
		if (meta2 != null && meta2.HasField(fieldId))
		{
			return true;
		}
		return false;
	}

	private BGMetaEntity GetMeta1(BGId metaId)
	{
		return repo?.GetMeta(metaId);
	}

	private BGMetaEntity GetMeta2(BGId metaId)
	{
		return mainRepo?.GetMeta(metaId);
	}

	private BGId GetNameFieldId(BGId metaId)
	{
		BGFieldEntityName bGFieldEntityName = GetMeta1(metaId)?.NameField;
		if (bGFieldEntityName != null)
		{
			return bGFieldEntityName.Id;
		}
		bGFieldEntityName = GetMeta2(metaId)?.NameField;
		if (bGFieldEntityName != null)
		{
			return bGFieldEntityName.Id;
		}
		throw new Exception("Unexpected error: both metas are null");
	}
}
