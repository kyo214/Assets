using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Trigonometry/Deg2Rad")]
public class BGCalcUnitFloatDeg2Rad : BGCalcUnitFloatAFloat
{
	public const int Code = 41;

	public override ushort TypeCode => 41;

	protected override string OutputLabel => "Deg2Rad(A)";

	protected override float Operation(float a)
	{
		return MathF.PI / 180f * a;
	}
}
