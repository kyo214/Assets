using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/Float Abs")]
public class BGCalcUnitFloatAbs : BGCalcUnitFloatAFloat
{
	public const int Code = 25;

	public override ushort TypeCode => 25;

	protected override string OutputLabel => "Abs(A)";

	protected override float Operation(float a)
	{
		return Mathf.Abs(a);
	}
}
