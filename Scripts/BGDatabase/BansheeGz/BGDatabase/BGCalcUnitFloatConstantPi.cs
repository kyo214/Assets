using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Constants/PI")]
public class BGCalcUnitFloatConstantPi : BGCalcUnitFloatConstant
{
	public const int Code = 43;

	public override ushort TypeCode => 43;

	protected override string OutputLabel => "PI";

	protected override float Operation()
	{
		return MathF.PI;
	}
}
