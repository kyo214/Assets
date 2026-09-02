using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "vector4?", Folder = "Unity Primitive Nullable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerVector4Nullable")]
public class BGFieldVector4Nullable : BGFieldCachedStructNullableA<Vector4>
{
	public const ushort CodeType = 75;

	public override ushort TypeCode => 75;

	protected override int ValueSize => 16;

	public BGFieldVector4Nullable(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldVector4Nullable(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(Vector4 value)
	{
		return BGFieldVector4.ValueToBytes(value);
	}

	protected override Vector4 ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldVector4.ValueFromBytes(segment);
	}

	protected override Vector4 ValueFromBytes(byte[] array, int offset)
	{
		return new Vector4(BitConverter.ToSingle(array, offset), BitConverter.ToSingle(array, offset + 4), BitConverter.ToSingle(array, offset + 8), BitConverter.ToSingle(array, offset + 12));
	}

	protected override string ValueToString(Vector4 value)
	{
		return BGFieldVector4.ValueToString(value);
	}

	protected override Vector4? ValueFromString(string value)
	{
		try
		{
			return BGFieldVector4.ValueFromString(value);
		}
		catch
		{
			return null;
		}
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldVector4Nullable(meta, id, name);
	}
}
