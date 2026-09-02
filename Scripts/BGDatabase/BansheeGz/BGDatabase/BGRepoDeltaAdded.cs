using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

internal class BGRepoDeltaAdded
{
	private readonly BGRepo added = new BGRepo();

	private const int LastVersion = 1;

	public void Match(BGRepo repo, BGRepo targetRepo)
	{
		added.Clear();
		BGRepoDeltaUtils.ForEachMatchingMeta(repo, targetRepo, (BGMetaEntity meta, BGMetaEntity targetMeta) =>
		{
			if (targetMeta.CountEntities != 0)
			{
				List<BGId> addedIds = BGRepoDeltaUtils.Except(targetMeta, meta);
				if (addedIds.Count != 0)
				{
					BGMetaEntity bGMetaEntity = Create(meta, targetMeta);
					BGEntity[] newEntities = new BGEntity[addedIds.Count];
					for (int i = 0; i < addedIds.Count; i++)
					{
						newEntities[i] = bGMetaEntity.NewEntity(addedIds[i]);
					}
					BGRepoDeltaUtils.ForEachMatchingField(bGMetaEntity, targetMeta, (BGField field, BGField targetField) =>
					{
						for (int j = 0; j < addedIds.Count; j++)
						{
							BGEntity bGEntity = newEntities[j];
							BGEntity entity = targetMeta.GetEntity(addedIds[j]);
							if (entity != null)
							{
								field.CopyValue(targetField, entity.Id, entity.Index, bGEntity.Id);
							}
						}
					});
				}
			}
		});
	}

	private BGMetaEntity Create(BGMetaEntity meta, BGMetaEntity targetMeta)
	{
		BGMetaEntity myMeta = BGRepoDeltaUtils.CreateMeta(added, meta);
		BGRepoDeltaUtils.ForEachMatchingField(meta, targetMeta, (BGField field, BGField targetField) =>
		{
			BGField bGField = BGRepoDeltaUtils.CreateField(myMeta, field);
		});
		return myMeta;
	}

	public void ApplyTo(BGRepo repo, BGModdingRepoProtection repoProtection)
	{
		BGRepoDeltaUtils.ForEachMatchingMeta(added, repo, (BGMetaEntity meta, BGMetaEntity targetMeta) =>
		{
			if (repoProtection == null || !repoProtection.IsAddDisabled(meta.Id))
			{
				int countEntities = meta.CountEntities;
				if (countEntities != 0)
				{
					BGEntity[] newEntities = new BGEntity[countEntities];
					for (int i = 0; i < countEntities; i++)
					{
						BGId id = meta.GetEntity(i).Id;
						BGEntity bGEntity = targetMeta.GetEntity(id) ?? targetMeta.NewEntity(id);
						newEntities[i] = bGEntity;
					}
					BGRepoDeltaUtils.ForEachMatchingField(meta, targetMeta, (BGField field, BGField targetField) =>
					{
						for (int j = 0; j < countEntities; j++)
						{
							BGEntity entity = meta.GetEntity(j);
							BGEntity bGEntity2 = newEntities[j];
							targetField.CopyValue(field, entity.Id, entity.Index, bGEntity2.Id);
						}
					});
				}
			}
		});
	}

	public void ToBinary(BGBinaryWriter builder)
	{
		builder.AddInt(1);
		builder.AddByteArray(added.Save());
	}

	public void FromBinary(BGBinaryReader reader)
	{
		added.Clear();
		int num = reader.ReadInt();
		if (num == 1)
		{
			added.Load(BGRepoDeltaUtils.ToArray(reader.ReadByteArray()));
			return;
		}
		throw new BGException("Can not read repo delta deleted from binary array: unsupported version $", num);
	}
}
