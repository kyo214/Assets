using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "short?", Folder = "Primitive Nullable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerShortNullable")]
public class BGFieldShortNullable : BGFieldCachedStructNullableA<short>
{
	public const ushort CodeType = 106;

	public override ushort TypeCode => 106;

	protected override int ValueSize => 2;

	public override bool CanBeUsedAsKey => true;

	public BGFieldShortNullable(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldShortNullable(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(short value)
	{
		return BGFieldShort.ValueToBytes(value);
	}

	protected override short ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldShort.ValueFromBytes(segment);
	}

	protected override short ValueFromBytes(byte[] array, int offset)
	{
		return (short)((array[offset + 1] << 8) | array[offset]);
	}

	protected override string ValueToString(short value)
	{
		return BGFieldShort.ValueToString(value);
	}

	protected override short? ValueFromString(string value)
	{
		try
		{
			return BGFieldShort.ValueFromString(value);
		}
		catch
		{
			return null;
		}
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldShortNullable(meta, id, name);
	}
}
