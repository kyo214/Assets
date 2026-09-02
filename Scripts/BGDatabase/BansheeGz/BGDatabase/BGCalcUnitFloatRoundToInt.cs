using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/RoundToInt")]
public class BGCalcUnitFloatRoundToInt : BGCalcUnitFloatAInt
{
	public const int Code = 49;

	public override ushort TypeCode => 49;

	protected override string OutputLabel => "RoundToInt(A)";

	protected override int Operation(float a)
	{
		return Mathf.RoundToInt(a);
	}
}
