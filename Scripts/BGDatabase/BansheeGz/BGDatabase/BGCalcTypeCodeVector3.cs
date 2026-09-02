using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeVector3 : BGCalcTypeCode<Vector3>
{
	public const byte Code = 22;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 22;

	public override object DefaultValue => Vector3.zero;

	public override string Name => "Vector3";

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		Vector3 vector = (Vector3)value;
		writer.AddFloat(vector.x);
		writer.AddFloat(vector.y);
		writer.AddFloat(vector.z);
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		return new Vector3(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
	}

	public override string ValueToString(object value)
	{
		return BGFieldVector3.ValueToString((Vector3)value);
	}

	public override object ValueFromString(string value)
	{
		return BGFieldVector3.ValueFromString(value);
	}

	public override bool CanBeConvertedFrom(BGCalcTypeCode otherCode)
	{
		if (otherCode == null)
		{
			return false;
		}
		byte typeCode = otherCode.TypeCode;
		if (typeCode == 21 || typeCode == 23)
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
			21 => (Vector3)(Vector2)value, 
			23 => (Vector3)(Vector4)value, 
			_ => value, 
		};
	}
}
