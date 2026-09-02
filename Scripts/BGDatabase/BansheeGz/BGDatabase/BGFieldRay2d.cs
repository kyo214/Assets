using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "ray2d", Folder = "Unity Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerRay2d")]
public class BGFieldRay2d : BGFieldCachedStructA<Ray2D>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 66;

	private const int Size = 16;

	public override ushort TypeCode => 66;

	protected override int ValueSize => 16;

	public override string Description => base.Description + ", " + Format;

	public static string Format => BGUtil.Format(" format is [origin.x$origin.y$direction.x$direction.y] (without braces)", '`', '`', '`');

	public BGFieldRay2d(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldRay2d(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override bool AreStoredValuesEqual(Ray2D myValue, Ray2D otherValue)
	{
		if (BGFieldVector2.AreValuesEqual(myValue.origin, otherValue.origin))
		{
			return BGFieldVector2.AreValuesEqual(myValue.direction, otherValue.direction);
		}
		return false;
	}

	public override byte[] ToBytes(int entityIndex)
	{
		Ray2D ray2D = this[entityIndex];
		Vector2 origin = ray2D.origin;
		Vector2 direction = ray2D.direction;
		byte[] array = new byte[16];
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(origin.x), 0, array, 0, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(origin.y), 0, array, 4, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(direction.x), 0, array, 8, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(direction.y), 0, array, 12, 4);
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
		Vector2 zero = Vector2.zero;
		zero.x = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset, 4));
		zero.y = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 4, 4));
		Vector2 zero2 = Vector2.zero;
		zero2.x = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 8, 4));
		zero2.y = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 12, 4));
		this[entityIndex] = new Ray2D(zero, zero2);
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
				Vector2 zero = Vector2.zero;
				zero.x = BitConverter.ToSingle(array, num);
				zero.y = BitConverter.ToSingle(array, num + 4);
				Vector2 zero2 = Vector2.zero;
				zero2.x = BitConverter.ToSingle(array, num + 8);
				zero2.y = BitConverter.ToSingle(array, num + 12);
				StoreItems[i] = new Ray2D
				{
					origin = zero,
					direction = zero2
				};
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
		Ray2D ray2D = this[entityIndex];
		Vector2 origin = ray2D.origin;
		Vector2 direction = ray2D.direction;
		return BGUtil.Format("$$$$$$$", BGFieldFloat.ValueToString(origin.x), '`', BGFieldFloat.ValueToString(origin.y), '`', BGFieldFloat.ValueToString(direction.x), '`', BGFieldFloat.ValueToString(direction.y));
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
			throw new BGException("Can not convert $ to Ray2D. Should be [origin.x,origin.y,direction.x,direction.y] (without braces)", value);
		}
		this[entityIndex] = new Ray2D(new Vector2(BGFieldFloat.ValueFromString(array[0]), BGFieldFloat.ValueFromString(array[1])), new Vector2(BGFieldFloat.ValueFromString(array[2]), BGFieldFloat.ValueFromString(array[3])));
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldRay2d(meta, id, name);
	}
}
