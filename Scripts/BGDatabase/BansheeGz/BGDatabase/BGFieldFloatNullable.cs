using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "float?", Folder = "Primitive Nullable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerFloatNullable")]
public class BGFieldFloatNullable : BGFieldCachedStructNullableA<float>
{
	public const ushort CodeType = 38;

	public override ushort TypeCode => 38;

	protected override int ValueSize => 4;

	public BGFieldFloatNullable(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldFloatNullable(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(float value)
	{
		return BGFieldFloat.ValueToBytes(value);
	}

	protected override float ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldFloat.ValueFromBytes(segment);
	}

	protected override float ValueFromBytes(byte[] array, int offset)
	{
		return BitConverter.ToSingle(array, offset);
	}

	protected override string ValueToString(float value)
	{
		return BGFieldFloat.ValueToString(value);
	}

	protected override float? ValueFromString(string value)
	{
		try
		{
			return BGFieldFloat.ValueFromString(value);
		}
		catch
		{
			return null;
		}
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldFloatNullable(meta, id, name);
	}
}
