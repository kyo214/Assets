using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldRelationSA<T, TStoreType> : BGFieldRelationA<T, TStoreType>, BGRelationI, BGAbstractRelationI
{
	[Serializable]
	private struct JsonConfig
	{
		public string ToId;
	}

	internal BGId toId;

	public BGMetaEntity To => base.Meta.Repo.GetMeta(toId);

	public BGId ToId => toId;

	public BGMetaEntity RelatedMeta => To;

	protected BGFieldRelationSA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldRelationSA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected BGFieldRelationSA(BGMetaEntity meta, string name, BGMetaEntity to)
		: base(meta, name)
	{
		if (to == null)
		{
			base.Meta.Unregister(this);
			throw new BGException("'To' can not be null");
		}
		toId = to.Id;
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new JsonConfig
		{
			ToId = toId.ToString()
		});
	}

	public override void ConfigFromString(string config)
	{
		toId = new BGId(JsonUtility.FromJson<JsonConfig>(config).ToId);
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(20);
		bGBinaryWriter.AddInt(1);
		bGBinaryWriter.AddId(toId);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		if (num == 1)
		{
			toId = bGBinaryReader.ReadId();
			return;
		}
		throw new BGException("Unknown version: $", num);
	}

	public static BGId IdFromString(string value)
	{
		int num = value.LastIndexOf('_');
		BGId result;
		if (num < 0)
		{
			result = new BGId(value.Trim());
		}
		else
		{
			string value2 = value.Substring(num + 1).Trim();
			result = new BGId(value2);
		}
		return result;
	}
}
