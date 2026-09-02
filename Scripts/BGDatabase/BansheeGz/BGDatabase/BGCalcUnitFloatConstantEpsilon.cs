using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/float/_Constants/Epsilon constant")]
public class BGCalcUnitFloatConstantEpsilon : BGCalcUnitFloatConstant
{
	public const int Code = 46;

	public override ushort TypeCode => 46;

	protected override string OutputLabel => "Epsilon";

	protected override float Operation()
	{
		return Mathf.Epsilon;
	}
}
