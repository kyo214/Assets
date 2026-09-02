using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "vector3?", Folder = "Unity Primitive Nullable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerVector3Nullable")]
public class BGFieldVector3Nullable : BGFieldCachedStructNullableA<Vector3>
{
	public const ushort CodeType = 74;

	public override ushort TypeCode => 74;

	protected override int ValueSize => 12;

	public BGFieldVector3Nullable(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldVector3Nullable(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(Vector3 value)
	{
		return BGFieldVector3.ValueToBytes(value);
	}

	protected override Vector3 ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldVector3.ValueFromBytes(segment);
	}

	protected override Vector3 ValueFromBytes(byte[] array, int offset)
	{
		return new Vector3(BitConverter.ToSingle(array, offset), BitConverter.ToSingle(array, offset + 4), BitConverter.ToSingle(array, offset + 8));
	}

	protected override string ValueToString(Vector3 value)
	{
		return BGFieldVector3.ValueToString(value);
	}

	protected override Vector3? ValueFromString(string value)
	{
		try
		{
			return BGFieldVector3.ValueFromString(value);
		}
		catch
		{
			return null;
		}
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldVector3Nullable(meta, id, name);
	}
}
