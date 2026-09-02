using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGPartitionsModel
{
	private readonly BGMetaEntity partitionMeta;

	private readonly BGPartitionModelDefault[] partitions;

	private readonly BGPartitionModelMain main;

	private Dictionary<BGId, BGPartitionModelDefault> id2partition;

	public BGPartitionModelA Main => main;

	public BGRepo Repo => partitionMeta.Repo;

	public int PartitionsCount => partitions.Length;

	public BGPartitionModelDefault[] Partitions => partitions;

	public void ForEach(Action<BGPartitionModelDefault> action)
	{
		BGPartitionModelDefault[] array = partitions;
		foreach (BGPartitionModelDefault obj in array)
		{
			action(obj);
		}
	}

	public BGPartitionsModel(BGRepo repo)
	{
		partitionMeta = repo.GetMeta("DbPartition");
		main = new BGPartitionModelMain();
		partitions = new BGPartitionModelDefault[partitionMeta.CountEntities];
		for (int i = 0; i < partitionMeta.CountEntities; i++)
		{
			partitions[i] = new BGPartitionModelDefault(partitionMeta.GetEntity(i));
		}
	}

	public BGPartitionModelDefault Get(int index)
	{
		if (index < 0)
		{
			return null;
		}
		if (index >= partitions.Length)
		{
			return null;
		}
		return partitions[index];
	}

	public BGPartitionModelDefault Get(BGId id)
	{
		if (id2partition == null)
		{
			InitIdDictionary();
		}
		return BGUtil.Get(id2partition, id);
	}

	private void InitIdDictionary()
	{
		id2partition = new Dictionary<BGId, BGPartitionModelDefault>(partitionMeta.CountEntities);
		partitionMeta.ForEachEntity((BGEntity entity) =>
		{
			id2partition[entity.MetaId] = Get(entity.Index);
		});
	}
}
