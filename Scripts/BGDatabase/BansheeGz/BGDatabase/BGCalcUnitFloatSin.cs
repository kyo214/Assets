using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Trigonometry/Sin")]
public class BGCalcUnitFloatSin : BGCalcUnitFloatAFloat
{
	public const int Code = 34;

	public override ushort TypeCode => 34;

	protected override string OutputLabel => "Sin(A)";

	protected override float Operation(float a)
	{
		return Mathf.Sin(a);
	}
}
