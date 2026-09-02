using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "color", Folder = "Unity Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerColor")]
public class BGFieldColor : BGFieldCachedStructA<Color>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 61;

	public const int SizeOfTheValue = 16;

	public override ushort TypeCode => 61;

	protected override int ValueSize => 16;

	public override string Description => base.Description + ", " + Format;

	public static string Format => BGUtil.Format(" format is [r$g$b$a] (without braces)", '`', '`', '`');

	public BGFieldColor(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldColor(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override bool AreStoredValuesEqual(Color myValue, Color otherValue)
	{
		if (Mathf.Approximately(myValue.r, otherValue.r) && Mathf.Approximately(myValue.g, otherValue.g) && Mathf.Approximately(myValue.b, otherValue.b))
		{
			return Mathf.Approximately(myValue.a, otherValue.a);
		}
		return false;
	}

	public override byte[] ToBytes(int entityIndex)
	{
		return ValueToBytes(this[entityIndex]);
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		this[entityIndex] = ValueFromBytes(segment);
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
				StoreItems[i] = new Color(BitConverter.ToSingle(array, num), BitConverter.ToSingle(array, num + 4), BitConverter.ToSingle(array, num + 8), BitConverter.ToSingle(array, num + 12));
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
		return ValueToString(this[entityIndex]);
	}

	public override void FromString(int entityIndex, string value)
	{
		this[entityIndex] = ValueFromString(value);
	}

	public static byte[] ValueToBytes(Color value)
	{
		byte[] array = new byte[16];
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(value.r), 0, array, 0, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(value.g), 0, array, 4, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(value.b), 0, array, 8, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(value.a), 0, array, 12, 4);
		return array;
	}

	public static Color ValueFromBytes(ArraySegment<byte> segment)
	{
		Color clear = Color.clear;
		if (segment.Count != 16)
		{
			return clear;
		}
		byte[] array = segment.Array;
		int offset = segment.Offset;
		clear.r = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset, 4));
		clear.g = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 4, 4));
		clear.b = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 8, 4));
		clear.a = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 12, 4));
		return clear;
	}

	public static string ValueToString(Color value)
	{
		return BGUtil.Format("$$$$$$$", BGFieldFloat.ValueToString(value.r), '`', BGFieldFloat.ValueToString(value.g), '`', BGFieldFloat.ValueToString(value.b), '`', BGFieldFloat.ValueToString(value.a));
	}

	public static Color ValueFromString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return Color.clear;
		}
		string[] array = value.Split(new char[1] { '`' });
		if (array.Length != 4)
		{
			throw new BGException("Can not convert $ to color." + Format, value);
		}
		return new Color(BGFieldFloat.ValueFromString(array[0]), BGFieldFloat.ValueFromString(array[1]), BGFieldFloat.ValueFromString(array[2]), BGFieldFloat.ValueFromString(array[3]));
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldColor(meta, id, name);
	}
}
