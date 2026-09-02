using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/int/Int Abs")]
public class BGCalcUnitIntAbs : BGCalcUnitIntAInt
{
	public const int Code = 15;

	public override ushort TypeCode => 15;

	protected override string OutputLabel => "Abs(A)";

	protected override int Operation(int a)
	{
		return Math.Abs(a);
	}
}
