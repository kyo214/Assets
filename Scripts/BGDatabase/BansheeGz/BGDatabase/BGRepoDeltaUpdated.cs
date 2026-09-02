using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

internal class BGRepoDeltaUpdated
{
	private class MetaUpdated
	{
		private readonly Dictionary<BGId, List<BGId>> fieldId2EntityIds = new Dictionary<BGId, List<BGId>>();

		public void ForEach(Action<BGId, List<BGId>> action)
		{
			foreach (KeyValuePair<BGId, List<BGId>> fieldId2EntityId in fieldId2EntityIds)
			{
				action(fieldId2EntityId.Key, fieldId2EntityId.Value);
			}
		}

		public void Add(BGId fieldId, BGId entityId)
		{
			if (!fieldId2EntityIds.TryGetValue(fieldId, out var value))
			{
				value = new List<BGId>();
				fieldId2EntityIds[fieldId] = value;
			}
			value.Add(entityId);
		}

		public static MetaUpdated Ensure(BGId metaId, Dictionary<BGId, MetaUpdated> id2Updated)
		{
			if (id2Updated.TryGetValue(metaId, out var value))
			{
				return value;
			}
			return id2Updated[metaId] = new MetaUpdated();
		}

		internal void FromBinary(BGBinaryReader reader)
		{
			fieldId2EntityIds.Clear();
			reader.ReadArray(() =>
			{
				BGId key = reader.ReadId();
				List<BGId> entityIdList = new List<BGId>();
				fieldId2EntityIds[key] = entityIdList;
				reader.ReadArray(() =>
				{
					entityIdList.Add(reader.ReadId());
				});
			});
		}

		internal void ToBinary(BGBinaryWriter builder)
		{
			builder.AddArray(() =>
			{
				foreach (KeyValuePair<BGId, List<BGId>> fieldId2EntityId in fieldId2EntityIds)
				{
					builder.AddId(fieldId2EntityId.Key);
					builder.AddArray(() =>
					{
						foreach (BGId item in fieldId2EntityId.Value)
						{
							builder.AddId(item);
						}
					}, fieldId2EntityId.Value.Count);
				}
			}, fieldId2EntityIds.Count);
		}
	}

	private readonly BGRepo updated = new BGRepo();

	private readonly Dictionary<BGId, MetaUpdated> metaId2Updated = new Dictionary<BGId, MetaUpdated>();

	private const int LastVersion = 1;

	public void Match(BGRepo repo, BGRepo targetRepo)
	{
		updated.Clear();
		metaId2Updated.Clear();
		BGRepoDeltaUtils.ForEachMatchingMeta(repo, targetRepo, (BGMetaEntity meta, BGMetaEntity targetMeta) =>
		{
			List<BGField> fields = new List<BGField>();
			List<BGField> targetFields = new List<BGField>();
			BGRepoDeltaUtils.ForEachMatchingField(meta, targetMeta, (BGField field, BGField targetField) =>
			{
				fields.Add(field);
				targetFields.Add(targetField);
			});
			if (fields.Count != 0)
			{
				BGRepoDeltaUtils.ForEachMatchingEntity(meta, targetMeta, (BGEntity entity, BGEntity targetEntity) =>
				{
					for (int i = 0; i < fields.Count; i++)
					{
						BGField bGField = fields[i];
						BGField bGField2 = targetFields[i];
						if (!bGField.AreStoredValuesEqual(bGField2, entity.Index, targetEntity.Index))
						{
							BGField bGField3 = EnsureField(bGField);
							BGEntity bGEntity = EnsureEntity(bGField3.Meta, targetEntity.Id);
							bGField3.CopyValue(bGField2, targetEntity.Id, targetEntity.Index, bGEntity.Id);
							MetaUpdated metaUpdated = MetaUpdated.Ensure(meta.Id, metaId2Updated);
							metaUpdated.Add(bGField3.Id, bGEntity.Id);
						}
					}
				});
			}
		});
	}

	private BGEntity EnsureEntity(BGMetaEntity meta, BGId entityId)
	{
		BGEntity entity = meta.GetEntity(entityId);
		if (entity != null)
		{
			return entity;
		}
		return meta.NewEntity(entityId);
	}

	private BGField EnsureField(BGField field)
	{
		BGMetaEntity bGMetaEntity = updated.GetMeta(field.MetaId);
		if (bGMetaEntity == null)
		{
			bGMetaEntity = BGRepoDeltaUtils.CreateMeta(updated, field.Meta);
		}
		BGField bGField = bGMetaEntity.GetField(field.Id, errorIfNotFound: false);
		if (bGField == null)
		{
			bGField = BGRepoDeltaUtils.CreateField(bGMetaEntity, field);
		}
		return bGField;
	}

	public void ApplyTo(BGRepo repo, BGModdingRepoProtection repoProtection)
	{
		foreach (KeyValuePair<BGId, MetaUpdated> item in metaId2Updated)
		{
			BGId metaId = item.Key;
			MetaUpdated value = item.Value;
			BGMetaEntity fromMeta = updated[metaId];
			if (fromMeta == null)
			{
				continue;
			}
			BGMetaEntity toMeta = repo[metaId];
			if (toMeta == null)
			{
				continue;
			}
			value.ForEach((BGId fieldId, List<BGId> entityIds) =>
			{
				if (entityIds.Count != 0)
				{
					BGField field = fromMeta.GetField(fieldId, errorIfNotFound: false);
					if (field != null)
					{
						BGField field2 = toMeta.GetField(fieldId, errorIfNotFound: false);
						if (field2 != null)
						{
							foreach (BGId entityId in entityIds)
							{
								BGEntity entity = fromMeta.GetEntity(entityId);
								if (entity != null)
								{
									BGEntity entity2 = toMeta.GetEntity(entityId);
									if (entity2 != null && (repoProtection == null || !repoProtection.IsEditDisabled(metaId, fieldId, entity2.Id)))
									{
										field2.CopyValue(field, entity.Id, entity.Index, entity2.Id);
									}
								}
							}
						}
					}
				}
			});
		}
	}

	public void ToBinary(BGBinaryWriter builder)
	{
		builder.AddInt(1);
		builder.AddByteArray(updated.Save());
		builder.AddArray(() =>
		{
			foreach (KeyValuePair<BGId, MetaUpdated> item in metaId2Updated)
			{
				builder.AddId(item.Key);
				item.Value.ToBinary(builder);
			}
		}, metaId2Updated.Count);
	}

	public void FromBinary(BGBinaryReader reader)
	{
		updated.Clear();
		metaId2Updated.Clear();
		int num = reader.ReadInt();
		if (num == 1)
		{
			updated.Load(BGRepoDeltaUtils.ToArray(reader.ReadByteArray()));
			reader.ReadArray(() =>
			{
				BGId key = reader.ReadId();
				MetaUpdated metaUpdated = new MetaUpdated();
				metaId2Updated[key] = metaUpdated;
				metaUpdated.FromBinary(reader);
			});
			return;
		}
		throw new BGException("Can not read repo delta deleted from binary array: unsupported version $", num);
	}
}
