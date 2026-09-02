using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/Float max")]
public class BGCalcUnitFloatMax : BGCalcUnitFloatABFloat
{
	public const int Code = 138;

	public override ushort TypeCode => 138;

	protected override string OutputLabel => "MAX(A,B)";

	protected override float Operation(float a, float b)
	{
		return Mathf.Max(a, b);
	}
}
