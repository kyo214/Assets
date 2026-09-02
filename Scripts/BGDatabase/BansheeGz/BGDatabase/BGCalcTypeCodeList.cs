using System;
using System.Collections;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeList : BGCalcTypeCode<IList>
{
	public const byte Code = 11;

	public override bool SupportDefaultValue => false;

	public override byte TypeCode => 11;

	public override object DefaultValue
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override string Name => "list";

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
