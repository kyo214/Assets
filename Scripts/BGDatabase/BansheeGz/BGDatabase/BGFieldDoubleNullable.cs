using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "double?", Folder = "Primitive Nullable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerDoubleNullable")]
public class BGFieldDoubleNullable : BGFieldCachedStructNullableA<double>
{
	public const ushort CodeType = 37;

	public override ushort TypeCode => 37;

	protected override int ValueSize => 8;

	public BGFieldDoubleNullable(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldDoubleNullable(BGMetaEntity meta, BGId id, string name)
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

	protected override double? ValueFromString(string value)
	{
		try
		{
			return BGFieldDouble.ValueFromString(value);
		}
		catch
		{
			return null;
		}
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldDoubleNullable(meta, id, name);
	}
}
