using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeGameObject : BGCalcTypeCode<GameObject>
{
	public const byte Code = 24;

	public override bool SupportDefaultValue => false;

	public override byte TypeCode => 24;

	public override object DefaultValue
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override string Name => "GameObject";

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
