using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "listBool", Folder = "List/Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerListBool")]
public class BGFieldListBool : BGFieldCachedStructListA<bool>
{
	public const ushort CodeType = 13;

	public override ushort TypeCode => 13;

	protected override int ValueSize => 1;

	public BGFieldListBool(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldListBool(BGMetaEntity meta, BGId id, string name)
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

	protected override bool ValueFromString(string value)
	{
		return BGFieldBool.ValueFromString(value);
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldListBool(meta, id, name);
	}

	protected override bool AreEqual(bool myValue, bool myValue2)
	{
		return myValue == myValue2;
	}
}
