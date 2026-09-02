using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "vector4", Folder = "Unity Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerVector4")]
public class BGFieldVector4 : BGFieldCachedStructA<Vector4>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 70;

	public const int SizeOfTheValue = 16;

	public override ushort TypeCode => 70;

	protected override int ValueSize => 16;

	public override string Description => base.Description + ", " + Format;

	public static string Format => BGUtil.Format(" [x$y$z$w] (without braces)", '`', '`', '`');

	public BGFieldVector4(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldVector4(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override bool AreStoredValuesEqual(Vector4 myValue, Vector4 otherValue)
	{
		return AreValuesEqual(myValue, otherValue);
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
				StoreItems[i] = new Vector4(BitConverter.ToSingle(array, num), BitConverter.ToSingle(array, num + 4), BitConverter.ToSingle(array, num + 8), BitConverter.ToSingle(array, num + 12));
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

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldVector4(meta, id, name);
	}

	public static byte[] ValueToBytes(Vector4 value)
	{
		byte[] array = new byte[16];
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(value.x), 0, array, 0, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(value.y), 0, array, 4, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(value.z), 0, array, 8, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(value.w), 0, array, 12, 4);
		return array;
	}

	public static Vector4 ValueFromBytes(ArraySegment<byte> segment)
	{
		Vector4 zero = Vector4.zero;
		if (segment.Count != 16)
		{
			return zero;
		}
		byte[] array = segment.Array;
		int offset = segment.Offset;
		zero.x = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset, 4));
		zero.y = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 4, 4));
		zero.z = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 8, 4));
		zero.w = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 12, 4));
		return zero;
	}

	public static string ValueToString(Vector4 value)
	{
		return BGUtil.Format("$$$$$$$", BGFieldFloat.ValueToString(value.x), '`', BGFieldFloat.ValueToString(value.y), '`', BGFieldFloat.ValueToString(value.z), '`', BGFieldFloat.ValueToString(value.w));
	}

	public static Vector4 ValueFromString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return Vector4.zero;
		}
		string[] array = value.Split(new char[1] { '`' });
		if (array.Length != 4)
		{
			throw new BGException("Can not convert $ to Vector4." + Format, value);
		}
		return new Vector4(BGFieldFloat.ValueFromString(array[0]), BGFieldFloat.ValueFromString(array[1]), BGFieldFloat.ValueFromString(array[2]), BGFieldFloat.ValueFromString(array[3]));
	}

	public static bool AreValuesEqual(Vector4 myValue, Vector4 otherValue)
	{
		if (Mathf.Approximately(myValue.x, otherValue.x) && Mathf.Approximately(myValue.y, otherValue.y) && Mathf.Approximately(myValue.z, otherValue.z))
		{
			return Mathf.Approximately(myValue.w, otherValue.w);
		}
		return false;
	}
}
