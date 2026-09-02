using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "bool?", Folder = "Primitive Nullable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerBoolNullable")]
public class BGFieldBoolNullable : BGFieldCachedStructNullableA<bool>
{
	public const ushort CodeType = 36;

	public override ushort TypeCode => 36;

	protected override int ValueSize => 1;

	public override bool CanBeUsedAsKey => true;

	public BGFieldBoolNullable(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldBoolNullable(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(bool value)
	{
		return BGFieldBool.ValueToBytes(value);
	}

	protected override bool ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldBool.ValueFromBytes(segment);
	}

	protected override bool ValueFromBytes(byte[] array, int offset)
	{
		return array[offset] != 0;
	}

	protected override string ValueToString(bool value)
	{
		return BGFieldBool.ValueToString(value);
	}

	protected override bool? ValueFromString(string value)
	{
		try
		{
			return BGFieldBool.ValueFromString(value);
		}
		catch
		{
			return null;
		}
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldBoolNullable(meta, id, name);
	}
}
