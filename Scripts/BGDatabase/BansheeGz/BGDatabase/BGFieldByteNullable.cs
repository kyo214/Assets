using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "byte?", Folder = "Primitive Nullable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerByteNullable")]
public class BGFieldByteNullable : BGFieldCachedStructNullableA<byte>
{
	public const ushort CodeType = 105;

	public override ushort TypeCode => 105;

	protected override int ValueSize => 1;

	public override bool CanBeUsedAsKey => true;

	public BGFieldByteNullable(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldByteNullable(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(byte value)
	{
		return BGFieldByte.ValueToBytes(value);
	}

	protected override byte ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldByte.ValueFromBytes(segment);
	}

	protected override byte ValueFromBytes(byte[] array, int offset)
	{
		return array[offset];
	}

	protected override string ValueToString(byte value)
	{
		return BGFieldByte.ValueToString(value);
	}

	protected override byte? ValueFromString(string value)
	{
		try
		{
			return BGFieldByte.ValueFromString(value);
		}
		catch
		{
			return null;
		}
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldByteNullable(meta, id, name);
	}
}
