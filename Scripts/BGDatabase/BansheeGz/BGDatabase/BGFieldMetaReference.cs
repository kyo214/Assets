using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "metaReference", Folder = "Special", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerMetaReference")]
public class BGFieldMetaReference : BGFieldCachedA<BGMetaEntity, BGId>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 108;

	public override ushort TypeCode => 108;

	public override int ConstantSize => 16;

	public override BGMetaEntity this[int entityIndex]
	{
		get
		{
			if (entityIndex >= StoreCount)
			{
				ThrowIndexOutOfBoundOnRead(entityIndex);
			}
			BGId bGId = StoreItems[entityIndex];
			if (bGId.IsEmpty)
			{
				return null;
			}
			return base.Meta.Repo.GetMeta(bGId);
		}
		set
		{
			if (base.events.On)
			{
				BGId bGId = StoreItems[entityIndex];
				if ((value != null || !bGId.IsEmpty) && (value == null || !(value.Id == bGId)))
				{
					BGMetaEntity oldValue = this[entityIndex];
					BGEntity entity = base.Meta[entityIndex];
					FireBeforeValueChanged(entity, oldValue, value);
					StoreSet(entityIndex, value?.Id ?? BGId.Empty);
					FireValueChanged(entity, oldValue, value);
				}
			}
			else
			{
				StoreSet(entityIndex, value?.Id ?? BGId.Empty);
			}
		}
	}

	public BGFieldMetaReference(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldMetaReference(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldMetaReference(meta, id, name);
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(4);
		bGBinaryWriter.AddInt(1);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		if (num != 1)
		{
			throw new BGException("Unknown version: $", num);
		}
	}

	public override byte[] ToBytes(int entityIndex)
	{
		return BGFieldId.ValueToBytes(StoreItems[entityIndex]);
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		StoreItems[entityIndex] = BGFieldId.ValueFromBytes(segment);
	}

	public void FromBytes(BGBinaryBulkRequestStruct request)
	{
		byte[] array = request.Array;
		int offset = request.Offset;
		int entitiesCount = request.EntitiesCount;
		for (int i = 0; i < entitiesCount; i++)
		{
			StoreItems[i] = new BGId(array, offset + 16 * i);
		}
	}

	public override string ToString(int entityIndex)
	{
		BGId bGId = StoreItems[entityIndex];
		if (bGId.IsEmpty)
		{
			return null;
		}
		BGMetaEntity bGMetaEntity = this[entityIndex];
		if (bGMetaEntity == null)
		{
			return bGId.ToString();
		}
		string text = bGMetaEntity.Name;
		BGId bGId2 = bGId;
		return text + "_" + bGId2.ToString();
	}

	public override void FromString(int entityIndex, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			StoreItems[entityIndex] = BGId.Empty;
			return;
		}
		int num = value.LastIndexOf('_');
		if (num == -1 || num >= value.Length - 2)
		{
			StoreItems[entityIndex] = new BGId(value);
			return;
		}
		string value2 = value.Substring(num + 1, value.Length - num - 1);
		StoreItems[entityIndex] = new BGId(value2);
	}
}
