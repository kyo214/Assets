using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Trigonometry/Tan")]
public class BGCalcUnitFloatTan : BGCalcUnitFloatAFloat
{
	public const int Code = 36;

	public override ushort TypeCode => 36;

	protected override string OutputLabel => "Tan(A)";

	protected override float Operation(float a)
	{
		return Mathf.Tan(a);
	}
}
