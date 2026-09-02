using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "vector2", Folder = "Unity Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerVector2")]
public class BGFieldVector2 : BGFieldCachedStructA<Vector2>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 68;

	public const int SizeOfTheValue = 8;

	public override ushort TypeCode => 68;

	protected override int ValueSize => 8;

	public override string Description => base.Description + ", " + Format;

	public static string Format => BGUtil.Format(" format is [x$y] (without braces)", '`');

	public BGFieldVector2(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldVector2(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override bool AreStoredValuesEqual(Vector2 myValue, Vector2 otherValue)
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
				int num = offset + 8 * i;
				StoreItems[i] = new Vector2(BitConverter.ToSingle(array, num), BitConverter.ToSingle(array, num + 4));
			}
		}
		else
		{
			for (int j = 0; j < entitiesCount; j++)
			{
				FromBytes(j, new ArraySegment<byte>(array, offset + 8 * j, 8));
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
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldVector2(meta, id, name);
	}

	public static byte[] ValueToBytes(Vector2 value)
	{
		byte[] array = new byte[8];
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(value.x), 0, array, 0, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(value.y), 0, array, 4, 4);
		return array;
	}

	public static Vector2 ValueFromBytes(ArraySegment<byte> segment)
	{
		Vector2 zero = Vector2.zero;
		if (segment.Count != 8)
		{
			return zero;
		}
		byte[] array = segment.Array;
		int offset = segment.Offset;
		zero.x = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset, 4));
		zero.y = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 4, 4));
		return zero;
	}

	public static string ValueToString(Vector2 value)
	{
		return BGUtil.Format("$$$", BGFieldFloat.ValueToString(value.x), '`', BGFieldFloat.ValueToString(value.y));
	}

	public static Vector2 ValueFromString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return Vector2.zero;
		}
		string[] array = value.Split(new char[1] { '`' });
		if (array.Length != 2)
		{
			throw new BGException("Can not convert $ to Vector2. " + Format, value, '`');
		}
		return new Vector2(BGFieldFloat.ValueFromString(array[0]), BGFieldFloat.ValueFromString(array[1]));
	}

	public static bool AreValuesEqual(Vector2 myValue, Vector2 otherValue)
	{
		if (Mathf.Approximately(myValue.x, otherValue.x))
		{
			return Mathf.Approximately(myValue.y, otherValue.y);
		}
		return false;
	}
}
