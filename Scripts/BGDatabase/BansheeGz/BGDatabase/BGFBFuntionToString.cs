using System;

namespace BansheeGz.BGDatabase;

public class BGFBFuntionToString : BGFBFuntion
{
	public const short Code = 2;

	public override string Name => "ToString";

	public override Type ReturnType => typeof(string);

	public override bool Supports(BGField field)
	{
		return true;
	}

	public override object Convert(BGField field, BGEntity e, object value)
	{
		return value?.ToString();
	}
}
