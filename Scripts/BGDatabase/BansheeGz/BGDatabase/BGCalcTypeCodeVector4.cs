using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeVector4 : BGCalcTypeCode<Vector4>
{
	public const byte Code = 23;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 23;

	public override object DefaultValue => Vector4.zero;

	public override string Name => "Vector4";

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		Vector4 vector = (Vector4)value;
		writer.AddFloat(vector.x);
		writer.AddFloat(vector.y);
		writer.AddFloat(vector.z);
		writer.AddFloat(vector.w);
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		return new Vector4(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
	}

	public override string ValueToString(object value)
	{
		return BGFieldVector4.ValueToString((Vector4)value);
	}

	public override object ValueFromString(string value)
	{
		return BGFieldVector4.ValueFromString(value);
	}

	public override bool CanBeConvertedFrom(BGCalcTypeCode otherCode)
	{
		if (otherCode == null)
		{
			return false;
		}
		byte typeCode = otherCode.TypeCode;
		if ((uint)(typeCode - 21) <= 1u)
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
			21 => (Vector4)(Vector2)value, 
			22 => (Vector4)(Vector3)value, 
			_ => value, 
		};
	}
}
