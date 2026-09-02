using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Trigonometry/Asin")]
public class BGCalcUnitFloatAsin : BGCalcUnitFloatAFloat
{
	public const int Code = 37;

	public override ushort TypeCode => 37;

	protected override string OutputLabel => "Asin(A)";

	protected override float Operation(float a)
	{
		return Mathf.Asin(a);
	}
}
