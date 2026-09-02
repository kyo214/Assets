using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

internal class BGRepoDeltaDeleted
{
	private readonly Dictionary<BGId, List<BGId>> metaId2EntityIds = new Dictionary<BGId, List<BGId>>();

	private const int LastVersion = 1;

	public void Match(BGRepo repo, BGRepo targetRepo)
	{
		metaId2EntityIds.Clear();
		BGRepoDeltaUtils.ForEachMatchingMeta(repo, targetRepo, (BGMetaEntity meta, BGMetaEntity targetMeta) =>
		{
			if (meta.CountEntities != 0)
			{
				List<BGId> list = BGRepoDeltaUtils.Except(meta, targetMeta);
				if (list.Count != 0)
				{
					metaId2EntityIds[meta.Id] = list;
				}
			}
		});
	}

	public void ApplyTo(BGRepo repo, BGModdingRepoProtection repoProtection)
	{
		foreach (KeyValuePair<BGId, List<BGId>> metaId2EntityId in metaId2EntityIds)
		{
			BGMetaEntity bGMetaEntity = repo[metaId2EntityId.Key];
			if (bGMetaEntity == null || metaId2EntityId.Value.Count == 0)
			{
				continue;
			}
			HashSet<BGEntity> hashSet = new HashSet<BGEntity>();
			foreach (BGId item in metaId2EntityId.Value)
			{
				BGEntity entity = bGMetaEntity.GetEntity(item);
				if (entity != null && (repoProtection == null || !repoProtection.IsDeleteDisabled(bGMetaEntity.Id, entity.Id)))
				{
					hashSet.Add(entity);
				}
			}
			if (hashSet.Count != 0)
			{
				bGMetaEntity.DeleteEntities(hashSet);
			}
		}
	}

	public void ToBinary(BGBinaryWriter builder)
	{
		builder.AddInt(1);
		builder.AddArray(() =>
		{
			foreach (KeyValuePair<BGId, List<BGId>> metaId2EntityId in metaId2EntityIds)
			{
				builder.AddId(metaId2EntityId.Key);
				builder.AddArray(() =>
				{
					foreach (BGId item in metaId2EntityId.Value)
					{
						builder.AddId(item);
					}
				}, metaId2EntityId.Value.Count);
			}
		}, metaId2EntityIds.Count);
	}

	public void FromBinary(BGBinaryReader reader)
	{
		metaId2EntityIds.Clear();
		int num = reader.ReadInt();
		if (num == 1)
		{
			reader.ReadArray(() =>
			{
				BGId key = reader.ReadId();
				List<BGId> entityIdList = new List<BGId>();
				metaId2EntityIds[key] = entityIdList;
				reader.ReadArray(() =>
				{
					entityIdList.Add(reader.ReadId());
				});
			});
			return;
		}
		throw new BGException("Can not read repo delta deleted from binary array: unsupported version $", num);
	}
}
