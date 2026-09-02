using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/Float min")]
public class BGCalcUnitFloatMin : BGCalcUnitFloatABFloat
{
	public const int Code = 137;

	public override ushort TypeCode => 137;

	protected override string OutputLabel => "MIN(A,B)";

	protected override float Operation(float a, float b)
	{
		return Mathf.Min(a, b);
	}
}
