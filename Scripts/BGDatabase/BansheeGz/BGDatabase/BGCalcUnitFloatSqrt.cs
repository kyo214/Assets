using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/Sqrt")]
public class BGCalcUnitFloatSqrt : BGCalcUnitFloatAFloat
{
	public const int Code = 47;

	public override ushort TypeCode => 47;

	protected override string OutputLabel => "Sqrt(A)";

	protected override float Operation(float a)
	{
		return Mathf.Sqrt(a);
	}
}
