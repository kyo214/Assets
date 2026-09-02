using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Trigonometry/Atan2")]
public class BGCalcUnitFloatAtan2 : BGCalcUnitFloatABFloat
{
	public const int Code = 40;

	public override ushort TypeCode => 40;

	protected override string OutputLabel => "Atan2(A)";

	protected override float Operation(float a, float b)
	{
		return Mathf.Atan2(a, b);
	}
}
