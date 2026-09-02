using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/Ceil to int")]
public class BGCalcUnitFloatCeilToInt : BGCalcUnitFloatAInt
{
	public const int Code = 30;

	public override ushort TypeCode => 30;

	protected override string OutputLabel => "CeilToInt(A)";

	protected override int Operation(float a)
	{
		return Mathf.CeilToInt(a);
	}
}
