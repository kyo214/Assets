using System.IO;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGPartitionSaveProcessor
{
	private class BGPartitionWrapper
	{
		private readonly BGRepo repo = new BGRepo();

		private BGMetaEntity cachedMeta;

		private readonly BGPartitionModelDefault partition;

		public BGRepo Repo => repo;

		public BGPartitionWrapper(BGPartitionModelDefault partition)
		{
			this.partition = partition;
		}

		public void Process(BGEntity entity)
		{
			if (cachedMeta == null || cachedMeta.Id != entity.MetaId)
			{
				cachedMeta = entity.Meta.CloneTo(repo, null, null, copyValues: false);
			}
			CopyRow(entity, cachedMeta);
		}
	}

	public BGPartitionSaveModel Save(BGPartitionSaveContext context)
	{
		if (Application.isEditor && Application.isPlaying)
		{
			throw new BGException("Saving runtime changes with partitions addon enabled is not supported, cause it may lead to unexpected results. You can disable partition addon temporarily in partition addon's settings after exiting play mode");
		}
		BGPartitionsModel partitions = context.partitions;
		string basicPath = context.basicPath;
		BGMetaPartitionModelProvider provider = context.provider;
		BGAddonPartition partitionAddon = context.partitionAddon;
		partitionAddon.CheckConfig();
		BGRepo repo = context.repo;
		BGRepo mainRepo = new BGRepo(repo);
		mainRepo.Addons.AddFrom(repo.Addons);
		BGAddonSettings.FormatEnum format = BGAddonSettings.GetFormat(mainRepo);
		BGPartitionWrapper[] partitionContexts = null;
		if (partitions != null)
		{
			partitionContexts = new BGPartitionWrapper[partitions.PartitionsCount];
			for (int i = 0; i < partitionContexts.Length; i++)
			{
				BGPartitionModelDefault partition = partitions.Partitions[i];
				BGPartitionWrapper bGPartitionWrapper = new BGPartitionWrapper(partition);
				if (format == BGAddonSettings.FormatEnum.Json)
				{
					bGPartitionWrapper.Repo.Addons.Add(new BGAddonSettings
					{
						Format = BGAddonSettings.FormatEnum.Json
					});
				}
				partitionContexts[i] = bGPartitionWrapper;
			}
		}
		BGPartitionSaveVerticalProvider verticalProvider = ((partitionAddon.EnabledVertical && !context.verticalDisabled) ? new BGPartitionSaveVerticalProvider(repo, partitionAddon) : null);
		repo.ForEachMeta((BGMetaEntity meta) =>
		{
			BGMetaEntity mainMeta = verticalProvider?.GetMeta(meta.Id) ?? mainRepo.GetMeta(meta.Id);
			BGMetaPartitionModelA metaModel = provider.Get(meta);
			if (metaModel == null || partitions == null)
			{
				CopyRows(meta, mainMeta);
			}
			else
			{
				meta.ForEachEntity((BGEntity entity) =>
				{
					int? partitionIndex = metaModel.GetPartitionIndex(entity);
					if (!partitionIndex.HasValue)
					{
						CopyRow(entity, mainMeta);
					}
					else if (partitionIndex.Value < 0 || partitionIndex.Value >= partitionContexts.Length)
					{
						Debug.Log("BGDatabase: WARNING- Can not get partition with index " + partitionIndex.Value + ", referenced by " + entity.FullName + " entity #" + entity.Index);
						CopyRow(entity, mainMeta);
					}
					else
					{
						partitionContexts[partitionIndex.Value]?.Process(entity);
					}
				});
			}
		});
		BGPartitionSaveModel result = new BGPartitionSaveModel();
		result.Add(basicPath, mainRepo.Save());
		string folder = Path.GetDirectoryName(basicPath);
		string basicPathNoExt = Path.GetFileNameWithoutExtension(basicPath);
		partitions?.ForEach((BGPartitionModelDefault bGPartitionModelDefault) =>
		{
			BGPartitionWrapper bGPartitionWrapper2 = partitionContexts[bGPartitionModelDefault.Entity.Index];
			if (bGPartitionWrapper2 != null)
			{
				string text = basicPathNoExt + "_p_" + BGAddonPartition.ToFilePath(bGPartitionModelDefault.Entity.Id);
				string text2 = text;
				if (folder != null)
				{
					text2 = Path.Combine(folder, text2);
				}
				result.Add(text2, bGPartitionWrapper2.Repo.Save());
			}
		});
		verticalProvider?.ForEachRepo((BGId id, BGRepo bGRepo) =>
		{
			string text = basicPathNoExt + "_v_" + BGAddonPartition.ToFilePath(id);
			if (folder != null)
			{
				text = Path.Combine(folder, text);
			}
			result.Add(text, bGRepo.Save());
		});
		return result;
	}

	public static void CopyRows(BGMetaEntity from, BGMetaEntity to)
	{
		from.ForEachEntity((BGEntity entity) =>
		{
			CopyRow(entity, to);
		});
	}

	public static void CopyRow(BGEntity entity, BGMetaEntity to)
	{
		BGEntity cloneEntity = to.NewEntity(entity.Id);
		entity.Meta.ForEachField((BGField field) =>
		{
			if (!field.EmptyContent)
			{
				BGField field2 = to.GetField(field.Index);
				field2.CopyValue(field, entity.Id, entity.Index, cloneEntity.Id);
			}
		});
	}
}
