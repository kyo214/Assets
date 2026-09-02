using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "ray", Folder = "Unity Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerRay")]
public class BGFieldRay : BGFieldCachedStructA<Ray>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 65;

	private const int Size = 24;

	public override ushort TypeCode => 65;

	protected override int ValueSize => 24;

	public override string Description => base.Description + ", " + Format;

	public static string Format => BGUtil.Format(" format is [origin.x$origin.y$origin.z$direction.x$direction.y$direction.z] (without braces)", '`', '`', '`', '`', '`');

	public BGFieldRay(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldRay(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override bool AreStoredValuesEqual(Ray myValue, Ray otherValue)
	{
		if (BGFieldVector3.AreValuesEqual(myValue.origin, otherValue.origin))
		{
			return BGFieldVector3.AreValuesEqual(myValue.direction, otherValue.direction);
		}
		return false;
	}

	public override byte[] ToBytes(int entityIndex)
	{
		Ray ray = this[entityIndex];
		Vector3 origin = ray.origin;
		Vector3 direction = ray.direction;
		byte[] array = new byte[24];
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(origin.x), 0, array, 0, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(origin.y), 0, array, 4, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(origin.z), 0, array, 8, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(direction.x), 0, array, 12, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(direction.y), 0, array, 16, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(direction.z), 0, array, 20, 4);
		return array;
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count != 24)
		{
			ClearValue(entityIndex);
			return;
		}
		byte[] array = segment.Array;
		int offset = segment.Offset;
		Vector3 zero = Vector3.zero;
		zero.x = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset, 4));
		zero.y = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 4, 4));
		zero.z = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 8, 4));
		Vector3 zero2 = Vector3.zero;
		zero2.x = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 12, 4));
		zero2.y = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 16, 4));
		zero2.z = BGFieldFloat.ValueFromBytes(new ArraySegment<byte>(array, offset + 20, 4));
		this[entityIndex] = new Ray(zero, zero2);
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
				int num = offset + 24 * i;
				Vector3 zero = Vector3.zero;
				zero.x = BitConverter.ToSingle(array, num);
				zero.y = BitConverter.ToSingle(array, num + 4);
				zero.z = BitConverter.ToSingle(array, num + 8);
				Vector3 zero2 = Vector3.zero;
				zero2.x = BitConverter.ToSingle(array, num + 12);
				zero2.y = BitConverter.ToSingle(array, num + 16);
				zero2.z = BitConverter.ToSingle(array, num + 20);
				StoreItems[i] = new Ray
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
				FromBytes(j, new ArraySegment<byte>(array, offset + 24 * j, 24));
			}
		}
	}

	public override string ToString(int entityIndex)
	{
		Ray ray = this[entityIndex];
		Vector3 origin = ray.origin;
		Vector3 direction = ray.direction;
		return BGUtil.Format("$$$$$$$$$$$", BGFieldFloat.ValueToString(origin.x), '`', BGFieldFloat.ValueToString(origin.y), '`', BGFieldFloat.ValueToString(origin.z), '`', BGFieldFloat.ValueToString(direction.x), '`', BGFieldFloat.ValueToString(direction.y), '`', BGFieldFloat.ValueToString(direction.z));
	}

	public override void FromString(int entityIndex, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			ClearValue(entityIndex);
			return;
		}
		string[] array = value.Split(new char[1] { '`' });
		if (array.Length != 6)
		{
			throw new BGException("Can not convert $ to Ray" + Format, value);
		}
		this[entityIndex] = new Ray(new Vector3(BGFieldFloat.ValueFromString(array[0]), BGFieldFloat.ValueFromString(array[1]), BGFieldFloat.ValueFromString(array[2])), new Vector3(BGFieldFloat.ValueFromString(array[3]), BGFieldFloat.ValueFromString(array[4]), BGFieldFloat.ValueFromString(array[5])));
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldRay(meta, id, name);
	}
}
