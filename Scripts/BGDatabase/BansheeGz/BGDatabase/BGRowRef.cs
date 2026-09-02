using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGRowRef
{
	[Serializable]
	private class JsonConfig
	{
		public string MetaId;

		public string EntityId;
	}

	private readonly BGId metaId;

	private readonly BGId entityId;

	public BGId MetaId => metaId;

	public BGId EntityId => entityId;

	public BGRowRef(BGId metaId, BGId entityId)
	{
		this.metaId = metaId;
		this.entityId = entityId;
	}

	public BGRowRef(string jsonString)
	{
		JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(jsonString);
		BGId.TryParse(jsonConfig.MetaId, out metaId);
		BGId.TryParse(jsonConfig.EntityId, out entityId);
	}

	public BGRowRef(BGEntity entity)
	{
		metaId = entity.MetaId;
		entityId = entity.Id;
	}

	public BGRowRef(ArraySegment<byte> segment)
	{
		metaId = new BGId(segment.Array, segment.Offset);
		entityId = new BGId(segment.Array, segment.Offset + 16);
	}

	public BGRowRef(byte[] array, int offset)
	{
		metaId = new BGId(array, offset);
		entityId = new BGId(array, offset + 16);
	}

	public BGEntity GetEntity(BGRepo repo)
	{
		return repo.GetMeta(metaId)?.GetEntity(entityId);
	}

	public string ToJson()
	{
		return JsonUtility.ToJson(new JsonConfig
		{
			MetaId = metaId.ToString(),
			EntityId = entityId.ToString()
		});
	}

	public string ToString(BGRepo repo)
	{
		if (repo == null)
		{
			return ToString();
		}
		BGEntity entity = GetEntity(repo);
		if (entity == null)
		{
			return "[not found]";
		}
		return entity.MetaName + "." + entity.Name;
	}

	public byte[] ToBytes()
	{
		byte[] result = new byte[32];
		metaId.ToByteArray(result, 0);
		entityId.ToByteArray(result, 16);
		return result;
	}

	protected bool Equals(BGRowRef other)
	{
		if (metaId.Equals(other.metaId))
		{
			return entityId.Equals(other.entityId);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((BGRowRef)obj);
	}

	public override int GetHashCode()
	{
		return (metaId.GetHashCode() * 397) ^ entityId.GetHashCode();
	}

	public static bool operator ==(BGRowRef left, BGRowRef right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(BGRowRef left, BGRowRef right)
	{
		return !object.Equals(left, right);
	}

	public bool IsMatch(BGEntity entity)
	{
		if (entity == null)
		{
			if (metaId.IsEmpty)
			{
				return entityId.IsEmpty;
			}
			return false;
		}
		if (entity.MetaId == metaId)
		{
			return entity.Id == entityId;
		}
		return false;
	}

	public override string ToString()
	{
		return metaId.ToString() + "." + entityId.ToString();
	}
}
