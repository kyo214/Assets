using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/int/Int max")]
public class BGCalcUnitIntMax : BGCalcUnitIntABInt
{
	public const int Code = 136;

	public override ushort TypeCode => 136;

	protected override string OutputLabel => "MAX(A,B)";

	protected override int Operation(int a, int b)
	{
		return Math.Max(a, b);
	}
}
