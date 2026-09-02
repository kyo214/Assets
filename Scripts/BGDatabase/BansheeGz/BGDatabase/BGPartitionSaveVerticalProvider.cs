using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGPartitionSaveVerticalProvider
{
	private readonly BGRepo referenceRepo;

	private readonly BGAddonPartition partitionAddon;

	private readonly Dictionary<BGId, Tuple<BGId, BGRepo>> metaId2Repo = new Dictionary<BGId, Tuple<BGId, BGRepo>>();

	private readonly List<Tuple<BGId, BGRepo>> repos = new List<Tuple<BGId, BGRepo>>();

	public BGPartitionSaveVerticalProvider(BGRepo referenceRepo, BGAddonPartition partitionAddon)
	{
		BGPartitionSaveVerticalProvider bGPartitionSaveVerticalProvider = this;
		this.referenceRepo = referenceRepo;
		this.partitionAddon = partitionAddon;
		BGPartitionVerticalStructure structure = new BGPartitionVerticalStructure(referenceRepo);
		BGAddonSettings.FormatEnum format = BGAddonSettings.GetFormat(referenceRepo);
		structure.ForEachPartition((BGEntity partition) =>
		{
			List<BGMetaEntity> metas = structure.GetMetas(partition);
			if (metas != null && metas.Count != 0)
			{
				BGRepo bGRepo = new BGRepo();
				if (format == BGAddonSettings.FormatEnum.Json)
				{
					bGRepo.Addons.Add(new BGAddonSettings
					{
						Format = BGAddonSettings.FormatEnum.Json
					});
				}
				foreach (BGMetaEntity item in metas)
				{
					bGPartitionSaveVerticalProvider.ProcessMeta(item, bGRepo, partition.Id);
				}
				if (bGRepo.CountMeta > 0)
				{
					bGPartitionSaveVerticalProvider.repos.Add(Tuple.Create(partition.Id, bGRepo));
				}
			}
		});
	}

	private void ProcessMeta(BGMetaEntity referenceMeta, BGRepo repo, BGId partitionId)
	{
		BGMetaEntity bGMetaEntity = referenceMeta.CloneTo(repo, null, null, copyValues: false);
		metaId2Repo[bGMetaEntity.Id] = Tuple.Create(partitionId, repo);
		referenceMeta.ForEachField((BGField field) =>
		{
			BGFieldNested bGFieldNested = (BGFieldNested)field;
			BGMetaNested nestedMeta = bGFieldNested.NestedMeta;
			ProcessMeta(nestedMeta, repo, partitionId);
		}, (BGField field) => field is BGFieldNested);
	}

	public void ForEachRepo(Action<BGId, BGRepo> action)
	{
		foreach (var (arg, arg2) in repos)
		{
			action(arg, arg2);
		}
	}

	public BGMetaEntity GetMeta(BGId metaId)
	{
		if (!metaId2Repo.TryGetValue(metaId, out var value))
		{
			return null;
		}
		return value.Item2.GetMeta(metaId);
	}
}
