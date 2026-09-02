using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "bounds", Folder = "Unity Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerBounds")]
public class BGFieldBounds : BGFieldCachedStructA<Bounds>, BGBinaryBulkLoaderStruct
{
	public const ushort CodeType = 60;

	private const int Size = 24;

	public override ushort TypeCode => 60;

	protected override int ValueSize => 24;

	public override string Description => base.Description + ", " + Format;

	public static string Format => BGUtil.Format(" format is [center.x$center.y$center.z$extends.x$extends.y$extends.z] (without braces)", '`', '`', '`', '`', '`');

	public BGFieldBounds(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldBounds(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override bool AreStoredValuesEqual(Bounds myValue, Bounds otherValue)
	{
		if (BGFieldVector3.AreValuesEqual(myValue.center, otherValue.center))
		{
			return BGFieldVector3.AreValuesEqual(myValue.extents, otherValue.extents);
		}
		return false;
	}

	public override byte[] ToBytes(int entityIndex)
	{
		Bounds bounds = this[entityIndex];
		Vector3 center = bounds.center;
		Vector3 extents = bounds.extents;
		byte[] array = new byte[24];
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(center.x), 0, array, 0, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(center.y), 0, array, 4, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(center.z), 0, array, 8, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(extents.x), 0, array, 12, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(extents.y), 0, array, 16, 4);
		Buffer.BlockCopy(BGFieldFloat.ValueToBytes(extents.z), 0, array, 20, 4);
		return array;
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count != 24)
		{
			ClearValueNoEvent(entityIndex);
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
		this[entityIndex] = new Bounds
		{
			center = zero,
			extents = zero2
		};
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
				StoreItems[i] = new Bounds
				{
					center = zero,
					extents = zero2
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
		Bounds bounds = this[entityIndex];
		Vector3 center = bounds.center;
		Vector3 extents = bounds.extents;
		return BGUtil.Format("$$$$$$$$$$$", BGFieldFloat.ValueToString(center.x), '`', BGFieldFloat.ValueToString(center.y), '`', BGFieldFloat.ValueToString(center.z), '`', BGFieldFloat.ValueToString(extents.x), '`', BGFieldFloat.ValueToString(extents.y), '`', BGFieldFloat.ValueToString(extents.z));
	}

	public override void FromString(int entityIndex, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			ClearValueNoEvent(entityIndex);
			return;
		}
		string[] array = value.Split(new char[1] { '`' });
		if (array.Length != 6)
		{
			throw new BGException("Can not convert $ to Bounds." + Format, value);
		}
		this[entityIndex] = new Bounds
		{
			center = new Vector3(BGFieldFloat.ValueFromString(array[0]), BGFieldFloat.ValueFromString(array[1]), BGFieldFloat.ValueFromString(array[2])),
			extents = new Vector3(BGFieldFloat.ValueFromString(array[3]), BGFieldFloat.ValueFromString(array[4]), BGFieldFloat.ValueFromString(array[5]))
		};
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldBounds(meta, id, name);
	}
}
