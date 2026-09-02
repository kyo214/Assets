using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/Floor to int")]
public class BGCalcUnitFloatFloorToInt : BGCalcUnitFloatAInt
{
	public const int Code = 32;

	public override ushort TypeCode => 32;

	protected override string OutputLabel => "FloorToInt(A)";

	protected override int Operation(float a)
	{
		return Mathf.FloorToInt(a);
	}
}
