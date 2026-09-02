using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGPartitionSnapshot
{
	private readonly List<Tuple<BGMetaEntity, int>> data = new List<Tuple<BGMetaEntity, int>>();

	public BGPartitionSnapshot(BGRepo repo)
	{
		BGMetaPartitionModelProvider bGMetaPartitionModelProvider = new BGMetaPartitionModelProvider();
		bGMetaPartitionModelProvider.ForEachModelWithField(repo, (BGMetaPartitionModelA.FieldOwner owner) =>
		{
			data.Add(Tuple.Create(owner.Meta, owner.Meta.CountEntities));
		});
	}

	public void MarkKeysAndIndexesDirty()
	{
		foreach (Tuple<BGMetaEntity, int> datum in data)
		{
			BGMetaEntity item = datum.Item1;
			int item2 = datum.Item2;
			if (item.CountEntities != item2)
			{
				item.ForEachKey((BGKey key) =>
				{
					key.MarkDirty();
				});
				item.ForEachIndex((BGIndex index) =>
				{
					index.MarkDirty();
				});
			}
		}
	}
}
