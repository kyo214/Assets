using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/Ceil")]
public class BGCalcUnitFloatCeil : BGCalcUnitFloatAFloat
{
	public const int Code = 29;

	public override ushort TypeCode => 29;

	protected override string OutputLabel => "Ceil(A)";

	protected override float Operation(float a)
	{
		return Mathf.Ceil(a);
	}
}
