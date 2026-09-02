using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Trigonometry/Atan")]
public class BGCalcUnitFloatAtan : BGCalcUnitFloatAFloat
{
	public const int Code = 39;

	public override ushort TypeCode => 39;

	protected override string OutputLabel => "Atan(A)";

	protected override float Operation(float a)
	{
		return Mathf.Atan(a);
	}
}
