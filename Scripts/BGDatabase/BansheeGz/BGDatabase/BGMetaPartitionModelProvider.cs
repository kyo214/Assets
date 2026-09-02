using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGMetaPartitionModelProvider
{
	public interface BGMetaPartitionModelProviderDelegate
	{
		BGMetaPartitionModelA Get(BGMetaEntity meta);
	}

	private static List<BGMetaPartitionModelProviderDelegate> delegates;

	private static List<BGMetaPartitionModelProviderDelegate> Delegates
	{
		get
		{
			if (delegates != null)
			{
				return delegates;
			}
			List<Type> allImplementations = BGUtil.GetAllImplementations(typeof(BGMetaPartitionModelProviderDelegate));
			List<BGMetaPartitionModelProviderDelegate> list = new List<BGMetaPartitionModelProviderDelegate>();
			if (allImplementations != null)
			{
				foreach (Type item2 in allImplementations)
				{
					BGMetaPartitionModelProviderDelegate item = (BGMetaPartitionModelProviderDelegate)Activator.CreateInstance(item2);
					list.Add(item);
				}
			}
			delegates = list;
			return delegates;
		}
	}

	public virtual BGMetaPartitionModelA Get(BGMetaEntity meta)
	{
		List<BGMetaPartitionModelProviderDelegate> list = Delegates;
		foreach (BGMetaPartitionModelProviderDelegate item in list)
		{
			BGMetaPartitionModelA bGMetaPartitionModelA = item.Get(meta);
			if (bGMetaPartitionModelA != null)
			{
				return bGMetaPartitionModelA;
			}
		}
		if (!BGAddonPartition.SupportPartitioningField(meta))
		{
			return null;
		}
		if (meta is BGMetaNested)
		{
			List<BGFieldRelationSingle> list2 = new List<BGFieldRelationSingle>();
			for (BGMetaEntity bGMetaEntity = meta; bGMetaEntity is BGMetaNested { OwnerRelation: var ownerRelation }; bGMetaEntity = ownerRelation.To)
			{
				list2.Add(ownerRelation);
			}
			for (int num = list2.Count - 1; num >= 0; num--)
			{
				BGFieldRelationSingle bGFieldRelationSingle = list2[num];
				BGMetaPartitionModelDefault bGMetaPartitionModelDefault = GetDefault(bGFieldRelationSingle.To);
				if (bGMetaPartitionModelDefault != null)
				{
					BGFieldRelationSingle[] array = new BGFieldRelationSingle[num + 1];
					for (int i = 0; i <= num; i++)
					{
						array[i] = list2[i];
					}
					return new BGMetaPartitionModelNested(array, bGMetaPartitionModelDefault);
				}
			}
		}
		return GetDefault(meta);
	}

	public static BGMetaPartitionModelDefault GetDefault(BGMetaEntity meta)
	{
		BGField partitionField = GetPartitionField(meta);
		if (partitionField == null)
		{
			return null;
		}
		return new BGMetaPartitionModelDefault(partitionField);
	}

	private static BGField GetPartitionField(BGMetaEntity meta)
	{
		BGField field = meta.GetField("dbPartition", errorIfNotFound: false);
		if (field == null)
		{
			return null;
		}
		bool flag = false;
		if (!(field is BGFieldByte) && !(field is BGFieldShort) && !(field is BGFieldInt) && !(field is BGFieldByteNullable) && !(field is BGFieldShortNullable) && !(field is BGFieldIntNullable))
		{
			if (field is BGFieldRelationSingle bGFieldRelationSingle && string.Equals(bGFieldRelationSingle.RelatedMeta.Name, "DbPartition") && string.Equals(bGFieldRelationSingle.Name, "dbPartition"))
			{
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		if (!flag)
		{
			return null;
		}
		return field;
	}

	public void ForEachNotPartitionedMeta(BGRepo repo, Action<BGMetaEntity> action)
	{
		repo.ForEachMeta((BGMetaEntity meta) =>
		{
			BGMetaPartitionModelA bGMetaPartitionModelA = Get(meta);
			if (bGMetaPartitionModelA == null)
			{
				action(meta);
			}
		});
	}

	public void ForEachModelWithField(BGRepo repo, Action<BGMetaPartitionModelA.FieldOwner> action)
	{
		repo.ForEachMeta((BGMetaEntity meta) =>
		{
			BGMetaPartitionModelA bGMetaPartitionModelA = Get(meta);
			if (bGMetaPartitionModelA is BGMetaPartitionModelA.FieldOwner obj)
			{
				action(obj);
			}
		});
	}

	public void ForEachRootModel(BGRepo repo, Action<BGMetaPartitionModelI> action)
	{
		repo.ForEachMeta((BGMetaEntity meta) =>
		{
			BGMetaPartitionModelA bGMetaPartitionModelA = Get(meta);
			if (bGMetaPartitionModelA != null && bGMetaPartitionModelA.IsRoot)
			{
				action(bGMetaPartitionModelA);
			}
		});
	}
}
