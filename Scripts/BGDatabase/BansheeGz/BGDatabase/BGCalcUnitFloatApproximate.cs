using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/Approximately")]
public class BGCalcUnitFloatApproximate : BGCalcUnitFloatABBool
{
	public const int Code = 33;

	public override ushort TypeCode => 33;

	protected override string OutputLabel => "Approximately(A, B)";

	protected override bool Operation(float a, float b)
	{
		return Mathf.Approximately(a, b);
	}
}
