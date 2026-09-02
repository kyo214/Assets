using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "long?", Folder = "Primitive Nullable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerLongNullable")]
public class BGFieldLongNullable : BGFieldCachedStructNullableA<long>
{
	public const ushort CodeType = 41;

	public override ushort TypeCode => 41;

	public override bool CanBeUsedAsKey => true;

	protected override int ValueSize => 8;

	public BGFieldLongNullable(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldLongNullable(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(long value)
	{
		return BGFieldLong.ValueToBytes(value);
	}

	protected override long ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldLong.ValueFromBytes(segment);
	}

	protected override long ValueFromBytes(byte[] array, int offset)
	{
		return (long)(((ulong)array[offset + 7] << 56) | ((ulong)array[offset + 6] << 48) | ((ulong)array[offset + 5] << 40) | ((ulong)array[offset + 4] << 32) | ((ulong)array[offset + 3] << 24) | ((ulong)array[offset + 2] << 16) | ((ulong)array[offset + 1] << 8) | array[offset]);
	}

	protected override string ValueToString(long value)
	{
		return BGFieldLong.ValueToString(value);
	}

	protected override long? ValueFromString(string value)
	{
		try
		{
			return BGFieldLong.ValueFromString(value);
		}
		catch
		{
			return null;
		}
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldLongNullable(meta, id, name);
	}
}
