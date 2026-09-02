namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/object/null literal")]
public class BGCalcUnitNull : BGCalcUnit
{
	public const int Code = 51;

	public override ushort TypeCode => 51;

	public override void Definition()
	{
		ValueOutput(BGCalcTypeCodeRegistry.Object, "null", "r", (BGCalcFlowI flow) => (object)null);
	}
}
