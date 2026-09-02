using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "listDouble", Folder = "List/Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerListDouble")]
public class BGFieldListDouble : BGFieldCachedStructListA<double>
{
	public const ushort CodeType = 14;

	public override ushort TypeCode => 14;

	protected override int ValueSize => 8;

	public BGFieldListDouble(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldListDouble(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override byte[] ValueToBytes(double value)
	{
		return BGFieldDouble.ValueToBytes(value);
	}

	protected override double ValueFromBytes(ArraySegment<byte> segment)
	{
		return BGFieldDouble.ValueFromBytes(segment);
	}

	protected override double ValueFromBytes(byte[] array, int offset)
	{
		return BitConverter.ToDouble(array, offset);
	}

	protected override string ValueToString(double value)
	{
		return BGFieldDouble.ValueToString(value);
	}

	protected override double ValueFromString(string value)
	{
		return BGFieldDouble.ValueFromString(value);
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldListDouble(meta, id, name);
	}

	protected override bool AreEqual(double myValue, double myValue2)
	{
		return Math.Abs(myValue - myValue2) < 1E-05;
	}
}
