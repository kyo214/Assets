using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Trigonometry/Cos")]
public class BGCalcUnitFloatCos : BGCalcUnitFloatAFloat
{
	public const int Code = 35;

	public override ushort TypeCode => 35;

	protected override string OutputLabel => "Cos(A)";

	protected override float Operation(float a)
	{
		return Mathf.Cos(a);
	}
}
