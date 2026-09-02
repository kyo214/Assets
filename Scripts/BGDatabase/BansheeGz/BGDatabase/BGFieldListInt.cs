using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "listInt", Folder = "List/Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerListInt")]
public class BGFieldListInt : BGFieldCachedStructListA<int>
{
	public const ushort CodeType = 17;

	public override ushort TypeCode => 17;

	protected override int ValueSize => 4;

	public BGFieldListInt(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldListInt(BGMetaEntity meta, BGId id, string name)
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

	protected override int ValueFromString(string value)
	{
		return BGFieldInt.ValueFromString(value);
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldListInt(meta, id, name);
	}

	protected override bool AreEqual(int myValue, int myValue2)
	{
		return myValue == myValue2;
	}
}
