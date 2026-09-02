using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldRelationMA<T, TStoreType> : BGFieldRelationA<T, TStoreType>, BGManyTablesRelationI, BGAbstractRelationI
{
	[Serializable]
	private struct JsonConfig
	{
		public List<string> ToIds;
	}

	private readonly List<BGId> toIds = new List<BGId>();

	public virtual List<BGId> ToIds => toIds;

	public virtual List<BGMetaEntity> RelatedMetas
	{
		get
		{
			List<BGMetaEntity> list = new List<BGMetaEntity>();
			foreach (BGId toId in toIds)
			{
				BGMetaEntity meta = base.Meta.Repo.GetMeta(toId);
				if (meta != null)
				{
					list.Add(meta);
				}
			}
			return list;
		}
	}

	protected BGFieldRelationMA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldRelationMA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected BGFieldRelationMA(BGMetaEntity meta, string name, List<BGMetaEntity> to)
		: base(meta, name)
	{
		if (to == null || to.Count == 0)
		{
			base.Meta.Unregister(this);
			throw new BGException("'To' can not be null or empty");
		}
		foreach (BGMetaEntity item in to)
		{
			toIds.Add(item.Id);
		}
	}

	public virtual void RemoveRelatedMeta(BGMetaEntity metaEntity)
	{
		if (toIds.RemoveAll((BGId id) => id == metaEntity.Id) != 0)
		{
			OnRemoveRelatedMeta(metaEntity);
			base.events.MetaWasChanged(base.Meta);
		}
	}

	public virtual void AddRelatedMeta(BGMetaEntity metaEntity)
	{
		if (!toIds.Contains(metaEntity.Id))
		{
			toIds.Add(metaEntity.Id);
			base.events.MetaWasChanged(base.Meta);
		}
	}

	protected abstract void OnRemoveRelatedMeta(BGMetaEntity metaEntity);

	protected virtual void CheckMetaId(BGEntity entity)
	{
		BGId metaId = entity.MetaId;
		bool flag = false;
		for (int i = 0; i < toIds.Count; i++)
		{
			if (!(toIds[i] != metaId))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			throw new BGException("Can not assign related entity: meta [$] is not included in field's settings!", entity.MetaName);
		}
	}

	public override string ConfigToString()
	{
		List<BGMetaEntity> relatedMetas = RelatedMetas;
		JsonConfig jsonConfig = new JsonConfig
		{
			ToIds = new List<string>(relatedMetas.Count)
		};
		foreach (BGMetaEntity item in relatedMetas)
		{
			jsonConfig.ToIds.Add(item.Id.ToString());
		}
		return JsonUtility.ToJson(jsonConfig);
	}

	public override void ConfigFromString(string config)
	{
		toIds.Clear();
		if (string.IsNullOrEmpty(config))
		{
			return;
		}
		JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(config);
		if (jsonConfig.ToIds == null)
		{
			return;
		}
		foreach (string toId in jsonConfig.ToIds)
		{
			if (BGId.TryParse(toId, out var item))
			{
				toIds.Add(item);
			}
		}
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter writer = new BGBinaryWriter(32);
		writer.AddInt(1);
		List<BGMetaEntity> relatedMetas = RelatedMetas;
		writer.AddArray(() =>
		{
			foreach (BGMetaEntity item in relatedMetas)
			{
				writer.AddId(item.Id);
			}
		}, relatedMetas.Count);
		return writer.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		toIds.Clear();
		BGBinaryReader reader = new BGBinaryReader(config);
		int num = reader.ReadInt();
		if (num == 1)
		{
			reader.ReadArray(() =>
			{
				toIds.Add(reader.ReadId());
			});
			return;
		}
		throw new BGException("Unknown version: $", num);
	}

	public static BGRowRef StringToRowRef(string value)
	{
		BGRowRef result = null;
		int num = value.LastIndexOf('_');
		if (num > 0 && value.Length - num == 23 && BGId.TryParse(value.Substring(num + 1, 22), out var entityId))
		{
			int num2 = value.LastIndexOf('_', num - 1);
			BGId metaId2;
			if (num2 > 0)
			{
				if (num - num2 == 23 && BGId.TryParse(value.Substring(num2 + 1, 22), out var metaId))
				{
					result = new BGRowRef(metaId, entityId);
				}
			}
			else if (num == 22 && BGId.TryParse(value.Substring(0, 22), out metaId2))
			{
				result = new BGRowRef(metaId2, entityId);
			}
		}
		return result;
	}

	public static string RowRefToString(BGRowRef rowRef, BGRepo repo)
	{
		if (rowRef == null)
		{
			return "";
		}
		string text = RowRefToString(rowRef);
		BGEntity entity = rowRef.GetEntity(repo);
		if (entity == null || string.IsNullOrEmpty(entity.Name))
		{
			return text;
		}
		return entity.MetaName + "." + entity.Name.Trim() + "_" + text;
	}

	public static string RowRefToString(BGRowRef rowRef)
	{
		string text = rowRef.MetaId.ToString();
		string text2 = rowRef.EntityId.ToString();
		return text + "_" + text2;
	}
}
