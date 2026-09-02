using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGPartitionVerticalStructure
{
	private readonly BGMetaEntity partitionMeta;

	private readonly BGFieldNested fieldNestedTables;

	private readonly BGFieldPartitionMetaReference fieldRef;

	public BGMetaEntity PartitionMeta => partitionMeta;

	public BGFieldNested FieldNestedTables => fieldNestedTables;

	public BGFieldPartitionMetaReference FieldRef => fieldRef;

	public BGPartitionVerticalStructure(BGRepo repo)
	{
		partitionMeta = repo.GetMeta("DbPartitionVertical");
		if (partitionMeta == null)
		{
			throw new Exception("Meta [DbPartitionVertical] not found");
		}
		fieldNestedTables = (BGFieldNested)partitionMeta.GetField("DbPartitionVerticalMetas", errorIfNotFound: false);
		if (fieldNestedTables == null)
		{
			throw new Exception("Field [DbPartitionVerticalMetas] not found");
		}
		fieldRef = (BGFieldPartitionMetaReference)fieldNestedTables.NestedMeta.GetField("metaRef", errorIfNotFound: false);
		if (fieldRef == null)
		{
			throw new Exception("Field [metaRef] not found");
		}
	}

	public static BGPartitionVerticalStructure Create(BGRepo repo)
	{
		BGMetaRow meta = new BGMetaRow(repo, "DbPartitionVertical")
		{
			Addon = "Partition"
		};
		BGFieldNested bGFieldNested = new BGFieldNested(meta, "DbPartitionVerticalMetas");
		bGFieldNested.Addon = "Partition";
		bGFieldNested.NestedMeta.Addon = "Partition";
		bGFieldNested.NestedMeta.EmptyName = true;
		BGFieldNested bGFieldNested2 = bGFieldNested;
		new BGFieldPartitionMetaReference(bGFieldNested2.NestedMeta, "metaRef").Addon = "Partition";
		return new BGPartitionVerticalStructure(repo);
	}

	public void ForEachPartition(Action<BGEntity> action)
	{
		int countEntities = partitionMeta.CountEntities;
		for (int i = 0; i < countEntities; i++)
		{
			action(partitionMeta.GetEntity(i));
		}
	}

	public List<BGMetaEntity> GetMetas(BGEntity partition)
	{
		List<BGMetaEntity> metas = new List<BGMetaEntity>();
		ForEachMeta(partition, (BGMetaEntity m) =>
		{
			metas.Add(m);
		});
		return metas;
	}

	public void ForEachMeta(BGEntity partition, Action<BGMetaEntity> action)
	{
		List<BGEntity> list = fieldNestedTables[partition.Index];
		if (list == null || list.Count == 0)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			BGMetaEntity bGMetaEntity = fieldRef[list[i].Index];
			if (BGAddonPartition.IsSupportedForVerticalPartitioning(bGMetaEntity))
			{
				action(bGMetaEntity);
			}
		}
	}

	public BGEntity AddPartition(string name)
	{
		BGEntity bGEntity = partitionMeta.NewEntity();
		bGEntity.Name = name;
		return bGEntity;
	}

	public BGEntity AddTable(BGEntity partition, BGMetaEntity meta)
	{
		BGEntity bGEntity = fieldNestedTables.NestedMeta.NewEntity(partition);
		fieldRef[bGEntity.Index] = meta;
		return bGEntity;
	}

	public static void Delete(BGRepo repo)
	{
		repo.GetMeta("DbPartitionVertical")?.Delete();
	}
}
