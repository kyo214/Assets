using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/Pow")]
public class BGCalcUnitFloatPow : BGCalcUnitFloatABFloat
{
	public const int Code = 48;

	public override ushort TypeCode => 48;

	protected override string OutputLabel => "Pow(A, B)";

	protected override float Operation(float a, float b)
	{
		return Mathf.Pow(a, b);
	}
}
