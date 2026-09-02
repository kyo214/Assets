using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeComponent : BGCalcTypeCode<Component>
{
	public const byte Code = 25;

	public override bool SupportDefaultValue => false;

	public override byte TypeCode => 25;

	public override object DefaultValue
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override string Name => "Component";

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
