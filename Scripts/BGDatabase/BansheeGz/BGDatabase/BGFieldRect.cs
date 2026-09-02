using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "rect", Folder = "Unity Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerRect")]
public class BGFieldRect : BGFieldCachedStructA<Rect>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 67;

	private const int Size = 16;

	public override ushort TypeCode => 67;

	protected override int ValueSize => 16;

	public override string Description => base.Description + ", " + Format;

	public static string Format => BGUtil.Format("format is [x$y$width$height] (without braces)", '`', '`', '`');

	public BGFieldRect(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldRect(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override bool AreStoredValuesEqual(Rect myValue, Rect otherValue)
	{
		if (Mathf.Approximately(myValue.x, otherValue.x) && Mathf.Approximately(myValue.y, otherValue.y) && Mathf.Approximately(myValue.width, otherValue.width))
		{
			return Mathf.Approximately(myValue.height, otherValue.height);
		}
		return false;
	}

	public override byte[] ToBytes(int entityIndex)
	{
		Rect rect = this[entityIndex];
		byte[] array = new byte[16];
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(rect.x), 0, array, 0, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(rect.y), 0, array, 4, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(rect.width), 0, array, 8, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(rect.height), 0, array, 12, 4);
		return array;
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count != 16)
		{
			ClearValue(entityIndex);
			return;
		}
		byte[] array = segment.Array;
		int offset = segment.Offset;
		Rect zero = Rect.zero;
		zero.x = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset, 4));
		zero.y = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 4, 4));
		zero.width = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 8, 4));
		zero.height = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 12, 4));
		this[entityIndex] = zero;
	}

	public void FromBytes(BGBinaryBulkRequestStruct request)
	{
		byte[] array = request.Array;
		int offset = request.Offset;
		int entitiesCount = request.EntitiesCount;
		if (BitConverter.IsLittleEndian)
		{
			for (int i = 0; i < entitiesCount; i++)
			{
				int num = offset + 16 * i;
				Rect zero = Rect.zero;
				zero.x = BitConverter.ToSingle(array, num);
				zero.y = BitConverter.ToSingle(array, num + 4);
				zero.width = BitConverter.ToSingle(array, num + 8);
				zero.height = BitConverter.ToSingle(array, num + 12);
				StoreItems[i] = zero;
			}
		}
		else
		{
			for (int j = 0; j < entitiesCount; j++)
			{
				FromBytes(j, new ArraySegment<byte>(array, offset + 16 * j, 16));
			}
		}
	}

	public override string ToString(int entityIndex)
	{
		Rect rect = this[entityIndex];
		return BGUtil.Format("$$$$$$$", BGFieldFloat.ValueToString(rect.x), '`', BGFieldFloat.ValueToString(rect.y), '`', BGFieldFloat.ValueToString(rect.width), '`', BGFieldFloat.ValueToString(rect.height));
	}

	public override void FromString(int entityIndex, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			ClearValue(entityIndex);
			return;
		}
		string[] array = value.Split(new char[1] { '`' });
		if (array.Length != 4)
		{
			throw new BGException("Can not convert $ to Rect." + Format, value);
		}
		this[entityIndex] = new Rect(BGFieldFloat.ValueFromString(array[0]), BGFieldFloat.ValueFromString(array[1]), BGFieldFloat.ValueFromString(array[2]), BGFieldFloat.ValueFromString(array[3]));
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldRect(meta, id, name);
	}
}
