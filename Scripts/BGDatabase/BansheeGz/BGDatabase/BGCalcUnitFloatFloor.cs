using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/Floor")]
public class BGCalcUnitFloatFloor : BGCalcUnitFloatAFloat
{
	public const int Code = 31;

	public override ushort TypeCode => 31;

	protected override string OutputLabel => "Floor(A)";

	protected override float Operation(float a)
	{
		return Mathf.Floor(a);
	}
}
