using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "color?", Folder = "Unity Primitive Nullable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerColorNullable")]
public class BGFieldColorNullable : BGFieldCachedStructNullableA<Color>
{
	public const ushort CodeType = 71;

	public override ushort TypeCode => 71;

	protected override int ValueSize => 16;

	public BGFieldColorNullable(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldColorNullable(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(Color value)
	{
		return BGFieldColor.ValueToBytes(value);
	}

	protected override Color ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldColor.ValueFromBytes(segment);
	}

	protected override Color ValueFromBytes(byte[] array, int offset)
	{
		return new Color(BitConverter.ToSingle(array, offset), BitConverter.ToSingle(array, offset + 4), BitConverter.ToSingle(array, offset + 8), BitConverter.ToSingle(array, offset + 12));
	}

	protected override string ValueToString(Color value)
	{
		return BGFieldColor.ValueToString(value);
	}

	protected override Color? ValueFromString(string value)
	{
		try
		{
			return BGFieldColor.ValueFromString(value);
		}
		catch
		{
			return null;
		}
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldColorNullable(meta, id, name);
	}
}
