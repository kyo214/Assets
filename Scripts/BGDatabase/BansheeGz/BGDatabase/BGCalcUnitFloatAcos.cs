using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Trigonometry/Acos")]
public class BGCalcUnitFloatAcos : BGCalcUnitFloatAFloat
{
	public const int Code = 38;

	public override ushort TypeCode => 38;

	protected override string OutputLabel => "Acos(A)";

	protected override float Operation(float a)
	{
		return Mathf.Acos(a);
	}
}
