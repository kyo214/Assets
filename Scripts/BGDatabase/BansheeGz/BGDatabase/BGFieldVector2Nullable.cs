using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "vector2?", Folder = "Unity Primitive Nullable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerVector2Nullable")]
public class BGFieldVector2Nullable : BGFieldCachedStructNullableA<Vector2>
{
	public const ushort CodeType = 73;

	public override ushort TypeCode => 73;

	protected override int ValueSize => 8;

	public BGFieldVector2Nullable(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldVector2Nullable(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(Vector2 value)
	{
		return BGFieldVector2.ValueToBytes(value);
	}

	protected override Vector2 ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldVector2.ValueFromBytes(segment);
	}

	protected override Vector2 ValueFromBytes(byte[] array, int offset)
	{
		return new Vector2(BitConverter.ToSingle(array, offset), BitConverter.ToSingle(array, offset + 4));
	}

	protected override string ValueToString(Vector2 value)
	{
		return BGFieldVector2.ValueToString(value);
	}

	protected override Vector2? ValueFromString(string value)
	{
		try
		{
			return BGFieldVector2.ValueFromString(value);
		}
		catch
		{
			return null;
		}
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldVector2Nullable(meta, id, name);
	}
}
