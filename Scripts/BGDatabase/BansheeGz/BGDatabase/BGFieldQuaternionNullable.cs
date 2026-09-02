using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "quaternion?", Folder = "Unity Primitive Nullable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerQuaternionNullable")]
public class BGFieldQuaternionNullable : BGFieldCachedStructNullableA<Quaternion>
{
	public const ushort CodeType = 72;

	public override ushort TypeCode => 72;

	protected override int ValueSize => 16;

	public BGFieldQuaternionNullable(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldQuaternionNullable(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(Quaternion value)
	{
		return BGFieldQuaternion.ValueToBytes(value);
	}

	protected override Quaternion ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldQuaternion.ValueFromBytes(segment);
	}

	protected override Quaternion ValueFromBytes(byte[] array, int offset)
	{
		return new Quaternion(BitConverter.ToSingle(array, offset), BitConverter.ToSingle(array, offset + 4), BitConverter.ToSingle(array, offset + 8), BitConverter.ToSingle(array, offset + 12));
	}

	protected override string ValueToString(Quaternion value)
	{
		return BGFieldQuaternion.ValueToString(value);
	}

	protected override Quaternion? ValueFromString(string value)
	{
		try
		{
			return BGFieldQuaternion.ValueFromString(value);
		}
		catch
		{
			return null;
		}
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldQuaternionNullable(meta, id, name);
	}
}
