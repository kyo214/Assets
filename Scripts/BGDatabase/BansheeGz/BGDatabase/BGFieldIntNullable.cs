using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "int?", Folder = "Primitive Nullable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerIntNullable")]
public class BGFieldIntNullable : BGFieldCachedStructNullableA<int>
{
	public const ushort CodeType = 40;

	public override ushort TypeCode => 40;

	protected override int ValueSize => 4;

	public override bool CanBeUsedAsKey => true;

	public BGFieldIntNullable(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldIntNullable(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(int value)
	{
		return BGFieldInt.ValueToBytes(value);
	}

	protected override int ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldInt.ValueFromBytes(segment);
	}

	protected override int ValueFromBytes(byte[] array, int offset)
	{
		return (array[offset + 3] << 24) | (array[offset + 2] << 16) | (array[offset + 1] << 8) | array[offset];
	}

	protected override string ValueToString(int value)
	{
		return BGFieldInt.ValueToString(value);
	}

	protected override int? ValueFromString(string value)
	{
		try
		{
			return BGFieldInt.ValueFromString(value);
		}
		catch
		{
			return null;
		}
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldIntNullable(meta, id, name);
	}
}
