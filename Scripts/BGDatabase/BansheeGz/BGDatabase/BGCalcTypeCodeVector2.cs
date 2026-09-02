using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeVector2 : BGCalcTypeCode<Vector2>
{
	public const byte Code = 21;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 21;

	public override object DefaultValue => Vector2.zero;

	public override string Name => "Vector2";

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		Vector2 vector = (Vector2)value;
		writer.AddFloat(vector.x);
		writer.AddFloat(vector.y);
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		return new Vector2(reader.ReadFloat(), reader.ReadFloat());
	}

	public override string ValueToString(object value)
	{
		return BGFieldVector2.ValueToString((Vector2)value);
	}

	public override object ValueFromString(string value)
	{
		return BGFieldVector2.ValueFromString(value);
	}

	public override bool CanBeConvertedFrom(BGCalcTypeCode otherCode)
	{
		if (otherCode == null)
		{
			return false;
		}
		byte typeCode = otherCode.TypeCode;
		if ((uint)(typeCode - 22) <= 1u)
		{
			return true;
		}
		return false;
	}

	public override object ConvertFrom(BGCalcTypeCode otherCode, object value)
	{
		if (otherCode == null)
		{
			return value;
		}
		return otherCode.TypeCode switch
		{
			22 => (Vector2)(Vector3)value, 
			23 => (Vector2)(Vector4)value, 
			_ => value, 
		};
	}
}
