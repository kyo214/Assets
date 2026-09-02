using System;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeCalcAction : BGCalcTypeCode<BGFieldCalcActionValue>
{
	public const byte Code = 26;

	public override bool SupportDefaultValue => false;

	public override byte TypeCode => 26;

	public override object DefaultValue
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override string Name => "calcAction";

	public override bool CanBeConvertedFrom(BGCalcTypeCode otherCode)
	{
		return false;
	}

	public override object ConvertFrom(BGCalcTypeCode otherCode, object value)
	{
		return value;
	}

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		throw new NotImplementedException();
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		throw new NotImplementedException();
	}

	public override string ValueToString(object value)
	{
		throw new NotImplementedException();
	}

	public override object ValueFromString(string value)
	{
		throw new NotImplementedException();
	}
}
