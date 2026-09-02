using System;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeControl : BGCalcTypeCode<BGCalcControl>
{
	public const byte Code = 1;

	public override bool SupportDefaultValue => false;

	public override object DefaultValue
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override byte TypeCode => 1;

	public override string Name => "control";

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
