using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "vector3", Folder = "Unity Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerVector3")]
public class BGFieldVector3 : BGFieldCachedStructA<Vector3>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 69;

	public const int SizeOfTheValue = 12;

	public override ushort TypeCode => 69;

	protected override int ValueSize => 12;

	public override string Description => base.Description + ", " + Format;

	public static string Format => BGUtil.Format(" format is [x$y$z] (without braces)", '`', '`');

	public BGFieldVector3(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldVector3(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override bool AreStoredValuesEqual(Vector3 myValue, Vector3 otherValue)
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
				int num = offset + 12 * i;
				StoreItems[i] = new Vector3(BitConverter.ToSingle(array, num), BitConverter.ToSingle(array, num + 4), BitConverter.ToSingle(array, num + 8));
			}
		}
		else
		{
			for (int j = 0; j < entitiesCount; j++)
			{
				FromBytes(j, new ArraySegment<byte>(array, offset + 12 * j, 12));
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
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldVector3(meta, id, name);
	}

	public static byte[] ValueToBytes(Vector3 value)
	{
		byte[] array = new byte[12];
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(value.x), 0, array, 0, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(value.y), 0, array, 4, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(value.z), 0, array, 8, 4);
		return array;
	}

	public static Vector3 ValueFromBytes(ArraySegment<byte> segment)
	{
		Vector3 zero = Vector3.zero;
		if (segment.Count != 12)
		{
			return zero;
		}
		byte[] array = segment.Array;
		int offset = segment.Offset;
		zero.x = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset, 4));
		zero.y = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 4, 4));
		zero.z = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 8, 4));
		return zero;
	}

	public static string ValueToString(Vector3 value)
	{
		return BGUtil.Format("$$$$$", BGFieldFloat.ValueToString(value.x), '`', BGFieldFloat.ValueToString(value.y), '`', BGFieldFloat.ValueToString(value.z));
	}

	public static Vector3 ValueFromString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return Vector3.zero;
		}
		string[] array = value.Split(new char[1] { '`' });
		if (array.Length != 3)
		{
			throw new BGException("Can not convert $ to Vector3." + Format, value);
		}
		return new Vector3(BGFieldFloat.ValueFromString(array[0]), BGFieldFloat.ValueFromString(array[1]), BGFieldFloat.ValueFromString(array[2]));
	}

	public static bool AreValuesEqual(Vector3 myValue, Vector3 otherValue)
	{
		if (Mathf.Approximately(myValue.x, otherValue.x) && Mathf.Approximately(myValue.y, otherValue.y))
		{
			return Mathf.Approximately(myValue.z, otherValue.z);
		}
		return false;
	}
}
