using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "guid?", Folder = "Primitive Nullable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerGuidNullable")]
public class BGFieldGuidNullable : BGFieldCachedStructNullableA<Guid>
{
	public const ushort CodeType = 39;

	public override ushort TypeCode => 39;

	public override bool CanBeUsedAsKey => true;

	protected override int ValueSize => 16;

	public BGFieldGuidNullable(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldGuidNullable(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(Guid value)
	{
		return BGFieldGuid.ValueToBytes(value);
	}

	protected override Guid ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldGuid.ValueFromBytes(segment);
	}

	protected override Guid ValueFromBytes(byte[] array, int offset)
	{
		byte[] array2 = new byte[16];
		Buffer.BlockCopy(array, offset, array2, 0, 16);
		return new Guid(array2);
	}

	protected override string ValueToString(Guid value)
	{
		return BGFieldGuid.ValueToString(value);
	}

	protected override Guid? ValueFromString(string value)
	{
		try
		{
			return BGFieldGuid.ValueFromString(value);
		}
		catch
		{
			return null;
		}
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldGuidNullable(meta, id, name);
	}
}
