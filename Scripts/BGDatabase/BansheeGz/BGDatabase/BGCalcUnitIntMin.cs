using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/int/Int min")]
public class BGCalcUnitIntMin : BGCalcUnitIntABInt
{
	public const int Code = 135;

	public override ushort TypeCode => 135;

	protected override string OutputLabel => "MIN(A,B)";

	protected override int Operation(int a, int b)
	{
		return Math.Min(a, b);
	}
}
